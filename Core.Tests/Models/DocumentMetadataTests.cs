using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;

namespace Core.Tests.Models
{
    [TestClass]
    public class DocumentMetadataTests
    {
        [TestMethod]
        public void Serialize_Partial()
        {
            var metadata = new DocumentMetadata()
            {
                StatTagVersion = "3.1.0"
            };
            var serialized = metadata.Serialize();
            Assert.AreEqual("{\"MetadataFormatVersion\":null,\"TagFormatVersion\":null,\"StatTagVersion\":\"3.1.0\",\"RepresentMissingValues\":null,\"CustomMissingValue\":null}", serialized);
        }

        [TestMethod]
        public void Serialize_Full()
        {
            var metadata = new DocumentMetadata()
            {
                StatTagVersion = "StatTag v3.1.0",
                RepresentMissingValues = "StatPackageDefault",
                CustomMissingValue = "[X]",
                MetadataFormatVersion = "1.0.0",
                TagFormatVersion = "1.0.0"
            };
            var serialized = metadata.Serialize();
            Assert.AreEqual("{\"MetadataFormatVersion\":\"1.0.0\",\"TagFormatVersion\":\"1.0.0\",\"StatTagVersion\":\"StatTag v3.1.0\",\"RepresentMissingValues\":\"StatPackageDefault\",\"CustomMissingValue\":\"[X]\"}", serialized);
        }

        [TestMethod]
        public void Deserialize_Partial()
        {
            var metadata = DocumentMetadata.Deserialize("{\"StatTagVersion\":\"3.1.0\"}");
            Assert.AreEqual("3.1.0", metadata.StatTagVersion);
            Assert.IsNull(metadata.RepresentMissingValues);
            Assert.IsNull(metadata.CustomMissingValue);
            Assert.IsNull(metadata.MetadataFormatVersion);
            Assert.IsNull(metadata.TagFormatVersion);
        }

        [TestMethod]
        public void Deserialize_Full()
        {
            var metadata = DocumentMetadata.Deserialize("{\"MetadataFormatVersion\":\"1.0.0\",\"TagFormatVersion\":\"1.0.0\",\"StatTagVersion\":\"StatTag v3.1.0\",\"RepresentMissingValues\":\"StatPackageDefault\",\"CustomMissingValue\":\"[X]\"}");
            Assert.AreEqual("StatTag v3.1.0", metadata.StatTagVersion);
            Assert.AreEqual("StatPackageDefault", metadata.RepresentMissingValues);
            Assert.AreEqual("[X]", metadata.CustomMissingValue);
            Assert.AreEqual("1.0.0", metadata.MetadataFormatVersion);
            Assert.AreEqual("1.0.0", metadata.TagFormatVersion);
        }

        [TestMethod]
        public void Deserialize_WithNullValues()
        {
            var metadata = DocumentMetadata.Deserialize("{\"MetadataFormatVersion\":\"1.0.0\",\"TagFormatVersion\":\"1.0.0\",\"StatTagVersion\":\"StatTag v3.1.0\",\"RepresentMissingValues\":null,\"CustomMissingValue\":null}");
            Assert.AreEqual("StatTag v3.1.0", metadata.StatTagVersion);
            Assert.AreEqual("1.0.0", metadata.MetadataFormatVersion);
            Assert.AreEqual("1.0.0", metadata.TagFormatVersion);
            Assert.IsNull(metadata.RepresentMissingValues);
            Assert.IsNull(metadata.CustomMissingValue);
        }

        [TestMethod]
        public void Deserialize_ExtraFields()
        {
            // Ensure deserializing works with extra fields in there
            var metadata = DocumentMetadata.Deserialize("{\"Extra\":\"Data\", \"StatTagVersion\":\"3.1.0\"}");
            Assert.AreEqual("3.1.0", metadata.StatTagVersion);
            Assert.AreEqual("Data", metadata.ExtraMetadata["Extra"]);
        }


        [TestMethod]
        public void FullSerializeDeserializeExtraFields()
        {
            // Ensure deserializing works with extra fields in there
            var metadata = DocumentMetadata.Deserialize("{\"Extra\":\"Data\", \"StatTagVersion\":\"3.1.0\"}");
            Assert.AreEqual("3.1.0", metadata.StatTagVersion);
            Assert.AreEqual("Data", metadata.ExtraMetadata["Extra"]);

            var serialized = metadata.Serialize();
            Assert.AreEqual("{\"MetadataFormatVersion\":null,\"TagFormatVersion\":null,\"StatTagVersion\":\"3.1.0\",\"RepresentMissingValues\":null,\"CustomMissingValue\":null,\"Extra\":\"Data\"}", serialized);
        }
    }
}
