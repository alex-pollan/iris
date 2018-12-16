using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Iris.Distributed;
using Iris.Distributed.Redis;
using Iris.Messaging;
using Iris.Api.Middleware;
using Iris.Messaging.Nsq;
using Iris.Api.Messaging;
using Iris.Logging;

namespace Iris.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IWebsocketsHandler<HelloMessage>, WebsocketsHandler<HelloMessage>>();
            services.AddSingleton(sp => sp.GetService<IWebsocketsHandler<HelloMessage>>() as IMessageDeliverer<HelloMessage>);
            services.AddSingleton<IInboundMessageQueue, InboundMessageQueue<HelloMessage>>();
            services.AddSingleton<INsqConfiguration, NsqConfiguration>();
            services.AddSingleton<IAppRedisConfiguration, AppRedisConfiguration>();
            services.AddSingleton<IInterprocessMessageBroadcaster<HelloMessage>, RedisInterprocessMessageBroadcaster<HelloMessage>>();
            services.AddSingleton<IInterprocessMessageReceiver, RedisInterprocessMessageReceiver<HelloMessage>>();
            services.AddSingleton<IInterprocessIdentity, MachineNameInterprocessIdentity>();
            services.AddSingleton<IMessageDispatcher<HelloMessage>, MessageDispatcher<HelloMessage>>();
            services.AddSingleton<IConnectionRequirement<HelloMessage>, CustomerSetConnectionRequirement<HelloMessage>>();
            services.AddSingleton<ILogger, Logger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                await next.Invoke();
                if (context.Response.StatusCode == 404 && !context.Request.Path.Value.Contains("/api"))
                {
                    context.Request.Path = new PathString("/index.html");
                    await next.Invoke();
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);

            app.UseMiddleware<WebsocketsMiddleware<HelloMessage>>();

            app.UseMvcWithDefaultRoute();

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseMvc();
        }
    }
}
