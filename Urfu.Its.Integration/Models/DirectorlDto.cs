using System.Collections.Generic;
namespace Urfu.Its.Integration.Models
{
    public class DirectorDto
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string lastName { get; set; }
        
        /// <summary>
        /// Имя
        /// </summary>
        public string firstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string middleName { get; set; }

        public string divisionUUID { get; set; }
    }
}