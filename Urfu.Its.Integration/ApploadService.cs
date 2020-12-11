using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class ApploadService
    {

        readonly string _restService = ConfigurationManager.AppSettings["ApploadServiceAddress"];
        readonly string _user = ConfigurationManager.AppSettings["ApploadServiceUser"];
        readonly string _password = ConfigurationManager.AppSettings["ApploadServicePassword"];
        private readonly string _loginUrl = ConfigurationManager.AppSettings["ApploadServiceLoginUrl"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<ApploadDto> GetApploads(int year, int term,bool onlyIts)
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
#if DEBUG
                        string responsebody = Encoding.UTF8.GetString(values);
#endif


                        var url = _restService
                            .Replace("{Term}", term.ToString())
                            .Replace("{Year}", year.ToString());
                        if (onlyIts)
                            url += "&its=true";
#if DEBUG
                            //responsebody = wc.DownloadString(url);
#endif

                        wc.Headers.Add("Accept", "application/json");
                        using (var data = wc.OpenRead(url))
                        using (var streamReader = new StreamReader(data))
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            var ratings = (_jsonSerializer.Deserialize<List<ApploadDto>>(jsonTextReader));
                            return ratings;
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