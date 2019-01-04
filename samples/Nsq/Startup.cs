using Iris.Api.Middleware;
using Iris.Messaging;
using Iris.Messaging.Nsq.Extensions;
using Iris.NetCore.Extensions;
using Iris.Samples.Nsq.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Iris.Samples.Nsq
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

            services.AddIris(options =>
            {
                options.UseConnectionRequirement(typeof(CustomerSetConnectionRequirement));
                options.AcceptMessage(typeof(HelloMessage));
            });

            services.AddIrisNsqInboundMessaging(options =>
            {
                options.UseEndpoints(Configuration["vcap:services:user-provided:0:credentials:lookup"]);
                options.AcceptMessage<HelloMessage>("HelloMessage", "HelloMessage-Channel");
            });
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

            app.UseIrisMiddleware();

            app.UseMvcWithDefaultRoute();

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseMvc();
        }
    }
}
