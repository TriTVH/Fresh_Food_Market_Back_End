using RedisConfigitguration.Contract;
using RedisConfiguration.Contract;
using RedisConfiguration.Utils;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.RedisStreamBus.Implementor
{
    public class RedisStreamBus : IRedisStreamBus
    {
        private readonly IConnectionMultiplexer _mux;
        private IDatabase Db => _mux.GetDatabase();

        public RedisStreamBus(IConnectionMultiplexer mux)
        {
            _mux = mux;
        }

        public async Task EnsureGroupExistsAsync(string stream, string group)
        {
            try
            {
                await Db.ExecuteAsync("XGROUP", "CREATE", stream, group, "$", "MKSTREAM");
            }
            catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
            {
                // group already exists
            }
        }

        public async Task<string> PublishAsync(string stream, StreamEnvelope envelope)
        {
            var result = await Db.ExecuteAsync(
                "XADD", stream, "*",
                "messageId", envelope.MessageId,
                "messageType", envelope.MessageType,
                "source", envelope.Source,
                "sagaId", envelope.SagaId,
                "correlationId", envelope.CorrelationId,
                "createdAtUtc", envelope.CreatedAtUtc.ToString("O"),
                "payload", envelope.Payload
            );

            return result.ToString()!;
        }

        public async Task<List<RedisStreamEntry>> ReadGroupAsync(
            string stream,
            string group,
            string consumer,
            int count,
            int blockMs)
        {
            var result = await Db.ExecuteAsync(
                "XREADGROUP",
                "GROUP", group, consumer,
                "COUNT", count,
                "BLOCK", blockMs,
                "STREAMS", stream, ">"
            );

            return RedisStreamParser.ParseReadGroupResult(result);
        }

        public async Task AckAsync(string stream, string group, string messageRedisId)
        {
            await Db.ExecuteAsync("XACK", stream, group, messageRedisId);
        }

        public async Task<List<RedisStreamEntry>> AutoClaimAsync(
            string stream,
            string group,
            string consumer,
            int minIdleMs,
            string startId,
            int count)
        {
            var result = await Db.ExecuteAsync(
                "XAUTOCLAIM",
                stream,
                group,
                consumer,
                minIdleMs,
                startId,
                "COUNT",
                count
            );
            return RedisStreamParser.ParseAutoClaimResult(result);
        }

       
    }

   
   
}
