using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Ajax.Utilities;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers.Api
{
    //[IdentityBasicAuthentication]
    public class VariantAdmissionsController : BaseController
    {
        // GET: VariantAdmission
        public List<VariantAdmDto> Get(string groupId=null)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.VariantAdmissions.OrderByDescending(_ => _.Variant.Program.Year)
                    .Where(_ => _.Status == AdmissionStatus.Admitted && (groupId == null || _.Student.GroupId == groupId))
                  
                    .Select(_ => new VariantAdmDto()
                    {
                        studentId = _.studentId,
                        variantId = _.variantId,
                        variantName = _.Variant.DocumentName,
                        profileId =_.Variant.Program.profileId
                    })
                    .Distinct()
                    .ToList();


            }

            
        }

    }

    public class VariantAdmDto
    {
        public string studentId { get; set; }
        public int variantId { get; set; }
        public string variantName { get; set; }
        public string profileId { get; set; }
    }
}