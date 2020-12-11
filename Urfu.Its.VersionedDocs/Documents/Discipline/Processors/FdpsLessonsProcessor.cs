using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Processors
{
    [Obsolete("Взамен реализован FdpsLessonsLoader")]
    public class FdpsLessonsProcessor : IBlockContentProcessor
    {
        private readonly JObject _actualDocumentData;

        public FdpsLessonsProcessor(JObject actualDocumentData)
        {
            _actualDocumentData = actualDocumentData;
        }

        public JToken ProcessContent(JToken data)
        {
            var result = new JArray();
            var items = (JArray)data;
            var sourceItems = _actualDocumentData[nameof(DisciplineWorkingProgramFgosVoSchemaModel.Fdps)];
            var sourceItemIdKey = nameof(FamilirizationTypeDirectionPlanInfo.ItemId);
            var linkIdKey = nameof(FdpLessonsInfo.FdpId);
            var existingItems = items.Where(item => sourceItems.Select(s => s[sourceItemIdKey].Value<string>()).Contains(item[linkIdKey].Value<string>())).ToList();
            var newSourceItems = sourceItems.Where(s => !items.Select(item => item[linkIdKey].Value<string>()).Contains(s[sourceItemIdKey].Value<string>())).ToList();
            //var oldItems = items.Where(item => !fdps.Select(fdp => fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>()).Contains(item[nameof(DisciplineScopeInfo.FdpId)].Value<string>()));

            foreach (var item in existingItems)
                result.Add(item);

            foreach (var newSourceItem in newSourceItems)
            {
                result.Add(JToken.FromObject(new FdpLessonsInfo
                {
                    FdpId = newSourceItem[sourceItemIdKey].Value<string>()
                }));
            }

            return result;
        }
    }
}