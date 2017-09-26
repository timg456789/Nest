
using System;
using System.IO;
using System.Linq;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Nest
{
    public class Function
    {
        public string FunctionHandler(ILambdaContext context)
        {
            string accessToken = Environment.GetEnvironmentVariable("access_token");
            var nestCameraStatus = new NestCameraStatus();
            nestCameraStatus.ThrowExceptionIfAllCamerasArentOnlineAndStreaming(accessToken);
            return "Camera is online and streaming";
        }
    }
}
