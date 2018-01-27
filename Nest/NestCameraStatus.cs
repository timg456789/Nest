﻿
using System.Collections.Generic;
using Nest.Models;

namespace Nest
{
    public class NestCameraStatus
    {

        public void ThrowExceptionIfAllCamerasArentOnlineAndStreaming(IList<NestCameraJson> cameras)
        {
            foreach (NestCameraJson camera in cameras)
            {
                ThrowExceptionIfCameraIsntOnlineAndStreaming(camera);
            }
        }

        public void ThrowExceptionIfCameraIsntOnlineAndStreaming(NestCameraJson cameraJson)
        {
            if (IsOffline(cameraJson))
            {
                throw new NestCameraOfflineException(cameraJson.LastIsOnlineChange);
            }
        }

        public bool IsOffline(NestCameraJson cameraJson)
        {
            return !cameraJson.IsOnline || !cameraJson.IsStreaming;
        }

    }
}
