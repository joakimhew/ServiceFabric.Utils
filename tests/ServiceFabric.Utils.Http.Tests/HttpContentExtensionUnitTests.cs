using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceFabric.Utils.Http.Extensions;

namespace ServiceFabric.Utils.Http.Tests
{
    public class ExampleMessageType
    {
        public string Header { get; set; }
        public string Body { get; set; }
    }

    [TestClass]
    public class HttpContentExtensionUnitTests
    {

        [TestMethod]
        public void Is_HttpContent_Read_As_ApiResponseMessage()
        {
            // Arrange
            HttpContent httpContent = new StringContent(
                @"{ ""Code"" : 200,
                    ""Message"": {""Header"" : ""Test header"", ""Body"" : ""Test body""}, 
                    ""Info"" : ""Extra info""
                  }");

            var expectedApiResponseMessage = new ApiResponseMessage<ExampleMessageType>()
            {
                Code = (HttpStatusCode)200,
                Message = new ExampleMessageType { Header = "Test header", Body = "Test body" },
                Info = "Extra info"
            };

            // Act
            var actualApiResponseMessage = httpContent.ReadAsApiResponseMessage<ExampleMessageType>();

            // Assert
            Assert.AreEqual(expectedApiResponseMessage.Code, actualApiResponseMessage.Code);
            Assert.AreEqual(expectedApiResponseMessage.Message.Header, actualApiResponseMessage.Message.Header);
            Assert.AreEqual(expectedApiResponseMessage.Message.Body, actualApiResponseMessage.Message.Body);
            Assert.AreEqual(expectedApiResponseMessage.Info, actualApiResponseMessage.Info);
        }

        [TestMethod]
        public async Task Is_HttpContent_Read_As_ApiResponseMessage_Asynchronously()
        {
            // Arrange
            HttpContent httpContent = new StringContent(
                @"{ ""Code"" : 200,
                    ""Message"": {""Header"" : ""Test header"", ""Body"" : ""Test body""}, 
                    ""Info"" : ""Extra info""
                  }");

            var expectedApiResponseMessage = new ApiResponseMessage<ExampleMessageType>()
            {
                Code = (HttpStatusCode) 200,
                Message = new ExampleMessageType {Header = "Test header", Body = "Test body"},
                Info = "Extra info"
            };

            // Act
            var actualApiResponseMessage = await httpContent.ReadAsApiResponseMessageAsync<ExampleMessageType>();

            // Assert
            Assert.AreEqual(expectedApiResponseMessage.Code, actualApiResponseMessage.Code);
            Assert.AreEqual(expectedApiResponseMessage.Message.Header, actualApiResponseMessage.Message.Header);
            Assert.AreEqual(expectedApiResponseMessage.Message.Body, actualApiResponseMessage.Message.Body);
            Assert.AreEqual(expectedApiResponseMessage.Info, actualApiResponseMessage.Info);
        }

        [TestMethod]
        public void Trying_To_Read_Valid_Json_As_ApiResponseMessage_From_HttpContent_Is_Successful()
        {
            // Arrange
            HttpContent httpContent = new StringContent(
                @"{ ""Code"" : 200,
                    ""Message"": {""Header"" : ""Test header"", ""Body"" : ""Test body""}, 
                    ""Info"" : ""Extra info""
                  }");


            var expectedApiResponseMessage = new ApiResponseMessage<ExampleMessageType>()
            {
                Code = (HttpStatusCode)200,
                Message = new ExampleMessageType { Header = "Test header", Body = "Test body" },
                Info = "Extra info"
            };

            ApiResponseMessage<ExampleMessageType> actualApiResponseMessage;

            // Act
            if (!httpContent.TryReadAsApiResponseMessage(out actualApiResponseMessage))
            {
                Assert.Fail();
            }

            // Assert
            Assert.AreEqual(expectedApiResponseMessage.Code, actualApiResponseMessage.Code);
            Assert.AreEqual(expectedApiResponseMessage.Message.Header, actualApiResponseMessage.Message.Header);
            Assert.AreEqual(expectedApiResponseMessage.Message.Body, actualApiResponseMessage.Message.Body);
            Assert.AreEqual(expectedApiResponseMessage.Info, actualApiResponseMessage.Info);
        }

        [TestMethod]
        public async Task Trying_To_Read_Valid_Json_As_ApiResponseMessage_From_HttpContent_Asynchronously_Is_Successful()
        {
            // Arrange
            HttpContent httpContent = new StringContent(
                @"{ ""Code"" : 200,
                    ""Message"": {""Header"" : ""Test header"", ""Body"" : ""Test body""}, 
                    ""Info"" : ""Extra info""
                  }");


            var expectedApiResponseMessage = new ApiResponseMessage<ExampleMessageType>()
            {
                Code = (HttpStatusCode)200,
                Message = new ExampleMessageType { Header = "Test header", Body = "Test body" },
                Info = "Extra info"
            };

            // Act
            ApiResponseMessage<ExampleMessageType> actualApiResponseMessage = null;

            var result = await httpContent.TryReadAsApiResponseMessageAsync<ExampleMessageType>();

            actualApiResponseMessage = result.ExpectedApiResponseMessage;

            // Assert

            if (!result.Success || actualApiResponseMessage == null)
            {
                Assert.Fail();
            }

            Assert.AreEqual(expectedApiResponseMessage.Code, actualApiResponseMessage.Code);
            Assert.AreEqual(expectedApiResponseMessage.Message.Header, actualApiResponseMessage.Message.Header);
            Assert.AreEqual(expectedApiResponseMessage.Message.Body, actualApiResponseMessage.Message.Body);
            Assert.AreEqual(expectedApiResponseMessage.Info, actualApiResponseMessage.Info);
        }

        [TestMethod]
        public async Task Trying_To_Read_As_ApiResponseMessage_Asynchronously_Returns_False_When_Json_Is_Invalid_Format()
        {
            // Arrange
            HttpContent httpContent = new StringContent(
                @"INVALID JSON");

            // Act
            var result = await httpContent.TryReadAsApiResponseMessageAsync<ExampleMessageType>();

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task Trying_To_Read_As_ApiResponseMessage_Asynchronously_Returns_False_When_Json_Schema_Is_Invalid()
        {
            // Arrage
            HttpContent httpContent = new StringContent(
                @"{ ""WrongProperty1"" : ""Whatever1"", ""WrongProperty2"" : ""Whatever2"" }");

            // Act
            var result = await httpContent.TryReadAsApiResponseMessageAsync<ExampleMessageType>();

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void Trying_To_Read_As_ApiResponseMessage_Returns_False_When_Json_Is_Invalid_Format()
        {
            // Arrange
            HttpContent httpContent = new StringContent(
                @"INVALID JSON");

            ApiResponseMessage<ExampleMessageType> apiResponseMessage;

            // Act
            var result = httpContent.TryReadAsApiResponseMessage(out apiResponseMessage);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Trying_To_Read_As_ApiResponseMessage_Returns_False_When_Json_Schema_Is_Invalid()
        {
            // Arrange
            HttpContent httpContent = new StringContent(
                @"{ ""WrongProperty1"" : ""Whatever1"", ""WrongProperty2"" : ""Whatever2"" }");

            ApiResponseMessage<ExampleMessageType> apiResponseMessage;

            // Act
            var result = httpContent.TryReadAsApiResponseMessage(out apiResponseMessage);

            // Assert
            Assert.IsFalse(result);
        }
    }
}