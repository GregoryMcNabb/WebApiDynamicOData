using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using DynamicOData.Models;
using DynamicDataServiceTests;
using System.Web.Http;
using DynamicDataServiceTests.TestingHelpers;

namespace DynamicDataService.Controllers.Tests
{


    [TestClass]
    public class GenericDataControllerTests
    {
        [TestMethod, TestCategory("Routing")]
        public void GetAllItemRoute()
        {
            var result = ActionSelectorValidator.GetTargetAction(new HttpMethod("GET"), nameof(MockContext.Items));
            Assert.AreEqual("Get", result.ActionName);
            Assert.AreEqual(nameof(MockContext.Items), result.ControllerDescriptor.ControllerName);
            Assert.IsTrue(result.ReturnType == typeof(IQueryable<Item>));
        }

        [TestMethod, TestCategory("Routing")]
        public void GetAllTypesRoute()
        {
            var result = ActionSelectorValidator.GetTargetAction(new HttpMethod("GET"), nameof(MockContext.Types));
            Assert.AreEqual("Get", result.ActionName);
            Assert.AreEqual($"{nameof(MockContext.Types)}", result.ControllerDescriptor.ControllerName);
            Assert.IsTrue(result.ReturnType == typeof(IQueryable<ItemType>));
        }

        [TestMethod, TestCategory("Routing")]
        public void GetItemByIDRoute()
        {
            var result = ActionSelectorValidator.GetTargetAction(new HttpMethod("GET"), $"{nameof(MockContext.Items)}(1)");
            Assert.AreEqual("Get", result.ActionName);
            Assert.AreEqual($"{nameof(MockContext.Items)}", result.ControllerDescriptor.ControllerName);
            Assert.IsTrue(result.ReturnType == typeof(IQueryable<Item>));
        }

        [TestMethod, TestCategory("Routing")]
        public void GetManyItemsByNavigationProperty()
        {
            var result = ActionSelectorValidator.GetTargetAction(new HttpMethod("GET"), $"{nameof(MockContext.Types)}(1)/Items");
            Assert.AreEqual("GetRelatedEntities", result.ActionName);
            Assert.AreEqual($"{nameof(MockContext.Types)}", result.ControllerDescriptor.ControllerName);
            Assert.IsTrue(result.ReturnType == typeof(IQueryable<Item>));
        }

        [TestMethod, TestCategory("Routing")]
        public void GetTypeByNavigationProperty()
        {
            var result = ActionSelectorValidator.GetTargetAction(new HttpMethod("GET"), $"{nameof(MockContext.Items)}(1)/Type");
            Assert.AreEqual("GetRelatedEntity", result.ActionName);
            Assert.AreEqual($"{nameof(MockContext.Items)}", result.ControllerDescriptor.ControllerName);
            Assert.IsTrue(result.ReturnType == typeof(SingleResult<ItemType>));
        }
        

        [TestMethod, TestCategory("Routing")]
        public void GetItemExpandTypeRouting()
        {
            var result = ActionSelectorValidator.GetTargetAction(new HttpMethod("GET"), $"{nameof(MockContext.Items)}(1)?$expand=Type");
            Assert.AreEqual("Get", result.ActionName);
            Assert.AreEqual($"{nameof(MockContext.Items)}", result.ControllerDescriptor.ControllerName);
            Assert.IsTrue(result.ReturnType == typeof(IQueryable<Item>));
        }
    }
}