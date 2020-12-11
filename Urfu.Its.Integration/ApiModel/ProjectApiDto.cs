using System;
using System.Collections.Generic;

namespace Urfu.Its.Integration.Models
{
    public class OrganizationApiDto
    {
        public string name { get; set; }
        public bool urfu { get; set; }
    }
    public class CustomerApiDto
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string division { get; set; }
        public string position { get; set; }
    }
    public class FileApiDto
    {
        public Uri url { get; set; }
        public string name { get; set; }
    }

    public class ProgramManagerApiDto
    {
        public string samAccountName { get; set; }
        public string userPrincipalName { get; set; }
        public string runpId { get; set; }
    }

    public class LimitApiDto
    {
        public int id { get; set; }
        public int year { get; set; }
        public int semesterId { get; set; }
        public int course { get; set; }
        public string profileId { get; set; }
        public int limit { get; set; }
    }

    public class ProjectRoleApiDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }

    public class ProjectApiDto
    {
        public int id { get; set; }
        public OrganizationApiDto organization { get; set; }
        public CustomerApiDto customer { get; set; }
        public string title { get; set; }
        public string shortTitle { get; set; }
        public string description { get; set; }
        public string target { get; set; }
        public string level { get; set; }
        public string summary { get; set; }
        public FileApiDto file { get; set; }
        public int? maxSubgroups { get; set; }
        public ProgramManagerApiDto programManager { get; set; }
        public List<string> programManagerProfileIds { get; set; }
        public List<string> teachers { get; set; }
        public List<LimitApiDto> limits { get; set; }
        public List<ProjectRoleApiDto> roles { get; set; }
    }
}