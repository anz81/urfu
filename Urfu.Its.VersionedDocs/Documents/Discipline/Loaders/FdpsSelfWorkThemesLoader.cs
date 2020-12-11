using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class FdpsSelfWorkThemesLoader : ObjectBlockContentLoader<IEnumerable<FdpSelfWorkThemesInfo>>
    {
        private readonly JObject _loadedDocumentData;

        public FdpsSelfWorkThemesLoader(JObject loadedDocumentData)
        {
            _loadedDocumentData = loadedDocumentData;
        }

        protected override IEnumerable<FdpSelfWorkThemesInfo> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData["Fdps"];
            // TODO BlockDataHelper.GetActualMergedData
            var actualItems = GetActualItems(fdps, (JArray)blockContent).ToList();
            return actualItems;
        }

        [Obsolete("Нужно использовать BlockDataHelper.GetActualMergedData")]
        private static IEnumerable<FdpSelfWorkThemesInfo> GetActualItems(JToken fdps, JArray linkItems)
        {
            var sourceItemIdKey = nameof(FamilirizationTypeDirectionPlanInfo.ItemId);
            var linkIdKey = nameof(FdpSelfWorkThemesInfo.FdpId);
            var actualFdpIds = fdps.Select(fdp => fdp[sourceItemIdKey].Value<string>());
            var oldFdpIds = linkItems.Select(item => item[linkIdKey].Value<string>());
            var existingLinks = linkItems
                .Where(item => actualFdpIds.Contains(item[linkIdKey].Value<string>()))
                .Select(item => JsonConvert.DeserializeObject<FdpSelfWorkThemesInfo>(item.ToString())).ToList();
            var newFdps = fdps.Where(fdp =>
                !oldFdpIds.Contains(fdp[sourceItemIdKey].Value<string>())).ToList();

            var newDisciplineScopes = newFdps.Select(f => new FdpSelfWorkThemesInfo
            {
                FdpId = f[sourceItemIdKey].Value<string>(),
            });

            var actualLinks = existingLinks.Concat(newDisciplineScopes).ToList();

            foreach (var fdp in fdps)
            {
                var fdpId = fdp[sourceItemIdKey].Value<string>();
                var ds = actualLinks.FirstOrDefault(s => s.FdpId == fdpId);
                yield return ds;
            }
        }
    }
}