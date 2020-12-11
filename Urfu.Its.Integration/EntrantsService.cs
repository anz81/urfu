using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class EntrantsService
    {
        readonly string _restService = ConfigurationManager.AppSettings["EntrantsServiceAddress"];
        readonly string _user = ConfigurationManager.AppSettings["EntrantsServiceUser"];
        readonly string _password = ConfigurationManager.AppSettings["EntrantsServicePassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<EntrantRatingDto> GetEntrantsRating()
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
                return _jsonSerializer.Deserialize<List<EntrantRatingDto>>(jsonTextReader);
            }
        }
    }
}