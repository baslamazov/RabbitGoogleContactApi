using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Threading.Tasks;
using GoogleAPI.Services;
using Microsoft.Extensions.Logging;

namespace GoogleAPI.Jobs
{
    public class CreateGoogleContactJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public CreateGoogleContactJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var RabbitMQService = scope.ServiceProvider.GetService<RabbitMQService>();
            var _logger = scope.ServiceProvider.GetService<ILogger<CreateGoogleContactJob>>();
            try
            {
                RabbitMQService.ServiceStart();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
