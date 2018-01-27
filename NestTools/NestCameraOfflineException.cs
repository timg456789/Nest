using System;

namespace NestTools
{
    public class NestCameraOfflineException : Exception
    {
        public NestCameraOfflineException(DateTime lastIsOnlineChange)
            : base("The camera is either offline or isn't streaming video! Last is online change: " +
                   lastIsOnlineChange + " UTC")
        {
            
        }
    }
}
