using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Utilities.Tests
{
    [TestClass()]
    public class FilterRulesTests
    {
        [TestMethod()]
        public void DeserializeTest()
        {
            
            string json = "[{\"property\":\"title\",\"value\":\"789\"},{\"property\":\"section\",\"value\":\"test\"}]";
            var rules = FilterRules.Deserialize(json);

            Assert.AreEqual(2, rules.Count());
            var rule0 = rules[0];

            Assert.IsInstanceOfType(rule0, typeof(FilterRule));

            Assert.AreEqual("title", rule0.Property);
            Assert.AreEqual("789", rule0.Value);

            var rule1 = rules[1];

            Assert.IsInstanceOfType(rule1, typeof(FilterRule));
            Assert.AreEqual("test", rule1.Value);

            rules = FilterRules.Deserialize("");
            Assert.IsNull(rules);

            rules = FilterRules.Deserialize(null);
            Assert.IsNull(rules);

            rules = FilterRules.Deserialize(" ");
            Assert.IsNull(rules);
        }
    }
}