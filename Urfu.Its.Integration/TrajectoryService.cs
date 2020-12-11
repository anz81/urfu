using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class TrajectoryService
    {
        readonly string _restService = ConfigurationManager.AppSettings["UniRestAddress"];
        readonly string _user = ConfigurationManager.AppSettings["UniRestUser"];
        readonly string _password = ConfigurationManager.AppSettings["UniRestPassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<TrajectoryDto> GetTrajectories()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            using (var data = wc.OpenRead($"{_restService}/trajectory"))
            using (var streamReader = new StreamReader(data))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _jsonSerializer.Deserialize<List<TrajectoryDto>>(jsonTextReader);
            }
        }
    }
}