using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;

namespace Core.Tests.Models
{
    [TestClass]
    public class DocumentMetadataTests
    {
        [TestMethod]
        public void Serialize()
        {
            var metadata = new DocumentMetadata()
            {
                StatTagVersion = "3.1.0"
            };
            var serialized = metadata.Serialize();
            Assert.AreEqual("{\"StatTagVersion\":\"3.1.0\"}", serialized);
        }

        [TestMethod]
        public void Deserialize()
        {
            var metadata = DocumentMetadata.Deserialize("{\"StatTagVersion\":\"3.1.0\"}");
            Assert.AreEqual("3.1.0", metadata.StatTagVersion);
        }

        [TestMethod]
        public void Deserialize_ExtraFields()
        {
            // Ensure deserializing works with extra fields in there
            var metadata = DocumentMetadata.Deserialize("{\"Extra\":\"Data\", \"StatTagVersion\":\"3.1.0\"}");
            Assert.AreEqual("3.1.0", metadata.StatTagVersion);
        }
    }
}
