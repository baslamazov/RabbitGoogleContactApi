using Contracts;
using Entities;
using GoogleAPI.Jobs;
using GoogleAPI.Services;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostAPIBuilder(args).Build().Run();

        }


        public static IHostBuilder CreateHostAPIBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {

                    services.Configure<Settings>(hostContext.Configuration);

                    var env = hostContext.HostingEnvironment;

                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();

                    services.AddSingleton<IDbContextBuilder, DataContextBuilder>();
                    services.AddSingleton<GoogleContactsService>();
                    services.AddSingleton<GoogleContactsDbService>();
                    services.AddSingleton<RabbitMQService>();

                    services.AddTransient<JobFactory>();
                    services.AddScoped<CreateGoogleContactJob>();
                    services.AddScoped<CheckAPIJob>();


                    services.AddTransient<IGoogleContactsRepository, GoogleContactsRepository>();
                    services.AddTransient<IBankRepository, BankRepository>();
                    services.AddTransient<UnitOfWork>();

                    services.AddHostedService<Worker>();
                });
    }
}
