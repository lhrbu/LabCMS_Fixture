using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using LabCMS.Gateway.Shared.Extensions;
using LabCMS.EquipmentDomain.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using LabCMS.EquipmentDomain.Server.Services;
using LabCMS.ProjectDomain.Shared.Services;
using LabCMS.EquipmentDomain.Shared.Services;

namespace LabCMS.EquipmentDomain.Server
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
            services.AddControllers().AddJsonOptions(options=>
                options.JsonSerializerOptions.PropertyNamingPolicy=null);
            services.AddSingleton<ProjectsWebCacheService>();
            services.AddSingleton<EquipmentHourlyRatesLocalCacheService>();
            services.AddTransient<ReloadCacheService>();
            services.AddTransient<DynamicQueryService>();
            services.AddTransient<ExcelExportService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LabCMS.EquipmentDomain.Server", Version = "v1" });
            });
            services.AddDbContext<EquipmentHourlyRatesRepository>(options =>
                options.UseSqlite(Configuration.GetConnectionString(nameof(EquipmentHourlyRatesRepository))));
            services.AddDbContext<UsageRecordsRepository>(options =>
                options.UseSqlite(Configuration.GetConnectionString(nameof(UsageRecordsRepository ))));
            services.AddDbContext<UsageRecordsRecycleBin>(options =>
                options.UseSqlite(Configuration.GetConnectionString(nameof(UsageRecordsRecycleBin))));
            services.AddSingleton<ElasticSearchInteropService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LabCMS.EquipmentDomain.Server v1"));
            

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseConsulAsServiceProvider(nameof(EquipmentDomain));
        }
    }
}
