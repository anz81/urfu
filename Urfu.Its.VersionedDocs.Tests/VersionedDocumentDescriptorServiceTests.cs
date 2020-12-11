using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.Tests.TestData;

namespace Urfu.Its.VersionedDocs.Tests
{
    [TestClass]
    public class VersionedDocumentDescriptorServiceTests
    {
        [TestMethod]
        public void ShouldFillDefaultContentTest()
        {
            var integerContent = "5";
            var integerBlockJsonContent = new VersionedDocumentBlockDescriptor("IntegerBlockJsonContent", VersionedDocumentBlockItemKind.Integer)
            {
                DefaultContentBuilderType = typeof(JsonContentBuilder),
                DefaultContent = integerContent
            };
            var stringBlockJsonContent = new VersionedDocumentBlockDescriptor("StringBlockJsonContent", VersionedDocumentBlockItemKind.String)
            {
                DefaultContentBuilderType = typeof(JsonContentBuilder),
                DefaultContent = "asdf"
            };
            var objectBlockJsonContent = new VersionedDocumentBlockDescriptor("ObjectBlockJsonContent", new []
            {
                new VersionedDocumentBlockItemDescriptor("ObjectBlockStringItem", VersionedDocumentBlockItemKind.String)
            })
            {
                DefaultContentBuilderType = typeof(JsonContentBuilder),
                DefaultContent = new JObject(new JProperty("ObjectBlockStringItem", "test")).ToString()
            };
            var objectBlockStructuralContent = new VersionedDocumentBlockDescriptor("ObjectBlockStructuralContent", new[]
            {
                new VersionedDocumentBlockItemDescriptor("ObjectBlockStringItem", VersionedDocumentBlockItemKind.String)
            })
            {
                DefaultContentBuilderType = typeof(StructuralContentBuilder)
            };
            var objectBlockStructuralAndJsonContent = new VersionedDocumentBlockDescriptor("ObjectBlockStructuralContent", new[]
            {
                new VersionedDocumentBlockItemDescriptor("ObjectBlockStringItem", VersionedDocumentBlockItemKind.String)
                {
                    DefaultContentBuilderType = typeof(JsonContentBuilder),
                    DefaultContent = "test"
                }
            })
            {
                DefaultContentBuilderType = typeof(StructuralContentBuilder)
            };
            var objectBlockWithArrayJsonContent = new VersionedDocumentBlockDescriptor("ObjectBlockWithArrayJsonContent", new[]
            {
                new VersionedDocumentBlockItemDescriptor("ObjectBlockArrayItem", new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.Integer))
                {
                    DefaultContentBuilderType = typeof(JsonContentBuilder),
                    DefaultContent = "[1,2,3]"
                }
            })
            {
                DefaultContentBuilderType = typeof(StructuralContentBuilder)                
            };

            var service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            service.BuildDefaultContent(integerBlockJsonContent).ShouldBeEquivalentTo(5);
            service.BuildDefaultContent(stringBlockJsonContent).ShouldBeEquivalentTo("asdf");
            service.BuildDefaultContent(objectBlockJsonContent).ShouldBeEquivalentTo(new
            {
                ObjectBlockStringItem = "test"
            });
            service.BuildDefaultContent(objectBlockStructuralContent).ShouldBeEquivalentTo(new
            {
                ObjectBlockStringItem = (string) null
            });
            service.BuildDefaultContent(objectBlockStructuralAndJsonContent).ShouldBeEquivalentTo(new
            {
                ObjectBlockStringItem = "test"
            });
            service.BuildDefaultContent(objectBlockWithArrayJsonContent).ShouldBeEquivalentTo(new {
                ObjectBlockArrayItem = new[] {1,2,3}
            });
        }

        [TestMethod]
        public void ShouldPrepareValidChainForSubCollectionsTest()
        {
            var oldDesc = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Collection",
                    new VersionedDocumentBlockItemDescriptor(
                        new[]
                        {
                            new VersionedDocumentBlockItemDescriptor("IntProp", VersionedDocumentBlockItemKind.Integer),
                            new VersionedDocumentBlockItemDescriptor("SubCollection",
                                new VersionedDocumentBlockItemDescriptor(new[]
                                {
                                    new VersionedDocumentBlockItemDescriptor("IntSubProp", VersionedDocumentBlockItemKind.Integer)
                                }))
                        }))});

            var newItemDescriptor = new VersionedDocumentBlockItemDescriptor("NewItem", VersionedDocumentBlockItemKind.String);
            var newDesc = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Collection",
                    new VersionedDocumentBlockItemDescriptor(new[]
                    {
                        new VersionedDocumentBlockItemDescriptor("IntProp", VersionedDocumentBlockItemKind.Integer),
                        new VersionedDocumentBlockItemDescriptor("SubCollection", new VersionedDocumentBlockItemDescriptor(new[]
                        {
                            new VersionedDocumentBlockItemDescriptor("IntSubProp", VersionedDocumentBlockItemKind.Integer),
                            newItemDescriptor
                        }))
                    }))});

            IVersionedDocumentDescriptorService service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            var changes = service.GetChanges(oldDesc, newDesc);

            changes.ShouldBeEquivalentTo(new[]
            {
                new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemAdded,
                    new IVersionedDocumentBlockItemDescriptor[]
                    {
                        newDesc.Blocks.First(b=>b.Name == "Collection"),
                        newDesc.Blocks.First(b=>b.Name == "Collection")
                            .Items.Properties.First(p=>p.Name == "SubCollection")
                    })
                {
                    NewItem = newItemDescriptor
                }
            });
        }

        [TestMethod]
        public void ShouldDetectAddedBlockChangeTest()
        {
            var oldDesc = new VersionedDocumentDescriptor(new[]{new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String)});

            var newBlockDescriptor = new VersionedDocumentBlockDescriptor("p2", VersionedDocumentBlockItemKind.String);
            var newDesc = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String), 
                newBlockDescriptor});

            IVersionedDocumentDescriptorService service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            var changes = service.GetChanges(oldDesc, newDesc);

            changes.ShouldBeEquivalentTo(new[]
            {
                new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemAdded, null)
                {
                    NewItem = newBlockDescriptor
                }
            });
        }

        [TestMethod]
        public void ShouldDetectAddedBlockItemChangeTest()
        {
            var oldDesc = new VersionedDocumentDescriptor(new[]{new VersionedDocumentBlockDescriptor("p1", new[]{
                new VersionedDocumentBlockItemDescriptor("subp1", VersionedDocumentBlockItemKind.Integer),
                new VersionedDocumentBlockItemDescriptor("subp2", VersionedDocumentBlockItemKind.Integer)
            })});

            var newItem = new VersionedDocumentBlockItemDescriptor("subp3", VersionedDocumentBlockItemKind.Integer);
            var changedContainer = new VersionedDocumentBlockDescriptor("p1", new[]
            {
                newItem,
                new VersionedDocumentBlockItemDescriptor("subp1", VersionedDocumentBlockItemKind.Integer),
                new VersionedDocumentBlockItemDescriptor("subp2", VersionedDocumentBlockItemKind.Integer)
            });

            var newDesc = new VersionedDocumentDescriptor(new []{changedContainer});

            IVersionedDocumentDescriptorService service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            var changes = service.GetChanges(oldDesc, newDesc);

            changes.ShouldBeEquivalentTo(new[]
            {
                new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemAdded, new IVersionedDocumentBlockItemDescriptor[]{changedContainer})
                {
                    NewItem = newItem
                }
            });
        }

        [TestMethod]
        public void ShouldDetectRemovedBlockChangeTest()
        {
            var removedItem = new VersionedDocumentBlockDescriptor("p2", VersionedDocumentBlockItemKind.String);

            var oldDesc = new VersionedDocumentDescriptor(new[] {
                removedItem,
                new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String)});

            var newDesc = new VersionedDocumentDescriptor(new []{new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String)});

            IVersionedDocumentDescriptorService service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            var changes = service.GetChanges(oldDesc, newDesc).ToList();

            changes.ShouldBeEquivalentTo(new[]
            {
                new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemRemoved, null)
                {
                    OldItem = removedItem
                }
            });
        }

        [TestMethod]
        public void ShouldDetectRemovedBlockItemChangeTest()
        {
            var removedItem = new VersionedDocumentBlockItemDescriptor("subp3", VersionedDocumentBlockItemKind.Integer);
            var oldDesc = new VersionedDocumentDescriptor(new []{new VersionedDocumentBlockDescriptor("p1", new[]
            {
                removedItem,
                new VersionedDocumentBlockItemDescriptor("subp1", VersionedDocumentBlockItemKind.Integer),
                new VersionedDocumentBlockItemDescriptor("subp2", VersionedDocumentBlockItemKind.Integer)
            })});

            var container = new VersionedDocumentBlockDescriptor("p1", new[]
            {
                new VersionedDocumentBlockItemDescriptor("subp1", VersionedDocumentBlockItemKind.Integer),
                new VersionedDocumentBlockItemDescriptor("subp2", VersionedDocumentBlockItemKind.Integer)
            });

            var newDesc = new VersionedDocumentDescriptor()
            {
                Blocks =
                {
                    container
                }
            };

            IVersionedDocumentDescriptorService service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            var changes = service.GetChanges(oldDesc, newDesc);

            changes.ShouldBeEquivalentTo(new[]
            {
                new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemRemoved, new []{container})
                {
                    OldItem = removedItem
                }
            });
        }

        [TestMethod]
        public void ShouldDetectBlockTypeStringToIntChangeTest()
        {
            var item1 = new VersionedDocumentBlockDescriptor("p2", VersionedDocumentBlockItemKind.String);
            var oldDesc = new VersionedDocumentDescriptor(new[] {
                item1,
                new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String)});

            var item2 = new VersionedDocumentBlockDescriptor("p2", VersionedDocumentBlockItemKind.Integer);
            var newDesc = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String),
                item2});

            IVersionedDocumentDescriptorService service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            var changes = service.GetChanges(oldDesc, newDesc).ToList();

            changes.ShouldBeEquivalentTo(new[]
            {
                new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemTypeChanged, null)
                {
                    OldItem = item1,
                    NewItem = item2
                }
            }, o => o.ComparingByValue<IVersionedDocumentBlockItemDescriptor>());
        }

        [TestMethod]
        public void ShouldDetectBlockObjectItemTypeStringToIntChangeTest()
        {
            var item1 = new VersionedDocumentBlockItemDescriptor("p2", VersionedDocumentBlockItemKind.String);
            var oldDesc = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String),
                new VersionedDocumentBlockDescriptor("p2", new[] { item1 })});

            var item2 = new VersionedDocumentBlockItemDescriptor("p2", VersionedDocumentBlockItemKind.Integer);
            var container = new VersionedDocumentBlockDescriptor("p2", new[] { item2 });
            var newDesc = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("p1", VersionedDocumentBlockItemKind.String),
                container
            });

            IVersionedDocumentDescriptorService service = new VersionedDocumentDescriptorService(new ContainerBuilder().Build());

            var changes = service.GetChanges(oldDesc, newDesc).ToList();

            changes.ShouldBeEquivalentTo(new[]
            {
                new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemTypeChanged, new []{container})
                {
                    OldItem = item1,
                    NewItem = item2
                }
            }, o => o.ComparingByValue<IVersionedDocumentBlockItemDescriptor>());
        }
    }
}