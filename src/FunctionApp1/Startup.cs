using System;
using FunctionApp1.Application;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 

[assembly: FunctionsStartup(typeof(FunctionApp1.Startup))]
namespace FunctionApp1
{
    public class Startup : FunctionsStartup
    {
        // add default parameterless constructor
        public Startup() { }

        // would be ideal to use this constructor like ASP.NET Core does if was supported, possibly future
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // update configuration using extension method, in future when more support
            // added for this extension method can go away entirely 
            // create a Func that will receive an IConfigurationBuilder as input and expects
            // IConfiguration as output by calling .Build() on the IConfigurationBuilder
            builder.AddConfiguration((configBuilder) =>
            {
                // these could come from other sources than environment variables, keeping simple for now
                var envName = Environment.GetEnvironmentVariable("ENVIRONMENT_NAME") ?? "Production";

                // use IConfigurationBuilder like you typically would
                var configuration = configBuilder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{envName}.json", true, true)
                    .TryAddAzureKeyVault(Environment.GetEnvironmentVariable("VAULT_NAME"))
                    .AddEnvironmentVariables()
                    .Build();
                
                return configuration;
            });

            // set the IConfiguration, in future maybe this will be set in constructor like ASP.NET Core is
            // be sure to call this after AddConfiguration has been called
            Configuration = builder.GetCurrentConfiguration();
           
            // explicitly call ConfigureServices to setup DI 
            ConfigureServices(builder.Services);
        }
         
        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // add in options 
            services.Configure<CustomConfig>(options =>
                Configuration.GetSection("CustomConfig")
                    .Bind(options));

            // add in services 
            services.AddTransient<IMyService, MyService>(); 
        }

    }
}
