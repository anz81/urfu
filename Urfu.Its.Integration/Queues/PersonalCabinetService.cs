using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.MqModel;

namespace Urfu.Its.Integration.Queues
{
    public static class PersonalCabinetService
    {
        private static readonly QueuePublisher Publisher = CreateQueuePublisher();

        private static QueuePublisher CreateQueuePublisher()
        {
            return new QueuePublisher
            {
                Exchange = ConfigurationManager.AppSettings["RabbitMQExchange"],
                Factory = new ConnectionFactory
                {
                    HostName = ConfigurationManager.AppSettings["RabbitMQServer"],
                    UserName = ConfigurationManager.AppSettings["RabbitMQLogin"],
                    Password = ConfigurationManager.AppSettings["RabbitMQPassword"],
                    Port = GetInt("RabbitMQPort", 7070)
                }
            };
        }

        static readonly string RabbitMQQueueName = ConfigurationManager.AppSettings["RabbitMQVariantsQueueName"];
        static readonly bool RabbitMQNotification = IsFlagEnabled("RabbitMQNotification");
        static readonly bool DisableVariantsQueue = IsFlagEnabled("DisableVariantsQueue");
        static readonly bool DisableProgramsQueue = IsFlagEnabled("DisableProgramsQueue");


        private static int GetInt(string setting, int defaultValue)
        {
            int value;
            return int.TryParse(ConfigurationManager.AppSettings[setting], out value) ? value : defaultValue;
        }

        private static bool IsFlagEnabled(string flag)
        {
            return "true".Equals(ConfigurationManager.AppSettings[flag], StringComparison.InvariantCultureIgnoreCase);
        }

        static readonly string RabbitMQAdmissionsQueueName = ConfigurationManager.AppSettings["RabbitMQAdmissionsQueueName"];
        static readonly string RabbitMQRunpModuleAdmissionsRoutingKey = ConfigurationManager.AppSettings["RabbitMQRunpModuleAdmissionsRoutingKey"];
        static readonly string RabbitMQVariantRoutingKey = ConfigurationManager.AppSettings["RabbitMQVariantsRoutingKey"];
        static readonly string RabbitMQRunpAdmissionsRoutingKey = ConfigurationManager.AppSettings["RabbitMQRunpAdmissionsRoutingKey"];
        static readonly string RabbitMQProgramsQueueName = ConfigurationManager.AppSettings["RabbitMQProgramsQueueName"];
        static readonly string RabbitMQProgramsRoutingKey = ConfigurationManager.AppSettings["RabbitMQProgramsRoutingKey"];
        static readonly string RabbitMQAdmissionRoutingKey = ConfigurationManager.AppSettings["RabbitMQAdmissionRoutingKey"];
        private static readonly string RabbitMQPracticeAdmissionRoutingKey =
            ConfigurationManager.AppSettings["RabbitMQPracticeAdmissionRoutingKey"];
        static readonly string RabbitMQExchange = ConfigurationManager.AppSettings["RabbitMQExchange"];
        private static readonly string RabbitMqSubgroupMembershipRoutingKey = ConfigurationManager.AppSettings["RabbitMQSubgroupMembershipRoutingKey"];


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void PostVariants(IEnumerable<VariantApiDto> variants)
        {
            if (DisableVariantsQueue)
                return;

            PostData(variants, RabbitMQVariantRoutingKey, null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void PostAdmissionsToRunp(IEnumerable<RunpAdmissionDto> variants)
        {
            PostData(variants, RabbitMQRunpAdmissionsRoutingKey, null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void PostPrograms(IEnumerable<ProgramApiDto> programs)
        {
            if (DisableProgramsQueue)
                return;

            PostData(programs, RabbitMQProgramsRoutingKey, null);
        }

        public static void PostVariant(VariantApiDto variant)
        {
            PostVariants(new[]{variant});
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void PostAdmission(IEnumerable<StudentAdmissionDto> variants)
        {
            if (string.IsNullOrEmpty(RabbitMQAdmissionsQueueName))
                return;

            PostData(variants, RabbitMQAdmissionRoutingKey, null);
        }


        public static void PostModuleAdmission(List<SectionFKAdmissionDto>[] dto)
        {
            if (string.IsNullOrEmpty(RabbitMQRunpModuleAdmissionsRoutingKey))
                return;

            PostData(dto, RabbitMQRunpModuleAdmissionsRoutingKey, null);
        }

        // Публикация изменений состава подгруппы/миноргруппы
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void PostSubgroupMember(IEnumerable<object> members, bool include)
        //public static void PostSubgroupMember(IEnumerable<SubgroupMemberMqDto> members, bool include)
        {
            IDictionary<string, object> headers = new Dictionary<string, object>
            {
                { "include", include }
            };
            PostData(new[] { members }, RabbitMqSubgroupMembershipRoutingKey, headers);
        }

        public static void PostPracticeAdmission(PracticeAdmissionMqDto practiceAdmission)
        {
            if (string.IsNullOrEmpty(RabbitMQPracticeAdmissionRoutingKey))
                return;
            PostData(new[] { practiceAdmission }, RabbitMQPracticeAdmissionRoutingKey, null);
        }
        private static void PostData(IEnumerable<object> objs, string routingKey, IDictionary<string, object> headers)
        {
            if (RabbitMQNotification)
            {
                Publisher.PostData(objs, routingKey, headers);
            }
        }

    }
}
