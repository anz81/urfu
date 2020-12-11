using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class ForeignLanguageRatingService
    {
        readonly string _restService = ConfigurationManager.AppSettings["ForeignLanguageRatingServiceAddress"];
        //readonly string _user = ConfigurationManager.AppSettings["UniRatingAvgServiceUser"];
        //private readonly string _password = ConfigurationManager.AppSettings["UniRatingAvgServicePassword"];

        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<ForeignLanguageRatingDtp> GetRating()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                //Credentials = new NetworkCredential(_user, _password)
            };
            lock (BrsService.GlobalLocker) // this is very very ugly, I know
            {
                var oldCallBack = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback = LksService.CertificateValidationCallBack;
                try
                {
                    using (var data = wc.OpenRead(_restService))
                    using (var streamReader = new StreamReader(data))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        return _jsonSerializer.Deserialize<List<ForeignLanguageRatingDtp>>(jsonTextReader);
                    }
                }
                finally
                {
                    if (ServicePointManager.ServerCertificateValidationCallback ==
                        LksService.CertificateValidationCallBack)
                        ServicePointManager.ServerCertificateValidationCallback = oldCallBack;
                }
            }
        }
    }
}