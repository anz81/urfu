using System;
using Urfu.Its.Common;

namespace Urfu.Its.Web.Schedule
{
    class HeartBeat : ScheduledJob
    {
        public override bool Enabled
        {
            get { return true; }
        }

        public HeartBeat()
        {
            LastRun = DateTime.Now;
            Period = TimeSpan.FromMinutes(15);
            Logger.Info("Старт обработчика плановых задач");
        }

        protected override void Work()
        {
            Logger.Info("Тик");
        }
    }
}