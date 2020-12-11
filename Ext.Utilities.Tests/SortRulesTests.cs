using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Ext.Utilities.Tests
{
    [TestClass()]
    public class SortRulesTests
    {
        [TestMethod()]
        public void DesirializeTest()
        {
            string json = "[{\"property\":\"title\",\"direction\":\"ASC\"},{\"property\":\"title\",\"direction\":\"DESC\"}]";
            var rules = SortRules.Deserialize(json);

            Assert.AreEqual(2, rules.Count());
            var rule0 = rules[0];

            Assert.IsInstanceOfType(rule0, typeof(SortRule));

            Assert.AreEqual("title", rule0.Property);
            Assert.AreEqual(SortDirection.Ascending, rule0.Direction);

            var rule1 = rules[1];

            Assert.IsInstanceOfType(rule1, typeof(SortRule));
            Assert.AreEqual(SortDirection.Descending, rule1.Direction);

            rules = SortRules.Deserialize("");
            Assert.IsNull(rules);

            rules = SortRules.Deserialize(null);
            Assert.IsNull(rules);

            rules = SortRules.Deserialize(" ");
            Assert.IsNull(rules);
        }
    }
}