using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Urfu.Its.Integration
{
    public class TechCardService
    {
        readonly string _restService = ConfigurationManager.AppSettings["TechCardServiceUrl"];
        readonly string _user = ConfigurationManager.AppSettings["TechCardServiceLogin"];
        readonly string _password = ConfigurationManager.AppSettings["TechCardServicePassword"];
        private readonly string _loginUrl = ConfigurationManager.AppSettings["TechCardServiceLoginUrl"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public IEnumerable<TechCardDto> GetTechCards(string year, string termType, string disciplineName, string groupId)
        {
            var reqparm = new NameValueCollection
            {
                {"username", _user},
                {"password", _password}
            };

            using (var wc = new MyCookieWebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password),
                Validate = false
            })
            {
                var values = wc.UploadValues(_loginUrl, "POST", reqparm);
#if DEBUG
                string responsebody = Encoding.UTF8.GetString(values);
#endif

                var url = _restService
                    .Replace("{YEAR}", year)
                    .Replace("{TERM_TYPE}", termType)
                    .Replace("{DISCIPLINE}", disciplineName)
                    .Replace("{GROUP_UUID}", groupId);

                wc.Headers.Add("Accept", "application/json");
                using (var data = wc.OpenRead(url))
                using (var streamReader = new StreamReader(data))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var techCards = (_jsonSerializer.Deserialize<List<TechCardDto>>(jsonTextReader));
                    return techCards;
                }                
            }
        }

        class MyCookieWebClient : CookieWebClient
        {

            public MyCookieWebClient()
            {
                Validate = false;
            }

            public bool Validate { get; set; } // yes/no

            protected override WebRequest GetWebRequest(Uri url)
            {
                // this is called for each DownloadXXXX call
                var req = (HttpWebRequest)base.GetWebRequest(url);

                // if Validate is true 
                if (!Validate)
                {
                    // set the callback on this request
                    req.ServerCertificateValidationCallback = (s, cert, chain, polErr) => true;
                }
                return req;
            }
        }
    }

    public class TechCardDto
    {
        public int? eduyear { get; set; }
        public int? term { get; set; }
        public string termType { get; set; }
        //public string chairUUID { get; set; }
        //public string disciplineLoad { get; set; }
        //public string formativeUUID { get; set; }
        public string rdis { get; set; }
        public string kgrp { get; set; }
        //public decimal? ratio { get; set; }
        public decimal? termRatio { get; set; }
        public List<TechCardLoadDto> loads { get; set; }
    }

    public class TechCardLoadControlDto
    {
        public string startDate { get; set; }
        public string UUID { get; set; }
        //public string endDate { get; set; }
        //public int? standart { get; set; }
        //public string controlActionURL { get; set; }
        public string controlAction { get; set; }
        public string kmer { get; set; }
        //public decimal? courseRatio { get; set; }
        public int maxValue { get; set; }
        //public decimal? factor { get; set; }
        public int? intermediate { get; set; }
        //public object load { get; set; }
        //public object teacher { get; set; }
    }

    public class TechCardLoadDto
    {
        public string technologyCardType { get; set; }
        public decimal? currentFactor { get; set; }
        public decimal? totalFactor { get; set; }
        public decimal? intermediateFactor { get; set; }
        public List<TechCardLoadControlDto> controls { get; set; }
    }
}