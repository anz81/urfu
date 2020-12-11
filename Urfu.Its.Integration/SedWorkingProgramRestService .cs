using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Urfu.Its.Integration.Models;


namespace Urfu.Its.Integration
{
    
    public class SedWorkingProgramRestService
    {
        readonly string _restService = ConfigurationManager.AppSettings["SedWorkingProgramRestAddress"];
        readonly string _restServiceMod = ConfigurationManager.AppSettings["SedWorkingProgramModRestAddress"];
        readonly string _user = ConfigurationManager.AppSettings["SedWorkingProgramRestUser"];
        readonly string _password = ConfigurationManager.AppSettings["SedWorkingProgramRestPassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        

        public void SendDocument(WorkingProgramDocumentDto document)
        {
            var res = SendWithRequest(document);
            //return int.Parse(res);

        }


        public StatusWorkingProgramDocument GetStatusDocument(string id)
        {
            var document = GetWidthRequest(id);
            return document;
           
        }

        private StatusWorkingProgramDocument GetWidthRequest(string id)
        {
            try
            {
                var query = $"{_restServiceMod}/{id}";

                var http = (HttpWebRequest)WebRequest.Create(new Uri(query));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "GET";
                http.Credentials = new NetworkCredential(_user, _password);

                using (var response = http.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    return DeserializeStatusDocument(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return new WorkingProgramDocumentDto
                //{
                //    sed_state = null,
                //    sed_syscomm = ex.Message,
                //    sed_lastcomm = "Судьба приказа не известна"
                //};
            }
        }

        private string SendWithRequest(WorkingProgramDocumentDto document)
        {
            string parsedContent = SerializeDocument(document);

            var encoding = new UTF8Encoding();
            var bytes = encoding.GetBytes(parsedContent);

            var http = (HttpWebRequest)WebRequest.Create(new Uri(_restService));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
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

        private string SerializeDocument(WorkingProgramDocumentDto document)
        {
            using (var writer = new StringWriter())
            {
                _jsonSerializer.Serialize(writer, document);
                writer.Flush();
                return writer.ToString();
            }
        }

        private WorkingProgramDocumentDto DeserializeDocument(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            using (var jr = new JsonTextReader(sr))
            {
                var doc = _jsonSerializer.Deserialize<WorkingProgramDocumentDto>(jr);
                return doc;
            }
        }

        private StatusWorkingProgramDocument DeserializeStatusDocument(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            using (var jr = new JsonTextReader(sr))
            {
                var doc = _jsonSerializer.Deserialize<StatusWorkingProgramDocument>(jr);
                return doc;
            }
        }
    }


}
