namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class ProfileTrajectoriesViewModel : ProfileTrajectoriesInfo
    {
        public string DisplayName => Code + " - " + Name;        
    }
}