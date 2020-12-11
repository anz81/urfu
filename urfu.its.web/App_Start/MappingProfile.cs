using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Integration.ApiModel;

namespace Urfu.Its.Web.App_Start
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Subgroup, SubgroupApiDto>();
            CreateMap<SubgroupApiDto, Subgroup>();
        }
    }
}