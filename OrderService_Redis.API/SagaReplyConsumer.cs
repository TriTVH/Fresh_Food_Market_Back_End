using RedisConfigitguration.Contract;
using RedisConfiguration.Contract;
using RedisConfiguration.Message;
using RedisConfiguration.RedisStreamBus;
using RedisConfiguration.Reply;
using RedisConfiguration.Utils;

namespace OrderService_Redis.API
{
    public sealed class SagaReplyConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SagaReplyConsumer> _logger;
        private readonly string _consumerName = $"order-{Environment.MachineName}-{Guid.NewGuid():N}";

        public SagaReplyConsumer(IServiceScopeFactory scopeFactory, ILogger<SagaReplyConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var initScope = _scopeFactory.CreateScope();
            var initBus = initScope.ServiceProvider.GetRequiredService<IRedisStreamBus>();
            await initBus.EnsureGroupExistsAsync(StreamConstants.SagaReplies, StreamConstants.OrderGroup);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var bus = scope.ServiceProvider.GetRequiredService<IRedisStreamBus>();
                    var sagaStore = scope.ServiceProvider.GetRequiredService<SagaStore>();
                    var orderStore = scope.ServiceProvider.GetRequiredService<OrderStore>();

                    var messages = await bus.ReadGroupAsync(
                        StreamConstants.SagaReplies,
                        StreamConstants.OrderGroup,
                        _consumerName,
                        count: 10,
                        blockMs: 1000);

                    foreach (var msg in messages)
                    {
                        try
                        {
                            var env = ToEnvelope(msg);

                            if (!sagaStore.TryMarkProcessed(env.MessageId))
                            {
                                await bus.AckAsync(StreamConstants.SagaReplies, StreamConstants.OrderGroup, msg.MessageRedisId);
                                continue;
                            }

                            switch (env.MessageType)
                            {
                                case MessageType.InventoryReserved:
                                    {
                                        var reply = JsonHelper.Deserialize<InventoryReservedReply>(env.Payload)!;
                                        sagaStore.MarkInventoryReserved(reply.SagaId, reply.ReservationId);
                                        orderStore.MarkPackaging(reply.OrderId);
                                        break;
                                    }

                                case MessageType.InventoryReserveFailed:
                                    {
                                        var reply = JsonHelper.Deserialize<InventoryReserveFailedReply>(env.Payload)!;
                                        sagaStore.MarkInventoryFailed(reply.SagaId, reply.Reason);
                                        orderStore.MarkOutOfStock(reply.OrderId);
                                        break;
                                    }
                            }

                            await bus.AckAsync(StreamConstants.SagaReplies, StreamConstants.OrderGroup, msg.MessageRedisId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed processing reply message {RedisId}", msg.MessageRedisId);
                        }
                    }

                    var reclaimed = await bus.AutoClaimAsync(
                        StreamConstants.SagaReplies,
                        StreamConstants.OrderGroup,
                        _consumerName,
                        minIdleMs: 30000,
                        startId: "0-0",
                        count: 10);

                    if (reclaimed.Count > 0)
                    {
                        _logger.LogInformation("OrderService reclaimed {Count} pending reply messages", reclaimed.Count);
                    }

                    await Task.Delay(200, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SagaReplyConsumer loop error");
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
