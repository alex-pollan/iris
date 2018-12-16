using Iris.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Iris.Messaging.Nsq
{
    public static class Extensions
    {
        public static IServiceCollection AddNsqInboundMessaging(this IServiceCollection services, 
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

        public void AcceptMessage(Type type, string topic, Type handlerType, string channelName)
        {
            MessageTypeTopics.Add(type, topic);
            MessageTypeHandlerChannels.Add(handlerType, channelName);
        }
    }
}
