using Iris.Api.Middleware;
using Iris.Distributed;
using Iris.Distributed.Redis;
using Iris.Logging;
using Iris.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris.NetCore.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddIris(this IServiceCollection services, 
            Action<IrisStartupOptions> options) 
        {
            var startupOptions = new IrisStartupOptions();

            options(startupOptions);

            if (!startupOptions.IsValid(out string errorMesssage))
            {
                throw new ArgumentException(errorMesssage);
            }

            services.AddSingleton<IWebsocketsHandler, WebsocketsHandler>();
            services.AddSingleton(sp => sp.GetService<IWebsocketsHandler>() as IMessageDeliverer);
            services.AddSingleton<IAppRedisConfiguration, AppRedisConfiguration>();
            services.AddSingleton<IInterprocessMessageBroadcaster, RedisInterprocessMessageBroadcaster>();
            services.AddSingleton<IInterprocessMessageReceiver, RedisInterprocessMessageReceiver>(provider=> 
            {
                var receiver = new RedisInterprocessMessageReceiver(
                    provider.GetService<IAppRedisConfiguration>(),
                    provider.GetService<IInterprocessIdentity>(),
                    provider.GetService<IMessageDeliverer>(),
                    provider.GetService<ILogger>());

                foreach (var messageType in startupOptions.InterprocessMessageReceiverMessageTypes)
                {
                    receiver.RegisterMessageType(messageType);
                }                

                return receiver;
            });
            services.AddSingleton<IInterprocessIdentity, MachineNameInterprocessIdentity>();
            services.AddSingleton<IMessageDispatcher, MessageDispatcher>();
            services.AddSingleton(typeof(IConnectionRequirement), startupOptions.ConnectionRequirementType);
            services.AddSingleton<ILogger, Logger>();

            return services;
        }

        public static IApplicationBuilder UseIrisMiddleware(this IApplicationBuilder app, 
            WebSocketOptions webSocketOptions = null)
        {
            if (webSocketOptions == null)
            {
                webSocketOptions = new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(120),
                    ReceiveBufferSize = 4 * 1024
                };
            }

            app.UseWebSockets(webSocketOptions);

            app.UseMiddleware<WebsocketsMiddleware>();

            return app;
        }
    }

    public class IrisStartupOptions
    {
        internal Type ConnectionRequirementType;
        internal List<Type> InterprocessMessageReceiverMessageTypes = new List<Type>();

        internal IrisStartupOptions() { }

        public IrisStartupOptions UseConnectionRequirement(Type type)
        {
            ConnectionRequirementType = type;

            if (!IsConnectionRequirementValid())
            {
                throw new ArgumentException($"Expected a class implementing {nameof(IConnectionRequirement)}");
            }

            return this;
        }

        public IrisStartupOptions AcceptMessage(Type messageType)
        {
            InterprocessMessageReceiverMessageTypes.Add(messageType);

            return this;
        }

        internal bool IsValid(out string errorMessage)
        {
            if (!IsConnectionRequirementValid())
            {
                errorMessage = $"Expected a class implementing {nameof(IConnectionRequirement)}";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private bool IsConnectionRequirementValid()
        {
            return ConnectionRequirementType.IsClass 
                && ConnectionRequirementType.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IConnectionRequirement)));
        }
    }
}
