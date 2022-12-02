using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using Repository;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAPI.Jobs
{
    public class CheckAPIJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public CheckAPIJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var _logger = scope.ServiceProvider.GetService<ILogger<CheckAPIJob>>();
            var UnitOfWork = scope.ServiceProvider.GetService<UnitOfWork>();

            try
            {
                var firstItem = UnitOfWork.GoogleContacts.GetWhereAsync(c => true, 0, 1);
                _logger.LogInformation($"Запрос в api первого элемента (табельный номер): {firstItem.Result.ToList()[0].TABNUM}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
