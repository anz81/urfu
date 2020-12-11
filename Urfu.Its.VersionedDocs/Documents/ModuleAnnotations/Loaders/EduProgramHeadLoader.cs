using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.ModuleAnnotations.Loaders
{
    public class EduProgramHeadLoader : ObjectBlockContentLoader<AuthorInfo>
    {
        private readonly BasicCharacteristicOPSchemaModel _ohopData;

        public EduProgramHeadLoader(BasicCharacteristicOPSchemaModel ohopData)
        {
            _ohopData = ohopData;
        }

        protected override AuthorInfo LoadAnyContent(JToken blockContent)
        {
            return _ohopData.EduProgramHead;
        }
    }
}