
using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;

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

            var nestClient = new NestClient(accessToken, new ConsoleLogger());
            List<NestCamera> cameras  = nestClient.GetCameras();
            nestCameraStatus.ThrowExceptionIfAllCamerasArentOnlineAndStreaming(cameras);

            return "Camera is online and streaming";
        }
    }
}
