using Iris.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Iris.Messaging.Nsq.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddIrisNsqInboundMessaging(this IServiceCollection services, 
            Action<NsqOptions> options)
        {
            var nsqOptions = new NsqOptions();

            options(nsqOptions);

            services.AddSingleton<IInboundMessageQueue>(sp => 
            {
                var dispatcher = sp.GetService<IMessageDispatcher>();
                var logger = sp.GetService<ILogger>();
                var configuration = new NsqConfiguration(nsqOptions.Endpoints,
                    nsqOptions.MessageTypeTopics, nsqOptions.MessageTypeHandlerChannels);
                return new NsqInboundMessageQueue(dispatcher, logger, configuration);
            });


            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IInboundMessageQueue>().Start();

            return services;
        }
    }

    public class NsqOptions
    {
        internal NsqOptions() { }

        internal string Endpoints { get; private set; }
        internal Dictionary<Type, string> MessageTypeTopics { get; } = new Dictionary<Type, string>();
        internal Dictionary<Type, string> MessageTypeHandlerChannels { get; } = new Dictionary<Type, string>();

        public void UseEndpoints(string endpoints)
        {
            Endpoints = endpoints;
        }

        public void AcceptMessage<T>(string topic, string channelName) where T : class, IUserMessage
        {
            MessageTypeTopics.Add(typeof(T), topic);
            MessageTypeHandlerChannels.Add(typeof(NsqHandler<T>), channelName);
        }
    }
}
