using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CsvHelper;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class UniRestService
    {
        static readonly XmlSerializer ProfilesSerializer = new XmlSerializer(typeof(ProfilesXmlDto));
        static readonly XmlSerializer DirectionSerializer = new XmlSerializer(typeof(DirectionsXmlDto));
        static readonly XmlSerializer GroupsSerializer = new XmlSerializer(typeof(GroupsXmlDto));
        static readonly XmlSerializer StudentsSerializer = new XmlSerializer(typeof(StudentsXmlDto));
        static readonly XmlSerializer PersonsSerializer = new XmlSerializer(typeof(PersonsXmlDto));

        readonly string _restService = ConfigurationManager.AppSettings["UniRestAddress"];
        readonly string _user = ConfigurationManager.AppSettings["UniRestUser"];
        readonly string _password = ConfigurationManager.AppSettings["UniRestPassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        
        [Obsolete]
        public List<DirectionXmlDto> GetDirectionsXml()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user,_password)
            };

            using (var data = wc.OpenRead(_restService+"/directions"))
            {
                return ((DirectionsXmlDto) DirectionSerializer.Deserialize(data)).Directions;
            }
        }

        public List<ProfileXmlDto> GetProfilesXml()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user,_password)
            };
            return WrapWithFakeHttpsHandler(() =>
            {
                using (var data = wc.OpenRead(_restService + "/newprofiles"))
                using (var streamReader = new StreamReader(data))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return _jsonSerializer.Deserialize<List<ProfileXmlDto>>(jsonTextReader);
                }
            });
        }

        public List<DirectionDto> GetDirections()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };

            using (var data = wc.OpenRead(_restService + "/specialities"))
            using (var streamReader = new StreamReader(data))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _jsonSerializer.Deserialize<List<DirectionDto>>(jsonTextReader);
            }
        }

        static T WrapWithFakeHttpsHandler<T>(Func<T> func)
        {
            lock (BrsService.GlobalLocker) // this is very very ugly, I know
            {
                var oldCallBack = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback = LksService.CertificateValidationCallBack;
                try
                {
                    return func();
                }
                finally
                {
                    if (ServicePointManager.ServerCertificateValidationCallback ==
                        LksService.CertificateValidationCallBack)
                        ServicePointManager.ServerCertificateValidationCallback = oldCallBack;
                }
            }
        }

        public List<GroupXmlDto> GetGroupsXml()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            return WrapWithFakeHttpsHandler(() =>
            {
                using (var data = wc.OpenRead(_restService + "/groups"))
                {
                    return ((GroupsXmlDto) GroupsSerializer.Deserialize(data)).Groups;
                }
            });
        }

        public List<StudentXmlDto> GetStudentsXml()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            return WrapWithFakeHttpsHandler(() =>
            {
                using (var data = wc.OpenRead(_restService + "/students?course=1,2,3,4,5,6"))
                {
                    return ((StudentsXmlDto) StudentsSerializer.Deserialize(data)).Students;
                }
            });
        }

        public List<PersonXmlDto> GetPersonsXml()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            return WrapWithFakeHttpsHandler(() =>
            {
                using (var data = wc.OpenRead(_restService + "/persons"))
                {
                    return ((PersonsXmlDto) PersonsSerializer.Deserialize(data)).Students;
                }
            });
        }

        public List<StudentPlansPair> GetStudentAllPlans()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            return WrapWithFakeHttpsHandler(() =>
            {
                wc.Headers.Add("Content-type", "text/plain");
                using (var data = wc.OpenRead(_restService + "/all_eduplan/students"))

                {
                    using (var sr = new StreamReader(data))
                    {
                        using (var jsonTextReader = new JsonTextReader(sr))
                        {
                            return _jsonSerializer.Deserialize<List<StudentPlansPair>>(jsonTextReader);
                        }
                    }
                }
            });
        }
        public List<StudentPlanPair> GetStudentPlans()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            return WrapWithFakeHttpsHandler(() =>
            {
                List<StudentPlanPair> list = new List<StudentPlanPair>();
                wc.Headers.Add("Content-type", "text/plain");
                using (var data = wc.OpenRead(_restService + "/eduplan/students"))
                {
                    using (var sr = new StreamReader(data))
                    {
                        //File.WriteAllText("c:\\1.xml",sr.ReadToEnd());
                        var firstLine = sr.ReadLine();

                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var tokens = line.Split(';');
                            if (!string.IsNullOrWhiteSpace(tokens[2]) && !string.IsNullOrWhiteSpace(tokens[4]) && tokens[2] != "null" && tokens[4] != "null")
                                list.Add(new StudentPlanPair
                                {
                                    StudentId = tokens[0],
                                    planId = tokens[1],
                                    planNumber = int.Parse(tokens[2]),
                                    versionId = tokens[3],
                                    versionNumber = int.Parse(tokens[4])
                                });
                        }
                    }
                }
                return list;
            });
        }
        public List<PlanAdditionalDto> GetPlanAdditionalsForDirection(string directionCode)
        {
            var wc = new UniModulesService.UniModulesServiceWebClient()
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            return WrapWithFakeHttpsHandler(() =>
            {
                wc.Headers.Add("Content-type", "text/plain");
                using (var data = wc.OpenRead(_restService + "/eduplan?direction="+directionCode))
                {
                    using (var sr = new StreamReader(data))
                    {
                        using (var csvReader = new CsvReader((IParser)sr))
                        {
                            csvReader.Configuration.Delimiter = ";";

                            return csvReader.GetRecords<PlanAdditionalDto>().ToList();
                        }
                    }
                }
            });

        }

        public List<DirectorDto> GetDirectorsDto()
        {
            var wc = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_user, _password)
            };
            lock (BrsService.GlobalLocker)
            {
                var oldCallBack = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback = LksService.CertificateValidationCallBack;
                try
                {
                    using (var data = wc.OpenRead(_restService + "/heads"))
                    using (var streamReader = new StreamReader(data))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        return _jsonSerializer.Deserialize<List<DirectorDto>>(jsonTextReader);
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
