using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class OrganizationsController : BaseController
    {
        public object Get(int year, int semester)
        {
            using (var db = new ApplicationDbContext())
            {
                var dtos = db.ContractPeriods
                    .Include(p => p.Contract.Company)
                    .Include(p => p.Contract.Company.Location)
                    .Include(p => p.Limits.Select(l => l.Direction))
                    .Where(p => p.Year == year && p.SemesterId == semester && p.Contract.Company.Source == Source.Practice)
                    .ToList()
                    .Select(p => new Integration.ApiModel.OrganizationApiDto()
                    {
                        id = p.Contract.Company.Id,
                        name = p.Contract?.Company?.Name,
                        director = p.Contract?.Company?.Director,
                        inn = p.Contract?.Company?.INN,
                        person_in_charge = (p.Contract?.Company?.PersonInCharge != null && p.Contract?.Company?.PostOfPersonInCharge != null) ? 
                            new OrganizationPersonDto()
                            {
                                name = p.Contract?.Company?.PersonInCharge,
                                post = p.Contract?.Company?.PostOfPersonInCharge
                            } 
                            : null,
                        phone = p.Contract?.Company?.PhoneNumber,
                        email = p.Contract?.Company?.Email,
                        address = p.Contract?.Company?.Address,
                        location = p.Contract?.Company?.Location?.FullLocation(),
                        directions =
                            p.Limits.Where(l => l.Direction != null).Select(l => new DirectionApiDto()
                            {
                                uid = l.Direction.uid,
                                okso = l.Direction.okso,
                                title = l.Direction.title
                            })
                    });
                return dtos;
            }
        }
    }
}