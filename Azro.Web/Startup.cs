using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azro.Core.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Azro.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var silo = new SiloHostBuilder()
                .UseLocalhostClustering()
                //.Configure<ClusterOptions>(options =>
                //{
                //    options.ClusterId = Configuration.GetSection("Orleans").GetValue<string>("ClusterId");
                //    options.ServiceId = Configuration.GetSection("Orleans").GetValue<string>("ServiceId");
                //})
                .AddMemoryGrainStorageAsDefault()
                .AddSimpleMessageStreamProvider("SMS-Provider")
                .EnableDirectClient()
                .ConfigureLogging(builder => builder.AddSerilog())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Game).Assembly).WithReferences())
                .Build();

            silo.StartAsync().Wait();

            services.AddSingleton(silo);

            var clusterClient = silo.Services.GetService<IClusterClient>();

            services.AddSingleton(clusterClient);
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            appLifetime.ApplicationStopping.Register(() =>
            {
                var silo = app.ApplicationServices.GetRequiredService<ISiloHost>();
                silo.StopAsync().Wait();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
