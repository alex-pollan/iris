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
