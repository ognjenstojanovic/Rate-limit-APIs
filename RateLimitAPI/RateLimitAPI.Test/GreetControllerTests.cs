using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimitAPI.Test.Setup;
using System.Threading;

namespace RateLimitAPI.Test
{
    [TestClass]
    public class GreetControllerTests
    {
        private readonly string greetRoute = "greet/{0}";
        private TestServerFixture fixture;

        [TestInitialize]
        public void Init()
        {
            fixture = new TestServerFixture();
        }

        [TestMethod]
        public void GreetController_Get_ReturnsProvidedParameter()
        {
            // Arrange
            var name = "Ognjen";

            // Act
            var response = fixture.Client.GetAsync(string.Format(greetRoute, name)).Result;

            // Assert            
            AssertSuccessfullResponse(name, response);
        }

        [TestMethod]
        public void GreetController_GetMoreTimesWithoutExceedingLimit_ReturnsProvidedParameter()
        {
            // Arrange
            var name = "Ognjen";

            // Act and Assert
            for (int i = 0; i < 10; i++)
            {
                var response = fixture.Client.GetAsync(string.Format(greetRoute, name)).Result;
                AssertSuccessfullResponse(name, response);
            }
        }

        [TestMethod]
        public void GreetController_ExceedRateLimit_ReturnsTooManyRequests()
        {
            // Arrange
            var name = "Ognjen";

            // Act and Assert
            for (int i = 0; i < 10; i++)
            {
                var response = fixture.Client.GetAsync(string.Format(greetRoute, name)).Result;
                AssertSuccessfullResponse(name, response);
            }

            var failedResponse = fixture.Client.GetAsync(string.Format(greetRoute, name)).Result;
            AssertFailedResponse(failedResponse);
        }


        [TestMethod]
        public void GreetController_RateLimitResetsAfterOneMinute_ReturnsProvidedParameterAfterWaiting()
        {
            // Arrange
            var name = "Ognjen";

            // Act and Assert
            for (int i = 0; i < 10; i++)
            {
                var response = fixture.Client.GetAsync(string.Format(greetRoute, name)).Result;
                AssertSuccessfullResponse(name, response);
            }

            var failedResponse = fixture.Client.GetAsync(string.Format(greetRoute, name)).Result;
            AssertFailedResponse(failedResponse);

            Thread.Sleep(60 * 1000);

            var successfullResponse = fixture.Client.GetAsync("greet/" + name).Result;
            AssertSuccessfullResponse(name, successfullResponse);
        }

        private static void AssertFailedResponse(System.Net.Http.HttpResponseMessage failedResponse)
        {
            Assert.IsTrue(failedResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests);
            Assert.IsTrue(failedResponse.Content.ReadAsStringAsync().Result.Replace("\"", "") == "Rate limit exceeded.");
        }

        private static void AssertSuccessfullResponse(string name, System.Net.Http.HttpResponseMessage response)
        {
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.IsTrue(response.Content.ReadAsStringAsync().Result.Replace("\"", "") == "Hi, " + name);

            Assert.IsTrue(response.Headers.Contains("X-RateLimit-Limit"));
            Assert.IsTrue(response.Headers.Contains("X-RateLimit-Remaining"));
        }
    }
}
