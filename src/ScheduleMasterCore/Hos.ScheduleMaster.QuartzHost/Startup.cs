using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.QuartzHost.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hos.ScheduleMaster.QuartzHost
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
            services.AddControllersWithViews(config =>
            {
                //���ȫ�ֹ�����
                config.Filters.Add(typeof(ApiValidationFilter));
                config.Filters.Add(typeof(GlobalExceptionFilter));
            });
            services.AddHealthChecks();

            services.AddDbContextPool<SmDbContext>(option =>
            option.UseMySql(Configuration.GetConnectionString("MysqlConnection"))
            );
            services.AddTransient<Core.Interface.IScheduleService, Core.Services.ScheduleService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime, ILogger<Startup> logger)
        {
            //var s = Environment.MachineName;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            ConfigurationCache.RootServiceProvider = app.ApplicationServices;
            //����ȫ�ֻ���
            ConfigurationCache.Reload();
            //��ʼ����־������
            Core.Log.LogManager.Init();
            //�ж��Ƿ�Ҫ�Զ����������ļ�ע��ڵ���Ϣ
            if (AppCommandResolver.IsAutoRegister())
            {
                logger.LogInformation("enabled auto register...");
                //���ýڵ���Ϣ
                ConfigurationCache.SetNode(Configuration);
                //��ʼ��Quartz
                Common.QuartzManager.InitScheduler().Wait();
            }

            appLifetime.ApplicationStopping.Register(OnStopping);
        }


        private void OnStopping()
        {
            // Perform on-stopping activities here
            Common.QuartzManager.Shutdown(true).Wait();
            Core.Log.LogManager.Shutdown();
        }
    }
}
