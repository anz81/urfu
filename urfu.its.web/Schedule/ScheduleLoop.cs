using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Urfu.Its.Common;

namespace Urfu.Its.Web.Schedule
{
    public class ScheduleLoop
    {
        public static void StartLoop()
        {
            Jobs.Add(new TeacherScan());
            Jobs.Add(new HeartBeat());
            Jobs.Add(new NightResync());
            HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>) MainLoop);
        }

        static readonly List<ScheduledJob> Jobs = new List<ScheduledJob>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void MainLoop(CancellationToken cancellationToken)
        {
            foreach (var task in Jobs)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                ProbeTask(task);
            }
            Thread.Sleep(1000);
            if (cancellationToken.IsCancellationRequested)
                return;
            foreach (var job in Jobs.ToList())
            {
                if (job.FailCount > 10)
                {
                    Logger.Error("Задача " + job + " убрана из списка выполняемых изза превышения количества допустимых ошибок");
                    Jobs.Remove(job);
                }
            }
            HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>) MainLoop);
        }

        private static void ProbeTask(ScheduledJob task)
        {
            if(task.Enabled)
                if (DateTime.Now - task.LastRun > task.Period)
                {
                    try
                    {
                        task.DoWork();
                    }
                    catch (Exception ex)
                    {
                        task.FailCount++;
                        Logger.Error(ex);
                    }
                }
        }
    }
}