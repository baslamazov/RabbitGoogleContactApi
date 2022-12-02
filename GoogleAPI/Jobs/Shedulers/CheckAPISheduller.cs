using System;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace GoogleAPI.Jobs.Shedulers
{
    public class CheckAPISheduller
    {
        public static async void Start(IServiceProvider provider)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = provider.GetService<JobFactory>();


            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<CheckAPIJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithIdentity("Check", "Check")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
