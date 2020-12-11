using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using CsvHelper;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class UniModulesService
    {
        readonly string _restService;
        readonly string _user;
        readonly string _password;
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public UniModulesService(string restService, string user, string password)
        {
            _restService = restService;
            _user = user;
            _password = password;
        }

        public static UniModulesService Create()
        {
            return new UniModulesService(
                    ConfigurationManager.AppSettings["UniRestModulesAddress"],
                    ConfigurationManager.AppSettings["UniRestModulesUser"],
                    ConfigurationManager.AppSettings["UniRestModulesPassword"]
                );
        }

        public List<ModuleDto> GetModulesForDirection(string directionCode)
        {
            return ReadData<ModuleDto>(_restService + "?speciality=" + directionCode);
        }

        public List<PlanDto> GetPlansForDirection(string directionCode)
        {
            return ReadData<PlanDto>(_restService + "/planinfo?speciality=" + directionCode);
        }

        public List<PlanTermsDto> GetPlanTerms()
        {
            return ReadData<PlanTermsDto>(_restService + "/planyearterminfo");
        }

        private List<T> ReadData<T>(string address)
        {
            using (var data = OpenRead(address))
            using (var streamReader = new StreamReader(data))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _jsonSerializer.Deserialize<List<T>>(jsonTextReader);
            }
        }

        protected virtual Stream OpenRead(string address)
        {
            var wc = new UniModulesServiceWebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            return wc.OpenRead(address);
        }

        public class UniModulesServiceWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 20 * 60 * 1000;
                return w;
            }
        }
    }



}