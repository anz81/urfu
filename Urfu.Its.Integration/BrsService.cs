using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class BrsService
    {
        private static readonly XmlSerializer DirectionSerializer = new XmlSerializer(typeof (RatingDto));

        private readonly string _restService = ConfigurationManager.AppSettings["BrsServiceAddress"];
        private readonly string _user = ConfigurationManager.AppSettings["BrsServiceUser"];
        private readonly string _password = ConfigurationManager.AppSettings["BrsServicePassword"];
        private readonly string _loginUrl = ConfigurationManager.AppSettings["BrsServiceLoginUrl"];

        public static readonly object GlobalLocker = new object();

        public List<StudentRatingDto> GetRatings(int year, int classNumber, int term, bool withCoefficients)
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
                lock (GlobalLocker) // this is very very ugly, I know
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
                            .Replace("{Year}", year.ToString())
                            .Replace("{Class}", classNumber.ToString())
                            .Replace("{WithCoefficients}", Convert.ToInt32(withCoefficients).ToString());
#if DEBUG
                        //responsebody = wc.DownloadString(url);
#endif


                        using (var data = wc.OpenRead(url))
                        {
                            var ratings = ((RatingDto) DirectionSerializer.Deserialize(data)).data.list;
                            foreach (var dto in ratings)
                            {
                                dto.id = dto.id.Substring(0, 32);
                            }
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

    public class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; private set; }

        /// <summary>
        /// This will instanciate an internal CookieContainer.
        /// </summary>
        public CookieWebClient()
        {
            this.CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Use this if you want to control the CookieContainer outside this class.
        /// </summary>
        public CookieWebClient(CookieContainer cookieContainer)
        {
            this.CookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            request.Timeout = 1000*1000;
            return request;
        }
    }
}