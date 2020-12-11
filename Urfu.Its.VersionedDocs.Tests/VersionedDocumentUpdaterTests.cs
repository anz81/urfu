using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;
using Urfu.Its.VersionedDocs.Loggers;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Tests
{
    [TestClass]
    public class VersionedDocumentUpdaterTests
    {
        [TestMethod]
        public void ShouldUpdateDocumentsWhenStringBlockAdded()
        {
            var template = PrepareDoc1Template();

            var newBlockDescriptor = new VersionedDocumentBlockDescriptor("NewBlock", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null);
            var newDocDescriptor = CreateDoc1Descriptor();
            newDocDescriptor.Blocks.Add(newBlockDescriptor);

            var descriptorServiceMock = new Mock<IVersionedDocumentDescriptorService>();
            descriptorServiceMock.Setup(d=>d.GetChanges(It.IsAny<VersionedDocumentDescriptor>(), It.IsAny<VersionedDocumentDescriptor>()))
                .Returns(new[]{ new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemAdded, Enumerable.Empty<IVersionedDocumentBlockItemDescriptor>())
                {
                    NewItem = newBlockDescriptor
                }});
            
            var updater = BuildUpdater(descriptorServiceMock);
            updater.UpdateLinkedDocumentsForTemplate(template, newDocDescriptor, out var errors, DateTime.Now);

            Assert.AreEqual(0, errors.Count);

            foreach (var document in template.Documents)
            {
                Assert.AreEqual(document.BlockLinks.Count, 3);
                var newBlock = document.BlockLinks.ElementAt(2).DocumentBlock;
                Assert.AreEqual("NewBlock", newBlock.Name);
                var actualData = JToken.Parse(newBlock.Data);
                var expectedData = JToken.Parse(BlockDataHelper.PrepareData(null));
                Assert.IsTrue(JToken.DeepEquals(actualData, expectedData));
            }
        }

        private static VersionedDocumentUpdater BuildUpdater(Mock<IVersionedDocumentDescriptorService> descriptorServiceMock)
        {
            var schemaService = new VersionedDocumentSchemaService();
            return new VersionedDocumentUpdater(descriptorServiceMock.Object, new VersionedDocumentsTraceLogger<VersionedDocumentUpdater>(), new FromDatabaseVersionedDocumentService(schemaService), schemaService);
        }

        [TestMethod]
        public void ShouldUpdateDocumentsWhenObjectBlockAddedWithDefaultStructureContent()
        {
            var template = PrepareDoc1Template();

            var newBlockDescriptor = new VersionedDocumentBlockDescriptor("NewBlock", new[]
            {
                new VersionedDocumentBlockItemDescriptor("NewBlockProp", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)
            });
            var newDocDescriptor = CreateDoc1Descriptor();
            newDocDescriptor.Blocks.Add(newBlockDescriptor);

            var descriptorServiceMock = new Mock<IVersionedDocumentDescriptorService>();
            descriptorServiceMock.Setup(d => d.GetChanges(It.IsAny<VersionedDocumentDescriptor>(), It.IsAny<VersionedDocumentDescriptor>()))
                .Returns(new[]{ new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemAdded, Enumerable.Empty<IVersionedDocumentBlockItemDescriptor>())
                {
                    NewItem = newBlockDescriptor
                }});
            descriptorServiceMock.Setup(d => d.BuildDefaultContent(It.IsAny<IVersionedDocumentBlockItemDescriptor>()))
                .Returns<IVersionedDocumentBlockItemDescriptor>(d =>
                    new StructuralContentBuilder(d).BuildDefaultContent());

            var updater = BuildUpdater(descriptorServiceMock);
            updater.UpdateLinkedDocumentsForTemplate(template, newDocDescriptor, out var errors, DateTime.Now);

            Assert.AreEqual(0, errors.Count);

            foreach (var document in template.Documents)
            {
                Assert.AreEqual(document.BlockLinks.Count, 3);
                var newBlock = document.BlockLinks.ElementAt(2).DocumentBlock;
                Assert.AreEqual("NewBlock", newBlock.Name);
                var expectedData = JToken.Parse(BlockDataHelper.PrepareData(new { NewBlockProp = (string) null}));
                var blockData = JToken.Parse(newBlock.Data);
                Assert.IsTrue(JToken.DeepEquals(blockData, expectedData));
            }
        }

        [TestMethod]
        public void ShouldUpdateDocumentsWithCollectionsWhenCollectionItemAddedWithDefaultStructureContent()
        {
            var template = PrepareDocWithCollectionTemplate();

            var newItemDescriptor = new VersionedDocumentBlockItemDescriptor("NewItem", new[]
            {
                new VersionedDocumentBlockItemDescriptor("NewItemProp", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)
            });
            var newDocDescriptor = CreateDocWithCollectionDescriptor();
            var containerDescriptor = newDocDescriptor.Blocks.ElementAt(0);
            containerDescriptor.Items.Properties.Add(newItemDescriptor);

            var descriptorServiceMock = new Mock<IVersionedDocumentDescriptorService>();
            descriptorServiceMock.Setup(d => d.GetChanges(It.IsAny<VersionedDocumentDescriptor>(), It.IsAny<VersionedDocumentDescriptor>()))
                .Returns(new[]{ new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemAdded, new []{containerDescriptor})
                {
                    NewItem = newItemDescriptor
                }});
            descriptorServiceMock.Setup(d => d.BuildDefaultContent(It.IsAny<IVersionedDocumentBlockItemDescriptor>()))
                .Returns<IVersionedDocumentBlockItemDescriptor>(d =>
                    new StructuralContentBuilder(d).BuildDefaultContent());

            var updater = BuildUpdater(descriptorServiceMock);
            updater.UpdateLinkedDocumentsForTemplate(template, newDocDescriptor, out var errors, DateTime.Now);

            Assert.AreEqual(0, errors.Count);

            foreach (var document in template.Documents)
            {
                Assert.AreEqual(document.BlockLinks.Count, 1);
                var newBlock = document.BlockLinks.ElementAt(0).DocumentBlock;

                var collectionArray = (JArray) newBlock.GetContent();
                Assert.AreEqual(1, collectionArray.Count);
                foreach (JObject item in collectionArray)
                {
                    Assert.IsTrue(JToken.DeepEquals(item, new JObject(
                        item.Property("IntProp"), 
                        new JProperty("NewItem", new JObject(new JProperty("NewItemProp", null))))));
                }                
            }
        }

        [TestMethod]
        public void ShouldUpdateDocumentsWithSubCollectionsWhenSubCollectionItemAddedWithDefaultStructureContent()
        {
            var template = PrepareDocWithSubCollectionTemplate();

            var newItemDescriptor = new VersionedDocumentBlockItemDescriptor("NewSubItem", new[]
            {
                new VersionedDocumentBlockItemDescriptor("NewSubItemProp", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)
            });
            var newDocDescriptor = CreateDocWithSubCollectionDescriptor();
            var containerDescriptor = newDocDescriptor.Blocks.ElementAt(0);
            containerDescriptor.Items.Properties.First(p=>p.Name == "SubCollection")
                .Items.Properties.Add(newItemDescriptor);

            var descriptorServiceMock = new Mock<IVersionedDocumentDescriptorService>();
            descriptorServiceMock.Setup(d => d.GetChanges(It.IsAny<VersionedDocumentDescriptor>(), newDocDescriptor))
                .Returns(new[]{ new VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction.ItemAdded, 
                    new IVersionedDocumentBlockItemDescriptor[]
                    {
                        containerDescriptor,
                        containerDescriptor.Items.Properties.First(p=>p.Name == "SubCollection"),                    
                    })
                    {
                        NewItem = newItemDescriptor
                    }});
            descriptorServiceMock.Setup(d => d.BuildDefaultContent(It.IsAny<IVersionedDocumentBlockItemDescriptor>()))
                .Returns<IVersionedDocumentBlockItemDescriptor>(d =>
                    new StructuralContentBuilder(d).BuildDefaultContent());

            var updater = BuildUpdater(descriptorServiceMock);
            updater.UpdateLinkedDocumentsForTemplate(template, newDocDescriptor, out var errors, DateTime.Now);

            Assert.AreEqual(0, errors.Count);

            foreach (var document in template.Documents)
            {
                Assert.AreEqual(document.BlockLinks.Count, 1);
                var newBlock = document.BlockLinks.ElementAt(0).DocumentBlock;

                var collectionToken = (JArray) newBlock.GetContent();
                Assert.AreEqual(1, collectionToken.Count);
                foreach (JObject item in collectionToken)
                {
                    var subCollectionToken = item.Property("SubCollection");
                    foreach (JObject subItem in subCollectionToken.Value)
                    {
                        Assert.IsTrue(JToken.DeepEquals(subItem, new JObject(
                            subItem.Property("IntSubProp"),
                            new JProperty("NewSubItem", new JObject(new JProperty("NewSubItemProp", null))))));
                    }                    
                }
            }
        }

        private static VersionedDocumentDescriptor CreateDoc1Descriptor()
        {
            return new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null),
                new VersionedDocumentBlockDescriptor("Info", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)
                })});
        }

        private static VersionedDocumentDescriptor CreateDocWithCollectionDescriptor()
        {
            return new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Collection", 
                    new VersionedDocumentBlockItemDescriptor(new[]
                    {
                        new VersionedDocumentBlockItemDescriptor("IntProp", VersionedDocumentBlockItemKind.Integer)
                    })
                )});
        }
        
        private static VersionedDocumentDescriptor CreateDocWithSubCollectionDescriptor()
        {
            return new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Collection", new VersionedDocumentBlockItemDescriptor(new []
                {
                    new VersionedDocumentBlockItemDescriptor("IntProp", VersionedDocumentBlockItemKind.Integer),
                    new VersionedDocumentBlockItemDescriptor("SubCollection", new VersionedDocumentBlockItemDescriptor(new[]
                    {
                        new VersionedDocumentBlockItemDescriptor("IntSubProp", VersionedDocumentBlockItemKind.Integer)
                    }))
                }))});
        }

        private VersionedDocumentTemplate PrepareDoc1Template()
        {
            var docs = new List<VersionedDocument>
            {
                PrepareDoc(JObject.FromObject(new
                {
                    Annotation = (string) null,
                    Info = new
                    {
                        Name = (string) null
                    }
                })),
                PrepareDoc(JObject.FromObject(new
                {
                    Annotation = "Annotation1",
                    Info = new
                    {
                        Name = "Name1"
                    }
                }))
            };

            var template = new VersionedDocumentTemplate
            {
                DocumentType = VersionedDocumentType.ModuleWorkingProgram,
                CreatedAt = DateTime.Now,
                Id = ++_lastTemplateId,
                Schema = JSchema.Parse(File.ReadAllText("TestData/doc1.jschema")).ToString(),
                Documents = docs
            };

            foreach (var document in template.Documents)
            {
                document.Template = template;
                document.TemplateId = template.Id;
            }

            return template;
        }

        private VersionedDocumentTemplate PrepareDocWithCollectionTemplate()
        {
            var docs = new List<VersionedDocument>
            {
                PrepareDoc(JObject.FromObject(new
                {
                    Collection = new object[]
                    {
                        new { IntProp = 1 }
                    }
                })),
                PrepareDoc(JObject.FromObject(new
                {
                    Collection = new object[]
                    {
                        new { IntProp = 2 }
                    }
                }))
            };

            var template = new VersionedDocumentTemplate
            {
                DocumentType = VersionedDocumentType.ModuleWorkingProgram,
                CreatedAt = DateTime.Now,
                Id = ++_lastTemplateId,
                Schema = JSchema.Parse(File.ReadAllText("TestData/docWithCollection.jschema")).ToString(),
                Documents = docs
            };

            foreach (var document in template.Documents)
            {
                document.Template = template;
                document.TemplateId = template.Id;
            }

            return template;
        }

        private VersionedDocumentTemplate PrepareDocWithSubCollectionTemplate()
        {
            var docs = new List<VersionedDocument>
            {
                PrepareDoc(JObject.FromObject(new
                {
                    Collection = new object[]
                    {
                        new
                        {
                            IntProp = 1,
                            SubCollection = new object[]
                            {
                                new { IntSubProp = 1 }
                            }
                        },                        
                    }
                })),
                PrepareDoc(JObject.FromObject(new
                {
                    Collection = new object[]
                    {
                        new
                        {
                            IntProp = 2,
                            SubCollection = new object[]
                            {
                                new { IntSubProp = 2 }
                            }
                        }
                    }
                }))
            };

            var template = new VersionedDocumentTemplate
            {
                DocumentType = VersionedDocumentType.ModuleWorkingProgram,
                CreatedAt = DateTime.Now,
                Id = ++_lastTemplateId,
                Schema = JSchema.Parse(File.ReadAllText("TestData/docWithCollection.jschema")).ToString(),
                Documents = docs
            };

            foreach (var document in template.Documents)
            {
                document.Template = template;
                document.TemplateId = template.Id;
            }

            return template;
        }

        private int _lastTemplateId = 0;
        private int _lastDocumentId = 0;
        private int _lastLinkId = 0;
        private int _lastBlockId = 0;

        private VersionedDocument PrepareDoc(JObject data)
        {
            var doc = new VersionedDocument
            {
                Id = ++_lastDocumentId,
                BlockLinks = new List<VersionedDocumentBlockLink>()
            };

            foreach (var property in data.Properties())
            {
                var link = new VersionedDocumentBlockLink
                {
                    Id = ++_lastLinkId,
                    UpdateTime = DateTime.Now,
                    DocumentBlock = new VersionedDocumentBlock
                    {
                        Id = ++_lastBlockId,
                        CreatedAt = DateTime.Now,
                        Name = property.Name,
                        Data = BlockDataHelper.PrepareData(property.Value),
                        Links = new List<VersionedDocumentBlockLink>()
                    },
                    Document = doc,
                    DocumentId = doc.Id
                };

                link.DocumentBlockId = link.DocumentBlock.Id;
                link.DocumentBlock.Links.Add(link);

                doc.BlockLinks.Add(link);
            }            

            return doc;
        }
    }
}
