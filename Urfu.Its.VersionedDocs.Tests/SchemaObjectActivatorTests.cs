using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.Tests.TestData;

namespace Urfu.Its.VersionedDocs.Tests
{
    [TestClass]
    public class SchemaObjectActivatorTests
    {
        [TestMethod]
        public void SimplePropertiesTest()
        {
            var schema = File.ReadAllText("TestData/simple1.jschema");

            IVersionedDocumentSchemaService activator = new VersionedDocumentSchemaService();

            var obj = activator.Create(schema, File.ReadAllText("TestData/simple1.json"));

            obj.ShouldBeEquivalentTo(new Simple1());
        }

        [TestMethod]
        public void ArrayPropertiesTest()
        {
            var schema = File.ReadAllText("TestData/arrays1.jschema");

            IVersionedDocumentSchemaService activator = new VersionedDocumentSchemaService();

            var obj = activator.Create(schema, File.ReadAllText("TestData/arrays1.json"));

            obj.ShouldBeEquivalentTo(new Arrays1());
        }
    }
}