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
    public class DebtorsService
    {

        readonly string _restService = ConfigurationManager.AppSettings["DebtorsServiceAddress"];
        readonly string _user = ConfigurationManager.AppSettings["DebtorsServiceUser"];
        readonly string _password = ConfigurationManager.AppSettings["DebtorsServicePassword"];
        private readonly string _loginUrl = ConfigurationManager.AppSettings["DebtorsServiceLoginUrl"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<DebtorDto> GetDebtors(string moduleTitle, int? year, string term)
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
                            .Replace("{moduleTitle}", moduleTitle);
                        if (!string.IsNullOrEmpty(term)) url += $"&amp;{term}";
                        if (year != null) url += $"&amp;{year.ToString()}";
                        //.Replace("{Term}", term)
                        //.Replace("{Year}", year != null ? year.ToString() : "");

#if DEBUG
                        //responsebody = wc.DownloadString(url);
#endif

                        wc.Headers.Add("Accept", "application/json");
                        using (var data = wc.OpenRead(url))
                        using (var streamReader = new StreamReader(data))
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            var ratings = (_jsonSerializer.Deserialize<List<DebtorDto>>(jsonTextReader));
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