using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Processors
{
    public class ProfilesProcessor : IBlockContentProcessor
    {
        private readonly JObject _actualDocumentData;

        public ProfilesProcessor(JObject actualDocumentData)
        {
            _actualDocumentData = actualDocumentData;
        }

        public JToken ProcessContent(JToken data)
        {
            var itemId = data.Value<string>(nameof(ProfileTrajectoriesInfo.Id));
            if (itemId == null)
                return data;

            var directionIds = _actualDocumentData[nameof(ModuleWorkingProgramFgosVoSchemaModel.Directions)]
                .Select(d => d.Value<string>(nameof(DirectionInfo.Id))).ToList();

            var profileDirectionId = data[nameof(ProfileTrajectoriesInfo.DirectionId)].Value<string>();
            if (!directionIds.Contains(profileDirectionId))
            {
                return JObject.FromObject(new ProfileTrajectoriesViewModel());
            }

            return data;
        }
    }
}