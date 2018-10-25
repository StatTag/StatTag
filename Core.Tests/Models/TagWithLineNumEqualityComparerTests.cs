using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;

namespace Core.Tests.Models
{
    [TestClass]
    public class TagWithLineNumEqualityComparerTests
    {
        [TestMethod]
        public void TagWithLineNumEqualityComparer_Empty()
        {
            var tags = new List<Tag>();
            var comparer = new TagWithLineNumEqualityComparer();
            Assert.IsFalse(tags.AsEnumerable().Contains(null, comparer));
            Assert.IsFalse(tags.AsEnumerable().Contains(new Tag() { Name = "Test 1" }, comparer));
        }

        [TestMethod]
        public void TagWithLineNumEqualityComparer_NotEqual()
        {
            var tags = new List<Tag>()
            {
                new Tag() {Name = "Test 1", LineStart = 1, LineEnd = 2},
                new Tag() {Name = "Test 1", LineStart = 4, LineEnd = 6}
            };
            var comparer = new TagWithLineNumEqualityComparer();
            Assert.IsFalse(tags.AsEnumerable().Contains(new Tag() { Name = "Test 1" }, comparer));
            Assert.IsFalse(tags.AsEnumerable().Contains(new Tag() { Name = "Test 1", LineStart = 1 }, comparer));
            Assert.IsFalse(tags.AsEnumerable().Contains(new Tag() { Name = "Test 1", LineStart = 1, LineEnd = 3 }, comparer));
            Assert.IsFalse(tags.AsEnumerable().Contains(new Tag() { Name = "Test1", LineStart = 1, LineEnd = 2 }, comparer));
        }

        [TestMethod]
        public void TagWithLineNumEqualityComparer_Equal()
        {
            var tags = new List<Tag>()
            {
                new Tag() {Name = "Test 1", LineStart = 1, LineEnd = 2},
                new Tag() {Name = "Test 1", LineStart = 4, LineEnd = 6}
            };
            var comparer = new TagWithLineNumEqualityComparer();
            Assert.IsTrue(tags.AsEnumerable().Contains(new Tag() { Name = "Test 1", LineStart = 1, LineEnd = 2 }, comparer));
            Assert.IsTrue(tags.AsEnumerable().Contains(new Tag() { Name = "Test 1", LineStart = 4, LineEnd = 6 }, comparer));
        }
    }
}
