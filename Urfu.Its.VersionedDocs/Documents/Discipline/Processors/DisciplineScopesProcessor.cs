using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Processors
{
    [Obsolete("Логика перенесена в DisciplineScopesLoader")]
    public class DisciplineScopesProcessor : IBlockContentProcessor
    {
        private readonly JObject _actualDocumentData;

        public DisciplineScopesProcessor(JObject actualDocumentData)
        {
            _actualDocumentData = actualDocumentData;
        }

        public JToken ProcessContent(JToken data)
        {
            var result = new JArray();
            var items = (JArray) data;
            var fdps = _actualDocumentData[nameof(DisciplineWorkingProgramFgosVoSchemaModel.Fdps)];
            var existingItems = items.Where(item =>fdps.Select(fdp => fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>()).Contains(item[nameof(DisciplineScopeInfo.FdpId)].Value<string>())).ToList();
            var newFdps = fdps.Where(fdp => !items.Select(item => item[nameof(DisciplineScopeInfo.FdpId)].Value<string>()).Contains(fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>())).ToList();
            //var oldItems = items.Where(item => !fdps.Select(fdp => fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>()).Contains(item[nameof(DisciplineScopeInfo.FdpId)].Value<string>()));

            foreach (var item in existingItems)
                result.Add(item);

            foreach (var newFdp in newFdps)
            {
                result.Add(JToken.FromObject(new DisciplineScopeInfo
                {
                    FdpId = newFdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>()
                }));
            }

            //foreach (var oldItem in oldItems)
            //    items.Remove(oldItem);

            return result;
        }
    }
}