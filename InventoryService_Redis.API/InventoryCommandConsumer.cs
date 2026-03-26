using RedisConfigitguration.Contract;
using RedisConfiguration.Command;
using RedisConfiguration.Contract;
using RedisConfiguration.Message;
using RedisConfiguration.RedisStreamBus;
using RedisConfiguration.Reply;
using RedisConfiguration.Utils;

namespace InventoryService_Redis.API
{
    public sealed class InventoryCommandConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InventoryCommandConsumer> _logger;
        private readonly string _consumerName = $"inventory-{Environment.MachineName}-{Guid.NewGuid():N}";

        public InventoryCommandConsumer(IServiceScopeFactory scopeFactory, ILogger<InventoryCommandConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var initScope = _scopeFactory.CreateScope();
            var initBus = initScope.ServiceProvider.GetRequiredService<IRedisStreamBus>();
            await initBus.EnsureGroupExistsAsync(StreamConstants.InventoryCommands, StreamConstants.InventoryGroup);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var bus = scope.ServiceProvider.GetRequiredService<IRedisStreamBus>();
                    var store = scope.ServiceProvider.GetRequiredService<InventoryStore>();

                    var messages = await bus.ReadGroupAsync(
                        StreamConstants.InventoryCommands,
                        StreamConstants.InventoryGroup,
                        _consumerName,
                        count: 10,
                        blockMs: 1000);

                    foreach (var msg in messages)
                    {
                        try
                        {
                            var env = ToEnvelope(msg);

                            if (!store.TryMarkProcessed(env.MessageId))
                            {
                                await bus.AckAsync(StreamConstants.InventoryCommands, StreamConstants.InventoryGroup, msg.MessageRedisId);
                                continue;
                            }

                            if (env.MessageType == MessageType.ReserveInventory)
                            {
                                var cmd = JsonHelper.Deserialize<ReserveInventoryCommand>(env.Payload)!;

                                if (store.Reserve(cmd.OrderId, cmd.Items, out var reservationId, out var reason))
                                {
                                    var reply = new InventoryReservedReply
                                    {
                                        SagaId = cmd.SagaId,
                                        OrderId = cmd.OrderId,
                                        ReservationId = reservationId
                                    };

                                    await bus.PublishAsync(StreamConstants.SagaReplies, new StreamEnvelope
                                    {
                                        MessageType = MessageType.InventoryReserved,
                                        Source = "inventory-service",
                                        SagaId = cmd.SagaId,
                                        CorrelationId = cmd.OrderId,
                                        Payload = JsonHelper.Serialize(reply)
                                    });
                                }
                                else
                                {
                                    var reply = new InventoryReserveFailedReply
                                    {
                                        SagaId = cmd.SagaId,
                                        OrderId = cmd.OrderId,
                                        Reason = reason ?? "INVENTORY_FAILED"
                                    };

                                    await bus.PublishAsync(StreamConstants.SagaReplies, new StreamEnvelope
                                    {
                                        MessageType = MessageType.InventoryReserveFailed,
                                        Source = "inventory-service",
                                        SagaId = cmd.SagaId,
                                        CorrelationId = cmd.OrderId,
                                        Payload = JsonHelper.Serialize(reply)
                                    });
                                }
                            }

                            await bus.AckAsync(StreamConstants.InventoryCommands, StreamConstants.InventoryGroup, msg.MessageRedisId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Inventory failed processing message {RedisId}", msg.MessageRedisId);
                        }
                    }

                    var reclaimed = await bus.AutoClaimAsync(
                        StreamConstants.InventoryCommands,
                        StreamConstants.InventoryGroup,
                        _consumerName,
                        minIdleMs: 30000,
                        startId: "0-0",
                        count: 10);

                    if (reclaimed.Count > 0)
                    {
                        _logger.LogInformation("Inventory reclaimed {Count} pending command messages", reclaimed.Count);
                    }

                    await Task.Delay(200, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "InventoryCommandConsumer loop error");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private static StreamEnvelope ToEnvelope(RedisStreamEntry msg)
        {
            return new StreamEnvelope
            {
                MessageId = msg.Values["messageId"],
                MessageType = msg.Values["messageType"],
                Source = msg.Values["source"],
                SagaId = msg.Values["sagaId"],
                CorrelationId = msg.Values["correlationId"],
                CreatedAtUtc = DateTime.Parse(msg.Values["createdAtUtc"]),
                Payload = msg.Values["payload"]
            };
        }
    }
}
