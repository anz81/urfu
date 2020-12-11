using System.Collections.Generic;
using Urfu.Its.Common;
using Urfu.Its.Integration;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Schedule
{
    class TeacherScan : ScheduledJob
    {
        protected override void Work()
        {
            foreach (var message in new TeacherService().GetTeachersFromQueue())
            {
                Logger.Info($"Пришло сообщение с преподавателем {message?.lastName} {message?.firstName} {message?.middleName} pkey: {message?.pkey}");
                SyncEngine.WriteTeachersToDb(new[]{message});
                Logger.Info($"Сохранен преподаватель {message?.lastName} {message?.firstName} {message?.middleName} pkey: {message?.pkey}");
            }
        }
    }
}