
using System;
using System.Collections.Generic;

namespace Nest
{
    public class NestCameraStatus
    {
        public void ThrowExceptionIfAllCamerasArentOnlineAndStreaming(string endUserAccessToken)
        {
            var nestClient = new NestClient(endUserAccessToken);
            List<NestCamera> cameras = nestClient.GetCameras();

            foreach (NestCamera camera in cameras)
            {
                if (!camera.IsOnline || !camera.IsStreaming)
                {
                    var msg = "The camera is either offline or isn't streaming video! Last is online change: " +
                              camera.LastIsOnlineChange + " UTC";
                    throw new Exception(msg);
                }
            }
        }
    }
}
