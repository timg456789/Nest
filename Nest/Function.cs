
using System;
using System.Collections.Generic;
using System.IO;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Nest
{
    public class Function
    {
        public string FunctionHandler(ILambdaContext context)
        {
            string accessToken = Environment.GetEnvironmentVariable("access_token");
            string s3Bucket = Environment.GetEnvironmentVariable("s3Bucket");
            string s3Key = Environment.GetEnvironmentVariable("s3Key");

            var nestCameraStatus = new NestCameraStatus();

            var nestClient = new NestClient(accessToken, new ConsoleLogger());
            List<NestCamera> cameras  = nestClient.GetCameras();
            nestCameraStatus.ThrowExceptionIfAllCamerasArentOnlineAndStreaming(cameras);

            List<string> savedImages = new List<string>();
            foreach (var camera in cameras)
            {
                var snapshotJpg = nestClient.GetSnapshotJpg(camera);
                var key = $"{s3Key}/{DateTime.UtcNow:yyyy-MM-ddTHH-mm-ssZ}-{Guid.NewGuid().ToString().Substring(0, 6)}.jpg";
                Console.WriteLine("Putting into: " + s3Bucket + "/" + key);
                using (Stream snapshotJpgStream = new MemoryStream(snapshotJpg))
                {
                    PutObjectRequest request = new PutObjectRequest
                    {
                        BucketName = s3Bucket,
                        Key = key,
                        InputStream = snapshotJpgStream
                    };
                    var s3Client = new AmazonS3Client(new AmazonS3Config {RegionEndpoint = RegionEndpoint.USEast1});
                    PutObjectResponse putResult = s3Client.PutObjectAsync(request).Result;
                    savedImages.Add($"status{(int)putResult.HttpStatusCode} bucket {s3Bucket}{key}");
                }
            }

            return "Camera is online and streaming. Snapshot saved " + string.Join(", ", savedImages);
        }
    }
}
