using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Urfu.Its.VersionedDocs.Core
{
    public static class VersionedDocumentBlockDescriptorExtensions
    {
        public static IEnumerable<VersionedDocumentBlockDescriptor> GetDependentBlocks(this VersionedDocumentBlockDescriptor blockDescriptor, VersionedDocumentDescriptor documentDescriptor, bool recursive = true)
        {            
            foreach (var d in documentDescriptor.Blocks.Where(b => blockDescriptor.DependentBlocks.Contains(b.Name)))
            {
                yield return d;
                if (!recursive) continue;
                foreach (var d2 in GetDependentBlocks(d, documentDescriptor))
                    yield return d2;
            }            
        }

        public static IEnumerable<VersionedDocumentBlockDescriptor> GetParentDependentBlocks(
            this VersionedDocumentBlockDescriptor blockDescriptor, VersionedDocumentDescriptor documentDescriptor,
            bool recursive = true)
        {
            foreach (var d in documentDescriptor.Blocks.Where(b => b.DependentBlocks.Contains(blockDescriptor.Name)))
            {
                yield return d;
                if (!recursive) continue;
                foreach (var d2 in GetParentDependentBlocks(d, documentDescriptor))
                    yield return d2;
            }
        }
    }

    public static class VersionedDocumentBlockItemDescriptorExtensions
    {
        public static IEnumerable<ObjectSyncNode> EnumerateObjectSyncNodes(
            this IVersionedDocumentBlockEnumerableItem descriptor, object obj)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var d1 = descriptor.EnumerateItems().FirstOrDefault(d => d.Name == prop.Name);
                if (d1 == null)
                {
                    yield return new ObjectSyncNode(obj, prop, descriptor, null);
                }
                else
                {
                    yield return new ObjectSyncNode(obj, prop, descriptor, d1);

                    var val = prop.GetValue(obj);
                    if (val != null && 
                        (d1.Kind.HasFlag(VersionedDocumentBlockItemKind.Object)
                           || d1.Kind.HasFlag(VersionedDocumentBlockItemKind.Array)))
                        foreach (var syncNode in EnumerateObjectSyncNodes(d1, val))
                        {
                            yield return syncNode;
                        }
                }
            }

            foreach (var d1 in descriptor.EnumerateItems())
            {
                var prop = properties.FirstOrDefault(d => d.Name == d1.Name);
                if (prop == null)
                {
                    yield return new ObjectSyncNode(obj, null, descriptor, d1);
                }
            }
        }
    }

    public class ObjectSyncNode
    {
        public ObjectSyncNode(object item1Container, PropertyInfo item1, IVersionedDocumentBlockEnumerableItem item2Container, IVersionedDocumentBlockItemDescriptor item2)
        {
            Item1Container = item1Container;
            Item1 = item1;
            Item2Container = item2Container;
            Item2 = item2;
        }

        public object Item1Container { get; }
        public PropertyInfo Item1 { get; }
        public IVersionedDocumentBlockItemDescriptor Item2 { get; }
        public IVersionedDocumentBlockEnumerableItem Item2Container { get; }
    }
}