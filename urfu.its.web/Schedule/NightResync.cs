using System;
using System.Configuration;
using System.Threading;
using Urfu.Its.Common;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Schedule
{
    class NightResync : ScheduledJob
    {
        private bool _armed;
        private readonly bool _configured;

        private TimeSpan _launchTime;

        public NightResync()
        {
            var appSetting = ConfigurationManager.AppSettings["NightResyncTime"];
            DateTime time;
            if (DateTime.TryParse(appSetting, out time))
            {
                _launchTime = time.TimeOfDay;
                _configured = true;
            }
        }

        protected override void Work()
        {
            if (!_configured)
                return;

            var now = DateTime.Now.TimeOfDay;
            if (_launchTime > now && _launchTime.Add(TimeSpan.FromHours(-1)) < now) //arming 1hr prior to launch
                _armed = true;
            else if (_armed && _launchTime < now)
                Resync();
        }

        private void Resync()
        {
            _armed = false;

            WrapWithTryCatch(SyncEngine.SyncDirections, "Направления");
            WrapWithTryCatch(() => SyncEngine.SyncPeople(CancellationToken.None), "Студенты");
            WrapWithTryCatch(() => SyncEngine.SyncModules(CancellationToken.None), "Модули");
            
            WrapWithTryCatch(() => SyncEngine.SyncStudentPlan(), "Планы студентов");
            int monthCorrection = 0;
            if (DateTime.Now.Month < 7)
                monthCorrection++;
            var currentEduYear = DateTime.Now.Year - monthCorrection;
            var lastEduYear = currentEduYear - 1;

            WrapWithTryCatch(() => SyncEngine.SyncApploads(currentEduYear, 1), $"нагрузки {currentEduYear} года, 1 семестра");
            WrapWithTryCatch(() => SyncEngine.SyncApploads(currentEduYear, 2), $"нагрузки {currentEduYear} года, 2 семестра");
            WrapWithTryCatch(() => SyncEngine.SyncApploads(lastEduYear, 1), $"нагрузки {lastEduYear} года, 1 семестра");
            WrapWithTryCatch(() => SyncEngine.SyncApploads(lastEduYear, 2), $"нагрузки {lastEduYear} года, 2 семестра");
            WrapWithTryCatch(() => SyncEngine.SyncDebtors("Физическая культура и спорт",null,null), "должников Физическая культура и спорт");
            WrapWithTryCatch(() => SyncEngine.SyncForeignLanguageRating(), "Рейтинг ИЯ");
            WrapWithTryCatch(() => SyncEngine.SyncGroupHistory(currentEduYear), $"Исторические группы года {currentEduYear}");
            WrapWithTryCatch(() => SyncEngine.SyncGroupHistory(lastEduYear), $"Исторические группы года {lastEduYear}");
            WrapWithTryCatch(() => SyncEngine.SyncModuleAgreements(), "Соглашения");
            WrapWithTryCatch(() => SyncEngine.SyncROPs(), "РОПы");
            WrapWithTryCatch(() => SyncEngine.SyncTrajectories(), "Траектории");
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                WrapWithTryCatch(() => SyncEngine.SyncTmers(), "Контрольные значения");
        }

        private void WrapWithTryCatch(Action action, string task)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.Error("Синхронизация " + task + " выполнена с ошибкой");
                Logger.Error(ex);
            }
        }
    }
}