using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public static class JsonMessageSerializer
    {
        private static readonly JsonSerializerOptions Opt = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static byte[] Serialize(object message)
        {
            var json = JsonSerializer.Serialize(message);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T Deserialize<T>(byte[] body)
        {
            var json = Encoding.UTF8.GetString(body);
            return JsonSerializer.Deserialize<T>(json)!;
        }
    }
}
