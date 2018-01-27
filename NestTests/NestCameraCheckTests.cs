using System;
using System.Linq;
using Xunit;
using Amazon.Lambda.TestUtilities;
using NestTools;
using NestTools.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace NestTests
{
    public class NestCameraCheckTests
    {

        private ITestOutputHelper output;
        private PrivateConfig config = PrivateConfig.CreateFromPersonalJson();

        public NestCameraCheckTests(ITestOutputHelper output)
        {
            this.output = output;
            Environment.SetEnvironmentVariable("access_token", config.NestDecryptedAccessToken);
            Environment.SetEnvironmentVariable("s3Bucket", config.NestS3Bucket);
            Environment.SetEnvironmentVariable("s3Key", config.NestS3Key);
        }

        [Fact]
        public void GetHomeOrAwayStatus()
        {
            var listLogger = new ListLogger();

            var nestClient = new NestClient(config.NestDecryptedAccessToken, listLogger);

            var homeStatus = nestClient
                .GetStructures()
                .Single(x => x.Name.Equals("home", StringComparison.OrdinalIgnoreCase)).Away;

            output.WriteLine(homeStatus);

            Assert.True(homeStatus.Equals("home", StringComparison.OrdinalIgnoreCase) ||
                        homeStatus.Equals("away", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Camera_Online_Message_Returned_With_Valid_Token_And_Streaming_Camera()
        {

            var context = new TestLambdaContext();
            var function = new Function();
            var cameraStatusConfirmation = function.FunctionHandler(new JObject(), context);
            Assert.StartsWith("Camera is online and streaming. Snapshot saved status 200 bucket tgonzalez-nest", cameraStatusConfirmation);
        }

        [Fact]
        public void Verify_Parsing_Exception_Is_Thrown_And_Response_Is_Logged()
        {
            var listLogger = new ListLogger();
            var nestClient = new NestClient(null, listLogger);

            var nestSummary = "not json";
            Assert.Throws<JsonReaderException>(() => nestClient.GetNestSummary(nestSummary));

            Assert.True(listLogger.log.Count(x => x.Equals("Parsing: " + nestSummary)) == 1);
        }

        [Fact]
        public void Verify_Camera_Exception_Cases()
        {
            var cameraStatus = new NestCameraStatus();
            Assert.Throws<NestCameraOfflineException>(() =>
                cameraStatus.ThrowExceptionIfCameraIsntOnlineAndStreaming(new NestCameraJson()));

            Assert.Throws<NestCameraOfflineException>(() =>
                cameraStatus.ThrowExceptionIfCameraIsntOnlineAndStreaming(new NestCameraJson
                {
                    IsOnline = true
                }));

            Assert.Throws<NestCameraOfflineException>(() =>
                cameraStatus.ThrowExceptionIfCameraIsntOnlineAndStreaming(new NestCameraJson
                {
                    IsStreaming = true // Probably isn't posible to be streaming and offline...This is a test case I can remove when I gain experience and comfort with this problem area. For now however, I'm validating the API's themselves and making no assumptions.
                }));

            var fullyOnlineCamera = new NestCameraJson
            {
                IsOnline = true,
                IsStreaming = true
            };
            cameraStatus.ThrowExceptionIfCameraIsntOnlineAndStreaming(fullyOnlineCamera);
            Assert.False(cameraStatus.IsOffline(fullyOnlineCamera));
        }

        // I can't add any type of test for this. It requires a one-time pin.
        public void CreateAuthTokenFromPin(string productId, string productSecret, string customerAuthPin)
        {
            var endUserAuthClient = new EndUserAuthenticationClient();
            var endUserAuth = endUserAuthClient.CreateAuthTokenFromPin(
                productId,
                productSecret,
                customerAuthPin);

            Assert.False(string.IsNullOrWhiteSpace(endUserAuth.AccessToken));
            Assert.Equal(315360000, endUserAuth.ExpiresIn);
        }
    }
}
