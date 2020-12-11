using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using AutoMapper;
using Newtonsoft.Json.Schema;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Services
{
    public class JSchemaVersionedDocumentDescriptorFactory : IVersionedDocumentDescriptorFactory
    {
        private readonly JSchema _schema;

        public JSchemaVersionedDocumentDescriptorFactory(JSchema schema)
        {
            _schema = schema;
        }

        public VersionedDocumentDescriptor CreateDocumentDescriptor()
        {
            return CreateDescriptorFromSchema(_schema);
        }

        private VersionedDocumentDescriptor CreateDescriptorFromSchema(JSchema schema)
        {
            var descriptor = new VersionedDocumentDescriptor();
            foreach (var property in schema.Properties)
            {
                var blockDescriptor = new VersionedDocumentBlockDescriptor(property.Key, ConvertSchemaToKind(property.Value));
                FillItemDescriptor(blockDescriptor, property.Value);
                descriptor.Blocks.Add(blockDescriptor);
            }

            return descriptor;
        }

        private void FillItemDescriptor(IVersionedDocumentBlockItemDescriptor itemDescriptor, JSchema schema)
        {
            switch (itemDescriptor.Kind)
            {
                case VersionedDocumentBlockItemKind.Object:
                    foreach (var property in schema.Properties)
                    {
                        var subItemDescriptor = new VersionedDocumentBlockItemDescriptor(property.Key, ConvertSchemaToKind(property.Value));
                        FillItemDescriptor(subItemDescriptor, property.Value);
                        itemDescriptor.Properties.Add(subItemDescriptor);
                    }
                    break;
                case VersionedDocumentBlockItemKind.Array:
                {
                    var itemsSchema = schema.Items[0];
                    var subItemDescriptor = new VersionedDocumentBlockItemDescriptor(ConvertSchemaToKind(itemsSchema));
                    FillItemDescriptor(subItemDescriptor, itemsSchema);
                    itemDescriptor.Items = subItemDescriptor;
                }
                    break;
            }
        }


        private VersionedDocumentBlockItemKind ConvertSchemaToKind(JSchema schema)
        {
            return (VersionedDocumentBlockItemKind)schema.Type;
        }
    }

    public class VersionedDocumentDescriptorService : IVersionedDocumentDescriptorService
    {
        private readonly ILifetimeScope _scope;

        public VersionedDocumentDescriptorService(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public IEnumerable<VersionedDocumentDescriptorChange> GetChanges(VersionedDocumentDescriptor sourceDesc,
            VersionedDocumentDescriptor targetDesc)
        {
            var syncNodes = EnumerateSyncNodes(null, sourceDesc.Blocks.ToArray(), null, targetDesc.Blocks.ToArray()).ToList();
            foreach (var syncNode in syncNodes)
            {
                if (syncNode.Item1 != null && syncNode.Item2 != null)
                {
                    if (syncNode.Item1.Kind != syncNode.Item2.Kind)
                    {
                        var chain = PrepareParentChain(targetDesc, syncNode.Item2);
                        yield return new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemTypeChanged, chain)
                        {
                            OldItem = syncNode.Item1,
                            NewItem = syncNode.Item2
                        };
                    }
                }
                else
                {
                    if (syncNode.Item1 == null)
                    {
                        var chain = PrepareParentChain(targetDesc, syncNode.Item2);
                        yield return new VersionedDocumentDescriptorChange(
                            VersionedDocumentSchemaChangeAction.ItemAdded, chain)
                        {
                            NewItem = syncNode.Item2
                        };
                    }

                    if (syncNode.Item2 == null)
                    {
                        var chain = PrepareParentChain(targetDesc, syncNode.Item2Container).ToList();
                        if (syncNode.Item2Container != null)
                            chain.Add(syncNode.Item2Container);
                        yield return new VersionedDocumentDescriptorChange(
                            VersionedDocumentSchemaChangeAction.ItemRemoved, chain)
                        {
                            OldItem = syncNode.Item1
                        };
                    }
                }
            }
        }

        public object BuildDefaultContent(IVersionedDocumentBlockItemDescriptor itemDescriptor)
        {
            if (itemDescriptor.DefaultContentBuilderType != null)
            {
                if (itemDescriptor.DefaultContentBuilderType != typeof(void))
                {
                    using (var blockScope = _scope.BeginLifetimeScope(b =>
                    {
                        b.RegisterType(itemDescriptor.DefaultContentBuilderType).AsSelf();
                        b.RegisterInstance(itemDescriptor).As<IVersionedDocumentBlockItemDescriptor>();
                    }))
                    {
                        var contentBuilder = (IBlockItemContentBuilder) blockScope.Resolve(itemDescriptor.DefaultContentBuilderType);
                        var content = contentBuilder.BuildDefaultContent();
                        if (content != null)
                        {
                            var syncNodes = itemDescriptor.EnumerateObjectSyncNodes(content).ToList();
                            foreach (var syncNode in syncNodes
                                .Where(n => n.Item2 != null && n.Item1 != null))
                            {
                                var content2 = BuildDefaultContent(syncNode.Item2);
                                if (content2 != null) // перезаписываем значение, если получены новые данные
                                {
                                    if (syncNode.Item1.PropertyType.IsInstanceOfType(content2))
                                    {
                                        syncNode.Item1.SetValue(content, content2);
                                    }
                                    else if(syncNode.Item1.PropertyType != typeof(string) && !syncNode.Item1.PropertyType.IsValueType)
                                    {
                                        var sourceItem = syncNode.Item1.GetValue(content);
                                        if (sourceItem == null)
                                        {
                                            sourceItem = Activator.CreateInstance(syncNode.Item1.PropertyType);
                                            syncNode.Item1.SetValue(content, sourceItem);
                                        }

                                        //if (!(content is IEnumerable enumerable) || enumerable.Cast<object>().Any())
                                        var mce = new AutoMapper.Configuration.MapperConfigurationExpression();
                                        var config = new MapperConfiguration(mce);
                                        var mapper = new Mapper(config);
                                        mapper.Map(content2, sourceItem, content2.GetType(), sourceItem.GetType());
                                    }                                    
                                }
                            }
                        }
                        
                        return content;
                    }
                }
            }

            return null;
        }        
        
        private IEnumerable<IVersionedDocumentBlockItemDescriptor> PrepareParentChain(VersionedDocumentDescriptor docDesc, IVersionedDocumentBlockItemDescriptor item)
        {
            var chain = new List<IVersionedDocumentBlockItemDescriptor>();
            FindRecursive(docDesc.Blocks, chain, item);
            chain.Reverse();
            return chain;
        }

        private bool FindRecursive(IEnumerable<IVersionedDocumentBlockItemDescriptor> children, ICollection<IVersionedDocumentBlockItemDescriptor> chain, IVersionedDocumentBlockItemDescriptor item)
        {
            foreach (var child in children)
            {
                if (Equals(child, item))
                    return true;

                var items = new IVersionedDocumentBlockItemDescriptor[0];
                if (child.Kind.HasFlag(VersionedDocumentBlockItemKind.Object))
                {
                    items = child.Properties.ToArray();
                }
                else if (child.Kind.HasFlag(VersionedDocumentBlockItemKind.Array))
                {
                    items = new[] {child.Items};
                }

                if (FindRecursive(items, chain, item))
                {
                    if(child.Name != null) // Если не дескиптор Items элемента коллекции. Не хорошая проверка. TODO будет время - надо переделать.
                        chain.Add(child);
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<SyncNode> EnumerateSyncNodes(
                IVersionedDocumentBlockItemDescriptor container1,
                IVersionedDocumentBlockItemDescriptor[] descriptors1,
                IVersionedDocumentBlockItemDescriptor container2,
                IVersionedDocumentBlockItemDescriptor[] descriptors2)
        {
            foreach (var d2 in descriptors2)
            {
                var d1 = descriptors1.FirstOrDefault(d => d.Name == d2.Name);
                if (d1 == null)
                {
                    yield return new SyncNode(null, container1, d2, container2);
                }
                else
                {
                    yield return new SyncNode(d1, container1, d2, container2);
                    
                    if (d1.Kind.HasFlag(VersionedDocumentBlockItemKind.Object))
                    {
                        foreach (var syncNode in EnumerateSyncNodes(d1, d1.Properties.ToArray(), d2, d2.Properties.ToArray()))
                        {
                            yield return syncNode;
                        }
                    }
                    else if (d1.Kind.HasFlag(VersionedDocumentBlockItemKind.Array))
                    {
                        foreach (var syncNode in EnumerateSyncNodes(d1, new[]{d1.Items}, d2, new []{d2.Items}))
                        {
                            yield return syncNode;
                        }
                    }
                }
            }

            foreach (var d1 in descriptors1)
            {
                var d2 = descriptors2.FirstOrDefault(d => d.Name == d1.Name);
                if (d2 == null)
                {
                    yield return new SyncNode(d1, container1, null, container2);
                }
            }
        }

        private class SyncNode
        {
            public SyncNode(IVersionedDocumentBlockItemDescriptor item1, IVersionedDocumentBlockItemDescriptor item1Container, IVersionedDocumentBlockItemDescriptor item2, IVersionedDocumentBlockItemDescriptor item2Container)
            {
                Item1 = item1;
                Item1Container = item1Container;
                Item2 = item2;
                Item2Container = item2Container;
            }

            public IVersionedDocumentBlockItemDescriptor Item1 { get; }
            public IVersionedDocumentBlockItemDescriptor Item1Container { get; }
            public IVersionedDocumentBlockItemDescriptor Item2 { get; }
            public IVersionedDocumentBlockItemDescriptor Item2Container { get; }
        }
    }    
}