using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("End user access token is required e.g. " +
                    "dotnet NestDotNetCore.dll ACCESS_TOKEN");
            }

            Check_Camera_Status(args[0]);
            Console.WriteLine("Camera Status Check Passes");
        }

        public static void Check_Camera_Status(string endUserAccessToken)
        {
            var nestClient = new NestClient(endUserAccessToken);
            List<NestCamera> cameras = nestClient.GetCameras();

            foreach (NestCamera camera in cameras)
            {
                if (!camera.IsOnline || !camera.IsStreaming)
                {
                    throw new Exception("The camera is either offline or isn't streaming video! Last is online change: " + camera.LastIsOnlineChange + " UTC");
                }
            }
        }

        public void CreateAuthTokenFromPin(string productId, string productSecret, string customerAuthPin)
        {
            var endUserAuthClient = new EndUserAuthenticationClient();
            var endUserAuth = endUserAuthClient.CreateAuthTokenFromPin(
                productId,
                productSecret,
                customerAuthPin);

            Assert.IsFalse(string.IsNullOrWhiteSpace(endUserAuth.AccessToken));
            Assert.AreEqual(315360000, endUserAuth.ExpiresIn);
        }
    }
}
