using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using Dlubal.WS.RfemRstabSoapWsCoreLib;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2
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
            services.AddRazorPages();

            services.AddOptions();
            services.Configure<MyConfig>(Configuration.GetSection("WebAppConfig"));
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time   
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=webservicemanager}/{action=Index}/{id?}");
            });

            new MyConfig(Configuration); // load config

            // DataLogger setting and run
            DataLogger dataLogger = DataLogger.DummyDataLogger;
            DataLogger.InitializeNewDataLogger("DotNetClientTest", MyConfig.LogPath, true, out dataLogger);
            Session.DataLogger = dataLogger;
        }
    }
}
