using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class UniDivisionsService
    {
        readonly string _restService = ConfigurationManager.AppSettings["UniDivisionsServiceAddress"];
        readonly string _user = ConfigurationManager.AppSettings["UniDivisionsServiceUser"];
        readonly string _password = ConfigurationManager.AppSettings["UniDivisionsServicePassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public Dictionary<string, DivisionDto> GetDivisions()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            using (var data = wc.OpenRead(_restService))
            using (var streamReader = new StreamReader(data))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _jsonSerializer.Deserialize<List<DivisionDto>>(jsonTextReader).ToDictionary(d => d.uuid);
            }
        }
    }
}