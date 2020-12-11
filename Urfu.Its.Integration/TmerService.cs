using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Urfu.Its.Integration;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Web.Models
{
    //Этот класс находится тут, потому что его невозможно абстрагировать в проект Integration
    public class TmerService
    {
        readonly string _restService = ConfigurationManager.AppSettings["TmerServiceAddress"];
        readonly string _user = ConfigurationManager.AppSettings["TmerServiceUser"];
        readonly string _password = ConfigurationManager.AppSettings["TmerServicePassword"];
        private readonly string _loginUrl = ConfigurationManager.AppSettings["TmerServiceLoginUrl"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<TmerDto> GetTmers()
        {
            using (var wc = new CookieWebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password),
            })
            {
                var reqparm = new NameValueCollection
                {
                    {"username", _user},
                    {"password", _password}
                };

                lock (BrsService.GlobalLocker) // this is very very ugly, I know
                {
                    var oldCallBack = ServicePointManager.ServerCertificateValidationCallback;
                    ServicePointManager.ServerCertificateValidationCallback = LksService.CertificateValidationCallBack;

                    try
                    {
                        var values = wc.UploadValues(_loginUrl, "POST", reqparm);

                        wc.Headers.Add("Accept", "application/json");
                        using (var data = wc.OpenRead(_restService))
                        using (var streamReader = new StreamReader(data))
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            var tmers = _jsonSerializer.Deserialize<List<TmerDto>>(jsonTextReader);
                            return tmers;
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
}