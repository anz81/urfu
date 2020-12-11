using System;
using System.Collections.Generic;
using System.Configuration;
//using System.DirectoryServices;
//using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Oracle.ManagedDataAccess.Client;

namespace Urfu.Its.Integration
{
    /// <summary>
    /// Для задачи http://projects.it.ustu/browse/ITS-742
    /// </summary>
    public class LiteratureService
    {
        private readonly string _connectionString;

        public LiteratureService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["LiteratureConnection"].ConnectionString;
        }

        public AbisRuslanUser GetAbisRuslanUser(string samaccname)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string commandText = "SELECT * FROM LUSR_INFO WHERE SAMACCNAME = :samaccname";

                using (var command = new OracleCommand(commandText, connection){CommandTimeout = 60})
                {
                    command.Parameters.Add("@samaccname", samaccname);
                    var reader = command.ExecuteReader();
                    reader.Read();
                    
                    var user = new AbisRuslanUser
                    {
                        BARCODE = Convert<string>(reader["BARCODE"]),
                        BOOKS_COUNT = System.Convert.ToInt32(Convert<decimal>(reader["BOOKS_COUNT"])),
                        PASSWORD = Convert<string>(reader["PASSWORD"]),
                        SAMACCNAME = Convert<string>(reader["SAMACCNAME"])
                    };

                    return user;                    
                }
            }            
        }
        private T Convert<T>(object o)
        {
            return o == DBNull.Value ? default(T) : (T) o;
        }
    }

    public class AbisRuslanUser
    {
        public string SAMACCNAME { get; set; }
        public string BARCODE { get; set; }
        public string PASSWORD { get; set; }
        public int BOOKS_COUNT { get; set; }
    }
}