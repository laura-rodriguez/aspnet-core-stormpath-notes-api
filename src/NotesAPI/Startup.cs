using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stormpath.AspNetCore;
using Stormpath.Configuration.Abstractions;
using NotesAPI.Services;
using Newtonsoft.Json.Serialization;

namespace NotesAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStormpath(new StormpathConfiguration()
            {
                Web = new WebConfiguration()
                {
                    // This explicitly tells the Stormpath middleware to only serve JSON responses (appropriate for an API).
                    // By default, HTML responses are served too.
                    Produces = new[] { "application/json" },
                    Oauth2 = new WebOauth2RouteConfiguration()
                    {
                        Uri = "/token",
                        Password = new WebOauth2PasswordGrantConfiguration()
                        {
                            ValidationStrategy = WebOauth2TokenValidationStrategy.Stormpath
                        }
                    }
                }
            });

            services.AddTransient<AccountService>();

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStormpath();
            app.UseMvc();
        }
    }
}
