using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Entities;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.TokenAuthentication;

namespace Wd3w.AspNetCore.EasyTesting.SampleApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOptions<SampleOption>()
                .Bind(Configuration)
                .ValidateDataAnnotations()
                .ValidateOnStartupTime();
            services.AddScoped<ISampleService, SampleService>();
            services.AddScoped<SampleRepository>();
            services.AddDbContext<SampleDb>();

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "test-redis";
                options.Configuration = "host=127.0.0.1";
            });
            services.AddSingleton(new DbContextOptionsBuilder<SampleDb>()
                .UseNpgsql("Host=my_host;Database=my_db;Username=my_user;Password=my_pw")
                .Options);
            
            services.AddAuthentication("Bearer")
                .AddTokenAuthenticationScheme<CustomTokenAuthService>("Bearer", new TokenAuthenticationConfiguration
                {
                    Realm = "https://www.test.com/sign-in",
                    TokenLength = 20
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }

    public static class OptionsBuilderExtensions
    {
        public static OptionsBuilder<TOptions> ValidateOnStartupTime<TOptions>(this OptionsBuilder<TOptions> builder)
            where TOptions : class
        {
            builder.Services.AddTransient<IStartupFilter, OptionsValidateFilter<TOptions>>();
            return builder;
        }
        
        public class OptionsValidateFilter<TOptions> : IStartupFilter where TOptions : class
        {
            private readonly IOptions<TOptions> _options;

            public OptionsValidateFilter(IOptions<TOptions> options)
            {
                _options = options;
            }

            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                _ = _options.Value; // Trigger for validating options.
                return next;
            }
        }
    }

    public class SampleOption
    {
        [Required]
        public string NeedValue { get; set; }
    }
}