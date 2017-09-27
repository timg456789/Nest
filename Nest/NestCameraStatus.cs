
using System.Collections.Generic;

namespace Nest
{
    public class NestCameraStatus
    {

        public void ThrowExceptionIfAllCamerasArentOnlineAndStreaming(IList<NestCamera> cameras)
        {
            foreach (NestCamera camera in cameras)
            {
                ThrowExceptionIfCameraIsntOnlineAndStreaming(camera);
            }
        }

        public void ThrowExceptionIfCameraIsntOnlineAndStreaming(NestCamera camera)
        {
            if (IsOffline(camera))
            {
                throw new NestCameraOfflineException(camera.LastIsOnlineChange);
            }
        }

        public bool IsOffline(NestCamera camera)
        {
            return !camera.IsOnline || !camera.IsStreaming;
        }

    }
}
