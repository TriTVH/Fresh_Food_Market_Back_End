using RedisConfigitguration.Contract;
using RedisConfiguration.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.RedisStreamBus
{
    public interface IRedisStreamBus
    {
        Task EnsureGroupExistsAsync(string stream, string group);

         Task<string> PublishAsync(string stream, StreamEnvelope envelope);

        Task<List<RedisStreamEntry>> ReadGroupAsync(
            string stream,
            string group,
            string consumer,
            int count,
            int blockMs);

        Task AckAsync(string stream, string group, string messageRedisId);

        Task<List<RedisStreamEntry>> AutoClaimAsync(
            string stream,
            string group,
            string consumer,
            int minIdleMs,
            string startId,
            int count);
    }
}
