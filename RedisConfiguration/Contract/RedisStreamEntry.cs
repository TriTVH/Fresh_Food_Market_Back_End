using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfigitguration.Contract
{
    public class RedisStreamEntry
    {
        public string MessageRedisId { get; set; } = default!;
        public Dictionary<string, string> Values { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);
    }
}
