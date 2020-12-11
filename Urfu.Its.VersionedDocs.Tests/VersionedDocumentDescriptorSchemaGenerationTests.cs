using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Services;

namespace Urfu.Its.VersionedDocs.Tests
{
    [TestClass]
    public class VersionedDocumentDescriptorSchemaGenerationTests
    {
        [TestMethod]
        public void PrimitiveBlockTest()
        {
            var docDesc = new VersionedDocumentDescriptor
            {
                Blocks =
                {
                    new VersionedDocumentBlockDescriptor("Annotation", VersionedDocumentBlockItemKind.String)                    
                }
            };

            var schema = docDesc.GenerateSchema();

            var expectedSchema = new JSchema
            {
                Type = JSchemaType.Object,
                Properties =
                {
                    ["Annotation"] = new JSchema
                    {
                        Type = JSchemaType.String
                    }
                }                
            };            

            Assert.IsTrue(new VersionedDocumentSchemaService().IsSchemaMatched(expectedSchema.ToString(), schema.ToString()));
            
            //CollectionAssert.AreEqual(expectedSchema.Properties.Keys.ToArray(), schema.Properties.Keys.ToArray());
            //CollectionAssert.AreEqual(expectedSchema.Properties.Values.Select(v=>v.ToString()).ToArray(), schema.Properties.Values.Select(v => v.ToString()).ToArray());            
        }

        [TestMethod]
        public void SimpleBlockTest()
        {
            var docDesc = new VersionedDocumentDescriptor
            {
                Blocks =
                {
                    new VersionedDocumentBlockDescriptor("StringProp1", VersionedDocumentBlockItemKind.String),
                    new VersionedDocumentBlockDescriptor("BooleanProp1", VersionedDocumentBlockItemKind.Boolean),
                    new VersionedDocumentBlockDescriptor("IntegerProp1", VersionedDocumentBlockItemKind.Integer),
                    new VersionedDocumentBlockDescriptor("NumberProp1", VersionedDocumentBlockItemKind.Number),
                }
            };

            var schema = docDesc.GenerateSchema();

            var expectedSchema = JSchema.Parse(File.ReadAllText("TestData/simple1.jschema"));

            schema.ShouldBeEquivalentTo(expectedSchema, o=>o.ExcludingFields());
        }

        [TestMethod]
        public void SingleArrayBlockTest()
        {
            var docDesc = new VersionedDocumentDescriptor(new[] {
                new VersionedDocumentBlockDescriptor("Authors", new VersionedDocumentBlockItemDescriptor(
                    new []
                    {
                        new VersionedDocumentBlockItemDescriptor("Fio", VersionedDocumentBlockItemKind.String),
                        new VersionedDocumentBlockItemDescriptor("Cathedra", VersionedDocumentBlockItemKind.String)
                    }))});

            var schema = docDesc.GenerateSchema();

            var expectedSchema = new JSchema
            {
                Type = JSchemaType.Object,
                AllowAdditionalProperties = false,
                Properties =
                {
                    ["Authors"] = new JSchema
                    {
                        Type = JSchemaType.Array,
                        Items =
                        {
                            new JSchema
                            {
                                Type = JSchemaType.Object,
                                AllowAdditionalProperties = false,
                                Properties =
                                {
                                    ["Fio"] = new JSchema { Type = JSchemaType.String },
                                    ["Cathedra"] = new JSchema { Type = JSchemaType.String },
                                },
                                Required = { "Fio", "Cathedra" }
                            }
                        }
                    }
                }
            };
            
            Assert.IsTrue(new VersionedDocumentSchemaService().IsSchemaMatched(expectedSchema.ToString(), schema.ToString()));
        }
    }
}
