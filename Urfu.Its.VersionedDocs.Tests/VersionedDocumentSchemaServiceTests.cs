using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Services;

namespace Urfu.Its.VersionedDocs.Tests
{
    [TestClass]
    public class VersionedDocumentSchemaServiceTests
    {
        [TestMethod]
        public void ShouldSchemaMatch()
        {
            IVersionedDocumentSchemaService service = new VersionedDocumentSchemaService();

            var isMatched = service.IsSchemaMatched(
                File.ReadAllText("TestData/schemaToMatch[1].jschema"),
                File.ReadAllText("TestData/schemaToMatch[2].jschema"));

            Assert.IsTrue(isMatched);
        }

        [TestMethod]
        public void ShouldNotSchemaMatch1()
        {
            IVersionedDocumentSchemaService service = new VersionedDocumentSchemaService();

            var isMatched = service.IsSchemaMatched(
                File.ReadAllText("TestData/schemaToNotMatch[1].jschema"),
                File.ReadAllText("TestData/schemaToNotMatch[2].jschema"));

            Assert.IsFalse(isMatched);
        }

        [TestMethod]
        public void ShouldNotSchemaMatch2()
        {
            IVersionedDocumentSchemaService service = new VersionedDocumentSchemaService();

            var isMatched = service.IsSchemaMatched(
                File.ReadAllText("TestData/schemaToNotMatch[1].jschema"),
                File.ReadAllText("TestData/schemaToNotMatch[3].jschema"));

            Assert.IsFalse(isMatched);
        }
    }
}