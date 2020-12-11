using System;
using System.Configuration;

namespace Urfu.Its.Web.Schedule
{
    abstract class ScheduledJob
    {
        protected ScheduledJob()
        {
            LastRun = DateTime.Now.AddMinutes(-1);
            Period = TimeSpan.FromMinutes(1);
            _settingsKey = ConfigurationManager.AppSettings[GetType().Name];
        }

        public TimeSpan Period { get; set; }
        public DateTime LastRun { get; set; }
        public int FailCount { get; set; }


        readonly string _settingsKey;

        public virtual bool Enabled
        {
            get
            {
                return "true".Equals(_settingsKey, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        protected abstract void Work();

        public void DoWork()
        {
            Work();
            LastRun = DateTime.Now;
        }
    }
}