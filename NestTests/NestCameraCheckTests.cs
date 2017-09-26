using System;
using System.IO;
using Nest;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public void Test1()
        {
            SetEnvironmentVariableFromLambdaDeploySettings();

            var context = new TestLambdaContext();
            var function = new Function();
            var cameraStatusConfirmation = function.FunctionHandler(context);
            Assert.Equal("Camera is online and streaming", cameraStatusConfirmation);
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
