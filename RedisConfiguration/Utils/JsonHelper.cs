using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisConfiguration.Utils
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public static string Serialize<T>(T obj)
            => JsonSerializer.Serialize(obj, Options);

        public static T? Deserialize<T>(string json)
            => JsonSerializer.Deserialize<T>(json, Options);
    }
}
