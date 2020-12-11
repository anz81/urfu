using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;


namespace Urfu.Its.Integration
{
    public class SedDocument
    {
        public int? base_issue_sedid { get; set; }
        public string issue_number { get; set; }
        public DateTime issue_date { get; set; }
        public string group_uuid { get; set; }
        public string group_name { get; set; }
        public int? term { get; set; }
        public string term_name { get; set; }
        public int? year { get; set; }
        public string institution_uuid { get; set; }
        public string institution_name { get; set; }
        public string started_by { get; set; }
        public string profile_uuid { get; set; }
        public string profile_name { get; set; }
        public string practice_uuid { get; set; }
        public string practice_type { get; set; }
        public string supervisors { get; set; }
        public byte[] file { get; set; }
        public int file_size { get; set; }
        public string file_name { get; set; }
        public string sed_date { get; set; }
        public string sed_state { get; set; }
        public string sed_lastcomm { get; set; }
        public string sed_syscomm { get; set; }
        public string AdName { get; set; }
        public string SamAccountName { get; set; }
    };

    public class SedRestService
    {
        readonly string _restService = ConfigurationManager.AppSettings["SedRestAddress"];
        readonly string _user = ConfigurationManager.AppSettings["SedRestUser"];
        readonly string _password = ConfigurationManager.AppSettings["SedRestPassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public int SendDocument(SedDocument document, string method, int? sedId)
        {
            var res = SendWithRequest(document, method, sedId);
            return int.Parse(res);

            //var wc = new WebClient
            //{
            //    UseDefaultCredentials = true,
            //    Credentials = new NetworkCredential(_user, _password)
            //};

            //using (var data = wc.OpenRead(_restService + "/groups"))
            //{
            //    return ((GroupsXmlDto)GroupsSerializer.Deserialize(data)).Groups;
            //}
        }


        public SedDocument GetDocument(int id)
        {
            var document = GetWidthRequest(id);
            return document;
            
            //var wc = new WebClient
            //{
            //    UseDefaultCredentials = true,
            //    Credentials = new NetworkCredential(_user, _password)
            //};

            //var query = $"{_restService}/{id}";
            //using (var data = wc.OpenRead(query))
            //using (var streamReader = new StreamReader(data))
            //using (var jsonTextReader = new JsonTextReader(streamReader))
            //{
            //    try
            //    {
            //        var document = _jsonSerializer.Deserialize<SedDocument>(jsonTextReader);
            //        return document;
            //    }
            //    catch (Exception ex)
            //    {
            //        return new SedDocument
            //        {
            //            sed_state = null,
            //            sed_syscomm = ex.Message,
            //            sed_lastcomm = "Судьба приказа не известна"
            //        };
            //    }
            //}
        }

        private SedDocument GetWidthRequest(int id)
        {
            try
            {
                var query = $"{_restService}/{id}";

                var http = (HttpWebRequest)WebRequest.Create(new Uri(query));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "GET";
                http.Credentials = new NetworkCredential(_user, _password);

                using (var response = http.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    return DeserializeDocument(stream);
                }
            }
            catch (Exception ex)
            {
                return new SedDocument
                {
                    sed_state = null,
                    sed_syscomm = ex.Message,
                    sed_lastcomm = "Судьба приказа не известна"
                };
            }
        }

        private string SendWithRequest(SedDocument document, string method, int? sedId)
        {
            string parsedContent = SerializeDocument(document);

            var encoding = new UTF8Encoding();
            var bytes = encoding.GetBytes(parsedContent);

            var url = method == "PUT" ? new Uri(_restService + "/" + sedId) : new Uri(_restService);

            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = method;
            http.Credentials = new NetworkCredential(_user, _password);

            using (var newStream = http.GetRequestStream())
            {
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
            }
            
            var response = http.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                return content;
            }
        }

        private string SerializeDocument(SedDocument document)
        {
            using (var writer = new StringWriter())
            {
                _jsonSerializer.Serialize(writer, document);
                writer.Flush();
                return writer.ToString();
            }
        }

        private SedDocument DeserializeDocument(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            using (var jr = new JsonTextReader(sr))
            {
                var doc = _jsonSerializer.Deserialize<SedDocument>(jr);
                return doc;
            }
        }
    }


}
