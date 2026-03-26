using RedisConfigitguration.Contract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.Utils
{
    public static class RedisStreamParser
    {
        public static List<RedisStreamEntry> ParseReadGroupResult(RedisResult result)
        {
            var entries = new List<RedisStreamEntry>();
            if (result.IsNull) return entries;

            var outer = (RedisResult[])result!;
            if (outer.Length == 0) return entries;

            foreach (var streamBlock in outer)
            {
                var streamArr = (RedisResult[])streamBlock;
                if (streamArr.Length < 2) continue;

                var messages = (RedisResult[])streamArr[1];
                foreach (var message in messages)
                {
                    var msgArr = (RedisResult[])message;
                    var redisId = msgArr[0].ToString()!;
                    var fields = (RedisResult[])msgArr[1];

                    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < fields.Length; i += 2)
                    {
                        var key = fields[i].ToString()!;
                        var value = i + 1 < fields.Length ? fields[i + 1].ToString()! : string.Empty;
                        dict[key] = value;
                    }

                    entries.Add(new RedisStreamEntry
                    {
                        MessageRedisId = redisId,
                        Values = dict
                    });
                }
            }

            return entries;
        }

        public static List<RedisStreamEntry> ParseAutoClaimResult(RedisResult result)
        {
            var entries = new List<RedisStreamEntry>();
            if (result.IsNull) return entries;

            var arr = (RedisResult[])result!;
            if (arr.Length < 2) return entries;

            var claimedMessages = (RedisResult[])arr[1];

            foreach (var msg in claimedMessages)
            {
                var msgArr = (RedisResult[])msg;
                var redisId = msgArr[0].ToString()!;
                var fields = (RedisResult[])msgArr[1];

                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < fields.Length; i += 2)
                {
                    var key = fields[i].ToString()!;
                    var value = i + 1 < fields.Length ? fields[i + 1].ToString()! : string.Empty;
                    dict[key] = value;
                }

                entries.Add(new RedisStreamEntry
                {
                    MessageRedisId = redisId,
                    Values = dict
                });
            }

            return entries;
        }
    }
}
