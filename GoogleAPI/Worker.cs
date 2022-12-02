using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GoogleAPI.Services;
using GoogleAPI.Jobs.Shedulers;
//using GoogleAPI.Jobs.Shedulers;

namespace GoogleAPI
{
    public class Worker : BackgroundService
    {
        private ILogger<Worker> _logger { get; }
        public UnitOfWork UnitOfWork { get; set; }
        public GoogleContactsService GoogleContactsService { get; set; }
        public RabbitMQService RabbitMQService { get; set; }
        public GoogleContactsDbService GoogleContactsDbService { get; set; }
        public IServiceProvider Provider { get; }

        public Worker(IServiceProvider provider)
        {
            UnitOfWork = provider.GetService<UnitOfWork>();
            GoogleContactsService = provider.GetService<GoogleContactsService>();
            RabbitMQService = provider.GetService<RabbitMQService>();
            GoogleContactsDbService = provider.GetService<GoogleContactsDbService>();
            _logger = provider.GetService<ILogger<Worker>>();
            Provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker запущен в: {time}", DateTimeOffset.Now);

            stoppingToken.ThrowIfCancellationRequested();
            try
            {
                // Создание контакта
                GoogleContactSheduller.Start(Provider);
                // Пендаль для АПИ
                CheckAPISheduller.Start(Provider);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Где-то плачет маленький карлик");
                _logger.LogError(ex.StackTrace);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
