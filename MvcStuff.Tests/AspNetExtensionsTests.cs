using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcStuff.Tests
{
    [TestClass]
    public class AspNetExtensionsTests
    {
        [TestMethod]
        public void Test_AcceptedMimesInOrder_1()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "*/*";
            var accepted = request.AcceptedMimesInOrder();
            Assert.AreEqual(accepted[0][0], "*/*");
        }

        [TestMethod]
        public void Test_AcceptedMimesInOrder_2()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "*/*;q=0.5";
            var accepted = request.AcceptedMimesInOrder();
            Assert.AreEqual(accepted[0][0], "*/*");
        }

        [TestMethod]
        public void Test_AcceptedMimesInOrder_3()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "text/html,*/*;q=0.2";
            var accepted = request.AcceptedMimesInOrder();
            Assert.AreEqual(accepted[0][0], "text/html");
            Assert.AreEqual(accepted[1][0], "*/*");
        }

        [TestMethod]
        public void Test_AcceptedMimesInOrder_4()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "*/*;q=0.2,text/html";
            var accepted = request.AcceptedMimesInOrder();
            Assert.AreEqual(accepted[0][0], "text/html");
            Assert.AreEqual(accepted[1][0], "*/*");
        }

        [TestMethod]
        public void Test_AcceptedMimesInOrder_5()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            var accepted = request.AcceptedMimesInOrder();
            Assert.AreEqual(accepted[0][0], "text/html");
            Assert.AreEqual(accepted[0][1], "application/xhtml+xml");
            Assert.AreEqual(accepted[1][0], "application/xml");
            Assert.AreEqual(accepted[2][0], "*/*");
        }

        [TestMethod]
        public void Test_AcceptedMimesInOrder_6()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            var accepted = request.AcceptedMimesInOrder();
            Assert.AreEqual(accepted[0][0], "application/xml");
            Assert.AreEqual(accepted[0][1], "application/xhtml+xml");
            Assert.AreEqual(accepted[0][2], "image/png");
            Assert.AreEqual(accepted[1][0], "text/html");
            Assert.AreEqual(accepted[2][0], "text/plain");
            Assert.AreEqual(accepted[3][0], "*/*");
        }

        [TestMethod]
        public void Test_GetBestResponseMimeTypes_1()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";

            var bestMimes = request.GetBestResponseMimeTypes(
                "text/html,application/xhtml+xml;q=0.9,application/json;q=0.85,application/xml;q=0.80,*/*;q=0.7");

            Assert.AreEqual(bestMimes[0], "application/xhtml+xml");
            Assert.AreEqual(bestMimes[1], "text/html");
        }

        [TestMethod]
        public void Test_IsHtmlRequest_1()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";

            Assert.IsTrue(request.IsHtmlRequest());
        }

        [TestMethod]
        public void Test_IsJsonRequest_1()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "application/json,application/xml;q=0.9,*/*;q=0.5";

            Assert.IsTrue(request.IsJsonRequest());
        }

        [TestMethod]
        public void Test_IsXmlRequest_1()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";

            Assert.IsFalse(request.IsXmlRequest());
        }

        [TestMethod]
        public void Test_IsXmlRequest_2()
        {
            var request = new MockHttpRequest();
            request.Headers["Accept"] = "application/xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";

            Assert.IsTrue(request.IsXmlRequest());
        }
    }
}
