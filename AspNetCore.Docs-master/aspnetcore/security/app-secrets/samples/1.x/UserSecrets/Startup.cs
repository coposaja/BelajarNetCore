﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserSecrets
{
    #region snippet_StartupClass
    public class Startup
    {
        private string _moviesApiKey = null;
        
        #region snippet_StartupConstructor
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", 
                             optional: false, 
                             reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }
        #endregion snippet_StartupConstructor

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            _moviesApiKey = Configuration["Movies:ServiceApiKey"];
        }

        public void Configure(IApplicationBuilder app)
        {
            var result = string.IsNullOrEmpty(_moviesApiKey) ? "Null" : "Not Null";
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync($"Secret is {result}");
            });
        }
    }
    #endregion snippet_StartupClass
}
