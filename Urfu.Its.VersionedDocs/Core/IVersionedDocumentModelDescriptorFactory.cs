namespace Urfu.Its.VersionedDocs.Core
{
    public interface IVersionedDocumentDescriptorFactory
    {
        VersionedDocumentDescriptor CreateDocumentDescriptor();
    }

    public interface IVersionedDocumentModelDescriptorFactory<TModel> : IVersionedDocumentDescriptorFactory
    {
    }
}