using System;
using Hacka.Domain;
using Hacka.Infra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Hacka.Api.BackgroundServices;
using Hacka.Domain.Abstractions;

namespace Hacka.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HackaContext>(opt => opt.UseInMemoryDatabase("HackaDb"), ServiceLifetime.Singleton);
            services.AddControllers();

            services.AddSingleton<IEventZabbixRepository, EventZabbixRepository>();
            services.AddSingleton<ISquadRepository, SquadRepository>();
            services.AddSingleton<IZabbixRepository, ZabbixRepository>();
            services.AddSingleton<IMsTeamsRepository, MsTeamsRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hacka.Api", Version = "v1" });
            });
            
            services.AddSwaggerGen();

            DataSeed.InitializeAsync(services.BuildServiceProvider()).Wait();
            services.AddHostedService<VerifyAlertsBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hacka API V1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
