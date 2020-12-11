using System;
using System.Collections.Generic;

namespace Urfu.Its.Integration.Models
{
    public class TrajectoryDto
    {
        public string trajectory_uuid { get; set; }
        public int? externalid { get; set; }
        public string specialization_uuid { get; set; }
        public string documentname { get; set; }
    }
}