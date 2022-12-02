using System;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace GoogleAPI.Jobs.Shedulers
{
    public class GoogleContactSheduller
    {
        public static async void Start(IServiceProvider provider)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = provider.GetService<JobFactory>();


            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<CreateGoogleContactJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithIdentity("Contact", "Contact")
                .WithSimpleSchedule(x => x.WithIntervalInHours(3).RepeatForever())
                //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(5, 00))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
