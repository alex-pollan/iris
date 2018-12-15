using Microsoft.Extensions.Configuration;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace Iris.Distributed.Redis
{
    public interface IAppRedisConfiguration
    {
        string Host { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        string Channel { get; set; }
        RedisConfiguration ToRedisConfiguration();
    }

    public class AppRedisConfiguration : IAppRedisConfiguration
    {
        public AppRedisConfiguration(IConfiguration configuration)
        {
            Host = configuration["vcap:services:user-provided:1:credentials:host"];
            Password = configuration["vcap:services:user-provided:1:credentials:password"];
            Port = int.Parse(configuration["vcap:services:user-provided:1:credentials:port"]);
            Channel = configuration["vcap:services:user-provided:1:credentials:channel"];            
        }

        public string Host { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Channel { get; set; }

        public RedisConfiguration ToRedisConfiguration()
        {
            return new RedisConfiguration
            {
                Hosts = new[] { new RedisHost { Host = Host, Port = Port } },
                Password = Password
            };
        }
    }
}
