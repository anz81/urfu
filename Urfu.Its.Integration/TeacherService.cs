using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Urfu.Its.Common;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class TeacherService
    {
        readonly string _connectionString = ConfigurationManager.AppSettings["EduLoadConectionString"];

        public IEnumerable<TeacherDto> GetTeachers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = commandText;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new TeacherDto
                        {
                            pkey = (string)reader["pkey"],
                            lastName = Cast(reader["lastName"]),
                            firstName = Cast(reader["firstName"]),
                            middleName = Cast(reader["middleName"]),
                            initials = Cast(reader["initials"]),
                            workPlace = Cast(reader["workPlace"]),
                            division = Cast(reader["division"]),
                            post = Cast(reader["post"]),
                            accountancyGuid = Cast(reader["accountancyGuid"]),
                            academicTitle = Cast(reader["academicTitle"]),
                            academicDegree = Cast(reader["academicDegree"])
                        };
                    }
                }
            }
        }

        private string Cast(object o)
        {
            if (o is DBNull)
                return null;
            return (string) o;
        }


        const string commandText = @"select 
pkey,
lastName,
firstName,
middleName,
initials,
coalesce(workPlace, divisionTitle, 'УрФУ') workPlace,
division,
post,
accountancyGuid,
academicTitle,
academicDegree
from (

select
pkey=p.tkey,
lastName=p.lastName,
firstName=p.firstName,
middleName=p.middleName,
initials=p.lastName+' '+left(p.firstName,1)+'.'+case when left(p.middleName,1) is null then '' else left(p.middleName,1)+'.' end,
workPlace = case when romr like 'Кафедра%' then null else romr end,
divisionTitle=ltrim(rtrim(replace(f.romr, 'Кафедра ', ''))),
division=coalesce(f.mainPlaceOfWorkId, (select top 1 d.uuid from Division d where d.title=ltrim(rtrim(replace(f.romr, 'Кафедра ', ''))))),
post=
case 
when f.rdlg like 'ст%пр%' then 'Старший преподаватель'
when f.rdlg like 'ассистент%' then 'Ассистент'
when f.rdlg like 'зав%каф%' then 'Заведующий кафедрой'
when f.rdlg like 'доцент%' then 'Доцент'
when f.rdlg like 'пр%фессор%' then 'Профессор'
when f.rdlg like 'зав%лаб%' then 'Заведующий лабораторией'
when f.rdlg like 'сдоцент%' then 'Доцент'
when f.rdlg like 'Инженер%' then 'Инженер'
when f.rdlg like 'декан%' then 'Декан'
when f.rdlg like 'аспирант%' then 'Аспирант'
when f.rdlg like 'Директор%' then 'Директор'
else 'Преподаватель' end,
snils = nullif(ltrim(rtrim(replace(replace(f.pssv, '-', ''), ' ', ''))),''),
accountancyGuid = p.accountancyGuid,
academicTitle =uzv.ruzv,
academicDegree =ust.rust

from TkFizL f
join TPerson p on p.id=f.person
join Tktst st on f.ktst=st.ktst
join Tkust ust on f.kust =ust.kust
join Tkuzv uzv on f.kuzv=uzv.kuzv

where not exists(select 1 from TkFizL ff where ff.person=f.person and (ff.ktst<f.ktst or (ff.ktst=f.ktst and ff.tn>f.tn)))


)  tt";



        static readonly ConnectionFactory Factory = new ConnectionFactory
        {
            HostName = ConfigurationManager.AppSettings["RabbitMQServer"],
            UserName = ConfigurationManager.AppSettings["RabbitMQLogin"],
            Password= ConfigurationManager.AppSettings["RabbitMQPassword"],
            Port = 7070
        };
        static readonly string RabbitMQQueueName = ConfigurationManager.AppSettings["RabbitMQTeachersQueueName"];
        static readonly string RabbitMQRoutingKey = ConfigurationManager.AppSettings["RabbitMQTeachersRoutingKey"];
        public IEnumerable<TeacherDto> GetTeachersFromQueue()
        {
            using (var connection = Factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    for (;;)
                    {
                        var consumer = new QueueingBasicConsumer(channel);
                        
                        channel.BasicConsume(RabbitMQQueueName, true, consumer);
                        BasicDeliverEventArgs ea = consumer.Queue.DequeueNoWait(null);
                        if (ea == null)
                            yield break;
                        Logger.Info("Получено новое сообщение с преподавателем");
                        var json = Encoding.UTF8.GetString(ea.Body);
                        var dto = JsonConvert.DeserializeObject<TeacherDto>(json);
                        yield return dto;
                    }
                }
            }
        }
    }
}