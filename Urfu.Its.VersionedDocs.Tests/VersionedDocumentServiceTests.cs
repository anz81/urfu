using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Features.Indexed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TemplateEngine;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Loggers;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Tests
{
    [TestClass]
    public class VersionedDocumentServiceTests
    {
        [TestMethod]
        public void ShouldIncrementChangedBlocksVersionsOnApplyDocumentChanges()
        {
            var descriptor = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String),
                new VersionedDocumentBlockDescriptor("Info", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String)
                }),
                new VersionedDocumentBlockDescriptor("Authors", new VersionedDocumentBlockItemDescriptor(new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Post", VersionedDocumentBlockItemKind.String),
                    new VersionedDocumentBlockItemDescriptor("Fio", VersionedDocumentBlockItemKind.String)
                }))});

            var initialTime = DateTime.Now;
            var document = new VersionedDocument
            {
                Template = new VersionedDocumentTemplate
                {
                    Schema = descriptor.GenerateSchemaString(),
                    DocumentType = VersionedDocumentType.ModuleWorkingProgram
                },
                BlockLinks = new List<VersionedDocumentBlockLink>()
                {
                    new VersionedDocumentBlockLink
                    {       
                        UpdateTime = initialTime,
                        DocumentBlockId = 1,
                        DocumentBlock = new VersionedDocumentBlock
                        {
                            Id = 1,
                            Name = "Annotation",
                            Data = BlockDataHelper.PrepareData("InitialAnnotation"),
                            Version = 0
                        }                        
                    },
                    new VersionedDocumentBlockLink
                    {                        
                        UpdateTime = initialTime,
                        DocumentBlockId = 2,
                        DocumentBlock = new VersionedDocumentBlock
                        {
                            Id = 2,
                            Name = "Info",
                            Data = BlockDataHelper.PrepareData(new { Name = "InitialName"}),
                            Version = 0
                        }
                    },
                    new VersionedDocumentBlockLink
                    {
                        UpdateTime = initialTime,
                        DocumentBlockId = 3,
                        DocumentBlock = new VersionedDocumentBlock
                        {
                            Id = 3,
                            Name = "Authors",
                            Data = BlockDataHelper.PrepareData(JToken.Parse("[{\"Post\":\"InitialPost\", \"Fio\": \"InitialFio\"}]")),
                            Version = 0
                        }
                    }
                }
            };
            foreach (var link in document.BlockLinks)
                link.DocumentBlock.Links = new List<VersionedDocumentBlockLink> {link};

            var builder = new ContainerBuilder();
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var schemaService = new VersionedDocumentSchemaService();
                var service = CreateVersionedDocumentService(scope, descriptor, (b, d) => { }, schemaService);
                service.ApplyDocumentChanges(document, @"{""Annotation"": ""ChangedAnnotation"", ""Authors"":[{""Post"":""InitialPost"", ""Fio"": ""ChangedFio""}]}");

                Assert.AreEqual(document.BlockLinks.ElementAt(0).DocumentBlock.PreviousBlockId, 1);
                Assert.AreEqual(document.BlockLinks.ElementAt(0).DocumentBlock.Version, 1);
                Assert.AreNotEqual(document.BlockLinks.ElementAt(0).UpdateTime, initialTime);

                Assert.IsNull(document.BlockLinks.ElementAt(1).DocumentBlock.PreviousBlockId);
                Assert.AreEqual(document.BlockLinks.ElementAt(1).DocumentBlock.Version, 0);
                Assert.AreEqual(document.BlockLinks.ElementAt(1).UpdateTime, initialTime);

                Assert.AreEqual(document.BlockLinks.ElementAt(2).DocumentBlock.PreviousBlockId, 3);
                Assert.AreEqual(document.BlockLinks.ElementAt(2).DocumentBlock.Version, 1);
                Assert.AreNotEqual(document.BlockLinks.ElementAt(2).UpdateTime, initialTime);
            }
        }

        [TestMethod]
        public void ShouldRemoveIncompatibleObjectsPropertiesOnApplyDocumentChanges()
        {
            var descriptor = new VersionedDocumentDescriptor
            {
                Blocks =
                {
                    new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String),
                    new VersionedDocumentBlockDescriptor("Info", new[]
                    {
                        new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String)
                    }),
                    new VersionedDocumentBlockDescriptor("Authors", new VersionedDocumentBlockItemDescriptor(new []
                    {
                        new VersionedDocumentBlockItemDescriptor("Id", VersionedDocumentBlockItemKind.Integer),
                        new VersionedDocumentBlockItemDescriptor("Post", VersionedDocumentBlockItemKind.String),
                        new VersionedDocumentBlockItemDescriptor("Degree", VersionedDocumentBlockItemKind.String),
                        new VersionedDocumentBlockItemDescriptor("Fio", VersionedDocumentBlockItemKind.String),
                        new VersionedDocumentBlockItemDescriptor("TeacherId", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null),
                        new VersionedDocumentBlockItemDescriptor("AuthorId", VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null)
                    })),
                    new VersionedDocumentBlockDescriptor("ArrayWithObjectHierarchy", new VersionedDocumentBlockItemDescriptor(new[]
                    {
                        new VersionedDocumentBlockItemDescriptor("InnerObject", new[]
                        {
                            new VersionedDocumentBlockItemDescriptor("Property", VersionedDocumentBlockItemKind.Integer)
                        })
                    }))
                }
            };

            var doc2Empty = JObject.Parse(File.ReadAllText("TestData/doc2-empty.json"));

            var document = new VersionedDocument
            {
                Template = new VersionedDocumentTemplate()
                {
                    Schema = File.ReadAllText("TestData/doc2.jschema"),
                    DocumentType = VersionedDocumentType.ModuleWorkingProgram
                },
                BlockLinks = new List<VersionedDocumentBlockLink>()
            };
            foreach (var property in doc2Empty.Properties())
            {
                document.BlockLinks.Add(new VersionedDocumentBlockLink
                {
                    DocumentBlock = new VersionedDocumentBlock
                    {
                        Name = property.Name,
                        Data = BlockDataHelper.PrepareData(property.Value)
                    }
                });
                foreach (var link in document.BlockLinks)
                    link.DocumentBlock.Links = new List<VersionedDocumentBlockLink> { link };
            }            

            var builder = new ContainerBuilder();
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var schemaService = new VersionedDocumentSchemaService();
                var service = CreateVersionedDocumentService(scope, descriptor, (b, d) => { },
                    schemaService: schemaService);                
                service.ApplyDocumentChanges(document, File.ReadAllText("TestData/doc2-client.json"));
                var actualData = new JObject();
                foreach (var block in document.BlockLinks.Select(b => b.DocumentBlock))
                    actualData.Merge(new JObject(new JProperty(block.Name, block.GetContent())));

                Assert.IsTrue(JToken.DeepEquals(actualData, JObject.Parse(File.ReadAllText("TestData/doc2.json"))));
            }            
        }

        [TestMethod]
        public void GenerateSerializedModelWithLoaderTest()
        {
            var expectedData = JObject.Parse(File.ReadAllText("TestData/doc1.json"));
            
            string moduleName = expectedData["Info"]["Name"].Value<string>();

            var versionedDocumentDescriptor = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String),
                new VersionedDocumentBlockDescriptor("Info", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String)
                })
                {
                    NonVersionedDataLoaderType = typeof(TestModuleInfoDataLoader)
                }});

            var moduleWorkingProgram = GetDoc1ModuleWorkingProgram(
                expectedData["Annotation"].Value<string>(), 
                (JObject) expectedData["Info"], 
                moduleName, 
                versionedDocumentDescriptor.GenerateSchema().ToString());

            var builder = new ContainerBuilder();
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var service = CreateVersionedDocumentService(scope, versionedDocumentDescriptor, (b, d) => { b.RegisterInstance(moduleWorkingProgram); });
                var model = service.CreateSerializedModel(moduleWorkingProgram.VersionedDocument);
                var expected = expectedData.ToString(Formatting.Indented);
                Assert.AreEqual(expected, model);
            }
        }

        [TestMethod]
        public void GenerateSerializedModelWithLoaderForOldDocumentVarsionTest()
        {
            var expectedData = JObject.Parse(File.ReadAllText("TestData/doc1.json"));

            string moduleName = expectedData["Info"]["Name"].Value<string>();

            var moduleWorkingProgram = GetDoc1ModuleWorkingProgram(
                expectedData["Annotation"].Value<string>(), 
                (JObject)expectedData["Info"], 
                moduleName, 
                File.ReadAllText("TestData/doc1.jschema"));

            // Текущий десткриптор не соответсвует схеме документа
            var versionedDocumentDescriptor = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Info", VersionedDocumentBlockItemKind.String)});

            var builder = new ContainerBuilder();
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var service = CreateVersionedDocumentService(scope, versionedDocumentDescriptor, (b, d) => { b.RegisterInstance(moduleWorkingProgram); }, inEditableState: false);
                var model = service.CreateSerializedModel(moduleWorkingProgram.VersionedDocument);
                var expected = expectedData.ToString(Formatting.Indented);
                Assert.AreEqual(expected, model);
            }
        }

        [TestMethod]
        public void GeneratePartialSerializedModelTest()
        {
            var expectedData = JObject.Parse(File.ReadAllText("TestData/doc1.json"));

            string moduleName = expectedData["Info"]["Name"].Value<string>();

            var moduleWorkingProgram = GetDoc1ModuleWorkingProgram(
                expectedData["Annotation"].Value<string>(), 
                (JObject)expectedData["Info"], 
                moduleName, 
                File.ReadAllText("TestData/doc1.jschema"));

            var versionedDocumentDescriptor = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null),
                new VersionedDocumentBlockDescriptor("Info", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)
                })
                {
                    NonVersionedDataLoaderType = typeof(TestModuleInfoDataLoader)
                }});

            var builder = new ContainerBuilder();
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var service = CreateVersionedDocumentService(scope, versionedDocumentDescriptor, (b, d) => { b.RegisterInstance(moduleWorkingProgram); });
                var model = service.CreateSerializedModel(moduleWorkingProgram.VersionedDocument, "Annotation");
                var jModel = JObject.Parse(model);
                Assert.AreEqual(1, jModel.Properties().Count());
                Assert.AreEqual(jModel["Annotation"].Value<string>(), expectedData["Annotation"].Value<string>());
            }
        }

        [TestMethod]
        public void ShouldSaveDependentBlock()
        {
            var moduleWorkingProgram = GetDoc1ModuleWorkingProgram(
                null, 
                new JObject(new JProperty("Name", null)), 
                null, 
                File.ReadAllText("TestData/doc1.jschema"));

            var versionedDocumentDescriptor = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)
                {
                    DependentBlocks = new[] { "Info" }
                },
                new VersionedDocumentBlockDescriptor("Info", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)
                })
                {
                    ProcessorType = typeof(TestNameToInfoProcessor)
                }});

            var container = ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var schemaService = new VersionedDocumentSchemaService();
                var service = CreateVersionedDocumentService(scope, versionedDocumentDescriptor, (b, d) => { }, schemaService: schemaService);

                service.ApplyDocumentChanges(moduleWorkingProgram.VersionedDocument, @"{""Annotation"": ""TestName""}");

                var actualData = new JObject();
                foreach (var block in moduleWorkingProgram.VersionedDocument.BlockLinks.Select(b => b.DocumentBlock))
                    actualData.Merge(new JObject(new JProperty(block.Name, block.GetContent())));

                Assert.AreEqual("TestName", actualData["Annotation"].Value<string>());
                Assert.AreEqual("TestName", actualData["Info"]["Name"].Value<string>());
            }
        }

        private static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(VersionedDocumentsLogger<>)).As(typeof(IObjectLogger<>));

            builder.RegisterType<LoggingVersionedDocumentInspector>()
                .As<IVersionedDocumentInspector>()
                .InstancePerLifetimeScope();

            var container = builder.Build();
            return container;
        }

        [TestMethod]
        public void ShouldSaveTwoLevelDependentBlocks()
        {
            var moduleWorkingProgram = GetDoc3ModuleWorkingProgram(
                null,
                new JObject(new JProperty("Name", null)),
                new JObject(new JProperty("Name", null)),
                null,
                File.ReadAllText("TestData/doc3.jschema"));

            var versionedDocumentDescriptor = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Info2", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String)
                })
                {
                    ProcessorType = typeof(TestInfoNameToInfo2NameProcessor)
                },
                new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String)
                {
                    DependentBlocks = new[] { "Info" }
                },
                new VersionedDocumentBlockDescriptor("Info", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String)
                })
                {
                    DependentBlocks = new[] { "Info2" },                    
                    ProcessorType = typeof(TestNameToInfoProcessor)
                }});

            var container = ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var schemaService = new VersionedDocumentSchemaService();
                var service = CreateVersionedDocumentService(scope, versionedDocumentDescriptor, (b, d) => { }, schemaService: schemaService);

                service.ApplyDocumentChanges(moduleWorkingProgram.VersionedDocument, new JObject(new JProperty("Annotation", "TestName")).ToString());

                var actualData = new JObject();
                foreach (var block in moduleWorkingProgram.VersionedDocument.BlockLinks.Select(b => b.DocumentBlock))
                    actualData.Merge(new JObject(new JProperty(block.Name, block.GetContent())));

                Assert.AreEqual("TestName", actualData["Annotation"].Value<string>());
                Assert.AreEqual("TestName", actualData["Info"]["Name"].Value<string>());
                Assert.AreEqual("TestName", actualData["Info2"]["Name"].Value<string>());
            }
        }

        [TestMethod]
        public void ShouldLoadTwoLevelDependentBlocks()
        {
            var moduleWorkingProgram = GetDoc3ModuleWorkingProgram(
                "TestName",
                new JObject(new JProperty("Name", null)),
                new JObject(new JProperty("Name", null)),
                null,
                File.ReadAllText("TestData/doc3.jschema"));

            var versionedDocumentDescriptor = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Info", new []
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String)
                })
                {
                    DependentBlocks = new[] { "Info2" },
                    NonVersionedDataLoaderType = typeof(TestInfoNameFromAnnotationLoader)
                },
                new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String)
                {
                    DependentBlocks = new[] { "Info" }
                },
                new VersionedDocumentBlockDescriptor("Info2", new[]
                {
                    new VersionedDocumentBlockItemDescriptor("Name", VersionedDocumentBlockItemKind.String)
                })
                {
                    NonVersionedDataLoaderType = typeof(TestInfo2FromInfoLoader)
                }});

            var builder = new ContainerBuilder();
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var schemaService = new VersionedDocumentSchemaService();
                var service = CreateVersionedDocumentService(scope, versionedDocumentDescriptor, (b, d) => { }, schemaService: schemaService);

                dynamic model = service.CreateProxyModel(moduleWorkingProgram.VersionedDocument, "Annotation");
                Assert.AreEqual("TestName", model.Annotation);
                Assert.IsNull(model.Info);
                Assert.IsNull(model.Info2);

                dynamic model2 = service.CreateProxyModel(moduleWorkingProgram.VersionedDocument, "Info");
                Assert.AreEqual("TestName", model2.Annotation);
                Assert.AreEqual("TestName", model2.Info.Name);
                Assert.IsNull(model2.Info2);

                dynamic model3 = service.CreateProxyModel(moduleWorkingProgram.VersionedDocument, "Info2");
                Assert.AreEqual("TestName", model3.Annotation);
                Assert.AreEqual("TestName", model3.Info.Name);
                Assert.AreEqual("TestName", model3.Info2.Name);

                dynamic model4 = service.CreateProxyModel(moduleWorkingProgram.VersionedDocument);
                Assert.AreEqual("TestName", model4.Annotation);
                Assert.AreEqual("TestName", model4.Info.Name);
                Assert.AreEqual("TestName", model4.Info2.Name);
            }
        }

        private static IVersionedDocumentService CreateVersionedDocumentService(
            ILifetimeScope scope, 
            VersionedDocumentDescriptor versionedDocumentDescriptor, 
            Action<ContainerBuilder, VersionedDocument> dependencyRegistrations, 
            IVersionedDocumentSchemaService schemaService = null,
            bool inEditableState = true)
        {
            var templateEngine = new Mock<ITemplateReportingEngine>();
            if (schemaService == null)
                schemaService = new Mock<IVersionedDocumentSchemaService>().Object;            

            var documentImplementationService = new Mock<IVersionedDocumentImplementationService>();
            documentImplementationService.Setup(s => s.RegisterDocumentDependencies(It.IsAny<ContainerBuilder>(), It.IsAny<VersionedDocument>())).Callback(dependencyRegistrations);
            documentImplementationService.Setup(s => s.GetDocumentDescriptor()).Returns(versionedDocumentDescriptor);
            documentImplementationService.Setup(s => s.IsInEditableState(It.IsAny<VersionedDocument>())).Returns(inEditableState);

            var documentImplementationServiceIndex = new Mock<IIndex<VersionedDocumentType, IVersionedDocumentImplementationService>>();
            documentImplementationServiceIndex.Setup(d => d[VersionedDocumentType.ModuleWorkingProgram]).Returns(documentImplementationService.Object);

            var logger = new Mock<IObjectLogger<VersionedDocumentService>>();
            logger.Setup(l => l.Error(It.IsAny<string>(), It.IsAny<object[]>())).Callback<string, object[]>((m,a)=>Trace.WriteLine(string.Format(m,a)));
            logger.Setup(l => l.Error(It.IsAny<Exception>())).Callback<Exception>(e=>Trace.WriteLine(e));
            logger.Setup(l => l.Warning(It.IsAny<string>(), It.IsAny<object[]>())).Callback<string, object[]>((m, a) => Trace.WriteLine(string.Format(m, a)));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>())).Callback<string, object[]>((m, a) => Trace.WriteLine(string.Format(m, a)));
            logger.Setup(l => l.Debug(It.IsAny<string>(), It.IsAny<object[]>())).Callback<string, object[]>((m, a) => Trace.WriteLine(string.Format(m, a)));            

            var service = new VersionedDocumentService(templateEngine.Object, schemaService, documentImplementationServiceIndex.Object, scope, logger.Object);
            return service;
        }

        private static ModuleWorkingProgram GetDoc1ModuleWorkingProgram(string annotation, JObject info, string moduleName, string templateSchema)
        {
            var template = new VersionedDocumentTemplate
            {
                DocumentType = VersionedDocumentType.ModuleWorkingProgram,
                Schema = templateSchema
            };
            var document = new VersionedDocument
            {
                Template = template
            };
            document.BlockLinks = new List<VersionedDocumentBlockLink>
            {
                new VersionedDocumentBlockLink
                {
                    Document = document,
                    DocumentBlock = new VersionedDocumentBlock
                    {      
                        Name = "Annotation",
                        Data = BlockDataHelper.PrepareData(annotation)
                    }
                },
                new VersionedDocumentBlockLink
                {
                    Document = document,
                    DocumentBlock = new VersionedDocumentBlock
                    {                       
                        Name = "Info",
                        Data = BlockDataHelper.PrepareData(info)
                    }
                }
            };
            foreach (var link in document.BlockLinks)
                link.DocumentBlock.Links = new List<VersionedDocumentBlockLink> { link };
            var moduleWorkingProgram = new ModuleWorkingProgram
            {
                VersionedDocument = document,
                Module = new Web.DataContext.Module
                {
                    title = moduleName
                }
            };
            return moduleWorkingProgram;
        }

        private static ModuleWorkingProgram GetDoc3ModuleWorkingProgram(string annotation, JObject info, JObject info2, string moduleName, string templateSchema)
        {
            var template = new VersionedDocumentTemplate
            {
                DocumentType = VersionedDocumentType.ModuleWorkingProgram,
                Schema = templateSchema
            };
            var document = new VersionedDocument
            {
                Template = template
            };
            document.BlockLinks = new List<VersionedDocumentBlockLink>
            {
                new VersionedDocumentBlockLink
                {
                    Document = document,
                    DocumentBlock = new VersionedDocumentBlock
                    {
                        Name = "Annotation",
                        Data = BlockDataHelper.PrepareData(annotation)
                    }
                },
                new VersionedDocumentBlockLink
                {
                    Document = document,
                    DocumentBlock = new VersionedDocumentBlock
                    {
                        Name = "Info",
                        Data = BlockDataHelper.PrepareData(info)
                    }
                },                
                new VersionedDocumentBlockLink
                {
                    Document = document,
                    DocumentBlock = new VersionedDocumentBlock
                    {
                        Name = "Info2",
                        Data = BlockDataHelper.PrepareData(info2)
                    }
                }
            };
            foreach (var link in document.BlockLinks)
                link.DocumentBlock.Links = new List<VersionedDocumentBlockLink> { link };
            var moduleWorkingProgram = new ModuleWorkingProgram
            {
                VersionedDocument = document,
                Module = new Web.DataContext.Module
                {
                    title = moduleName
                }
            };
            return moduleWorkingProgram;
        }

        private class TestModuleInfoDataLoader : ObjectBlockContentLoader<object>
        {
            private readonly ModuleWorkingProgram _workingProgram;

            public TestModuleInfoDataLoader(ModuleWorkingProgram workingProgram)
            {
                _workingProgram = workingProgram;
            }

            protected override object LoadAnyContent(JToken blockContent)
            {
                return new
                {
                    Name = _workingProgram.Module.title
                };
            }
        }

        private class TestNameToInfoProcessor : IBlockContentProcessor
        {
            private readonly JObject _actualDocumentData;

            public TestNameToInfoProcessor(JObject actualDocumentData)
            {
                _actualDocumentData = actualDocumentData;
            }

            public JToken ProcessContent(JToken data)
            {
                var name = _actualDocumentData["Annotation"].Value<string>();
                return new JObject(new JProperty("Name", name));
            }
        }

        private class TestInfoNameToInfo2NameProcessor : IBlockContentProcessor
        {
            private readonly JObject _actualDocumentData;

            public TestInfoNameToInfo2NameProcessor(JObject actualDocumentData)
            {
                _actualDocumentData = actualDocumentData;
            }

            public JToken ProcessContent(JToken data)
            {
                var name = _actualDocumentData["Info"]["Name"].Value<string>();
                return new JObject(new JProperty("Name", name));
            }
        }

        private class TestInfoNameFromAnnotationLoader : ObjectBlockContentLoader<JObject>
        {
            private readonly JObject _loadedDocumentData;

            public TestInfoNameFromAnnotationLoader(JObject loadedDocumentData)
            {
                _loadedDocumentData = loadedDocumentData;
            }

            protected override JObject LoadAnyContent(JToken blockContent)
            {
                return new JObject(new JProperty("Name", _loadedDocumentData["Annotation"].Value<string>()));
            }
        }

        private class TestInfo2FromInfoLoader : ObjectBlockContentLoader<JObject>
        {
            private readonly JObject _loadedDocumentData;

            public TestInfo2FromInfoLoader(JObject loadedDocumentData)
            {
                _loadedDocumentData = loadedDocumentData;
            }

            protected override JObject LoadAnyContent(JToken blockContent)
            {
                return (JObject) _loadedDocumentData["Info"].DeepClone();
            }
        }
    }    
}