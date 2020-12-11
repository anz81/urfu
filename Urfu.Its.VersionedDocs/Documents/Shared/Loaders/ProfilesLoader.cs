using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class ProfilesLoader : ObjectBlockContentLoader<IEnumerable<ProfileTrajectoriesViewModel>>
    {
        private readonly ApplicationDbContext _db;
        private readonly JObject _loadedDocumentData;

        public ProfilesLoader(ApplicationDbContext db, JObject loadedDocumentData)
        {
            _db = db;
            _loadedDocumentData = loadedDocumentData;
        }

        protected override IEnumerable<ProfileTrajectoriesViewModel> LoadAnyContent(JToken blockContent)
        {
            var profilesData = blockContent as JArray;

            var directionIds = _loadedDocumentData[nameof(ModuleWorkingProgramFgosVoSchemaModel.Directions)]
                .Select(d => d.Value<string>(nameof(DirectionInfo.Id))).ToList();

            var filteredProfiles = profilesData;//.Where(p => directionIds.Contains(p.Value<string>(nameof(ProfileTrajectoriesInfo.DirectionId)))).ToList();
            var filteredProfileIds = filteredProfiles.Select(p => p.Value<string>(nameof(ProfileTrajectoriesInfo.Id)));
            var profiles = _db.Profiles.Where(p => filteredProfileIds.Contains(p.ID)).ToList();

            foreach (var profile in profiles)
            {
                var jsonItem = filteredProfiles.First(p => p.Value<string>(nameof(ProfileTrajectoriesInfo.Id)) == profile.ID);
                var item = new ProfileTrajectoriesViewModel
                {
                    Id = profile.ID,
                    Name = profile.NAME,
                    Code = profile.CODE,
                    DirectionId = profile.DIRECTION_ID,
                    Trajectories = jsonItem[nameof(ProfileTrajectoriesInfo.Trajectories)].Values<string>().ToList()
                };

                yield return item;
            }                       
        }
    }
}