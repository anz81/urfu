using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class UniPlanTermsService
    {
        readonly string _restService = ConfigurationManager.AppSettings["UniRestModulesAddress"];
        readonly string _user = ConfigurationManager.AppSettings["UniRestModulesUser"];
        readonly string _password = ConfigurationManager.AppSettings["UniRestModulesPassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<PlanTermsDto> GetPlanTerms()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            using (var data = wc.OpenRead(_restService + "/planyearterminfo"))
            using (var streamReader = new StreamReader(data))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _jsonSerializer.Deserialize<List<PlanTermsDto>>(jsonTextReader);
            }
        }

        /// <summary>
        /// Пример ответа: 
        /// {
        ///     "eduplanUUID": "unplan18ggl5g0000kfks7l98g8ai29s",
        ///     "WeeksCount": [
        ///     {
        ///         "Term": 1,
        ///         "WeeksCount": 18
        ///     },
        ///     {
        ///         "Term": 2,
        ///         "WeeksCount": 18
        ///     },
        ///     {
        ///         "Term": 3,
        ///         "WeeksCount": 9
        ///     },
        ///     {
        ///         "Term": 4,
        ///         "WeeksCount": 18
        ///     }]
        /// }
        /// </summary>
        /// <returns></returns>
        public List<PlanTermsWeeksDto> GetPlanTermsWeeks()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };

            using (var data = wc.OpenRead(_restService + "/planweekinfo"))
            using (var streamReader = new StreamReader(data))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _jsonSerializer.Deserialize<List<PlanTermsWeeksDto>>(jsonTextReader);
            }
        }
    }
}
