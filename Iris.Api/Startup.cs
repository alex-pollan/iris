using Iris.Api.Messaging;
using Iris.Api.Middleware;
using Iris.Distributed;
using Iris.Distributed.Redis;
using Iris.Logging;
using Iris.Messaging;
using Iris.Messaging.Nsq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

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

            services.AddSingleton<IWebsocketsHandler, WebsocketsHandler>();
            services.AddSingleton(sp => sp.GetService<IWebsocketsHandler>() as IMessageDeliverer);

            services.AddNsqInboundMessaging(options =>
            {
                options.UseEndpoints(Configuration["vcap:services:user-provided:0:credentials:lookup"]);
                options.AcceptMessage(
                    typeof(HelloMessage), "HelloMessage",
                    typeof(NsqHandler<HelloMessage>), "HelloMessage-Channel"
                );
            });

            services.AddSingleton<IAppRedisConfiguration, AppRedisConfiguration>();
            services.AddSingleton<IInterprocessMessageBroadcaster, RedisInterprocessMessageBroadcaster>();
            services.AddSingleton<IInterprocessMessageReceiver, RedisInterprocessMessageReceiver>();
            services.AddSingleton<IInterprocessIdentity, MachineNameInterprocessIdentity>();
            services.AddSingleton<IMessageDispatcher, MessageDispatcher>();
            services.AddSingleton<IConnectionRequirement, CustomerSetConnectionRequirement>();
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

            app.UseMiddleware<WebsocketsMiddleware>();

            app.UseMvcWithDefaultRoute();

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseMvc();
        }
    }
}
