using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class ProjectStudentInfoService
    {
        readonly string _restService = ConfigurationManager.AppSettings["ProjectStudentInfoServiceAddress"];

        public string GetStudentInfo(string id)
        {
            try
            {
                var url = new Uri(_restService.Replace("{id}", id));

                var http = (HttpWebRequest)WebRequest.Create(url);
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "GET";

                var response = http.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                    return content;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}