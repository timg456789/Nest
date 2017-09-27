using System;
using System.IO;
using System.Linq;
using Nest;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Assert = Xunit.Assert;

namespace NestTests
{
    public class NestCameraCheckTests
    {
        private void SetEnvironmentVariableFromLambdaDeploySettings()
        {
            using (var file = File.OpenText("C:\\Users\\peon\\Desktop\\projects\\Nest\\Nest\\aws-lambda-tools-defaults.json")) // Lamda deploy environment variables.
            {
                var reader = new JsonTextReader(file);
                var jObject = JObject.Load(reader);

                string environmentVariables = jObject.Value<string>("environment-variables");
                string[] splitVars = environmentVariables.Split('=');

                for (var ct = 0; ct < splitVars.Length; ct += 2)
                {
                    var unquotedName = splitVars[ct].Substring(1, splitVars[ct].Length - 2);
                    var unquotedValue = splitVars[ct + 1].Substring(1, splitVars[ct + 1].Length - 2);
                    Environment.SetEnvironmentVariable(unquotedName, unquotedValue);
                }
            }
        }

        [Fact]
        public void Camera_Online_Message_Returned_With_Valid_Token_And_Streaming_Camera()
        {
            SetEnvironmentVariableFromLambdaDeploySettings();

            var context = new TestLambdaContext();
            var function = new Function();
            var cameraStatusConfirmation = function.FunctionHandler(context);
            Assert.Equal("Camera is online and streaming", cameraStatusConfirmation);
        }

        [Fact]
        public void Verify_Parsing_Exception_Is_Thrown_And_Response_Is_Logged()
        {
            var listLogger = new ListLogger();
            var nestClient = new NestClient(null, listLogger);

            var nestSummary = "not json";
            Assert.Throws<JsonReaderException>(() => nestClient.GetNestSummary(nestSummary));

            Assert.True(listLogger.log.Count(x => x.Equals("Failed to parse: " + nestSummary)) == 1);
        }

        [Fact]
        public void Verify_Camera_Exception_Cases()
        {
            var cameraStatus = new NestCameraStatus();
            Assert.Throws<NestCameraOfflineException>(() =>
                cameraStatus.ThrowExceptionIfCameraIsntOnlineAndStreaming(new NestCamera()));

            Assert.Throws<NestCameraOfflineException>(() =>
                cameraStatus.ThrowExceptionIfCameraIsntOnlineAndStreaming(new NestCamera
                {
                    IsOnline = true
                }));

            Assert.Throws<NestCameraOfflineException>(() =>
                cameraStatus.ThrowExceptionIfCameraIsntOnlineAndStreaming(new NestCamera
                {
                    IsStreaming = true // Probably isn't posible to be streaming and offline...This is a test case I can remove when I gain experience and comfort with this problem area. For now however, I'm validating the API's themselves and making no assumptions.
                }));

            var fullyOnlineCamera = new NestCamera
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
