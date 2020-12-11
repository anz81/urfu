using Microsoft.VisualStudio.TestTools.UnitTesting;
using Urfu.Its.Integration.ApiModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Integration.ApiModel
{
    [TestClass]
    public class ApiDtoFunctionsTests
    {
        [TestMethod]
        public void ToSubgroupKeyTest()
        {
            Assert.AreEqual("1-group-discipline-action-1-2016", ApiDtoFunctions.ToSubgroupKey(1, "group", "discipline", "action", 1, 2016));
        }
    }
}