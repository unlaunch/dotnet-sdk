using System;

namespace io.unlaunch.exception
{
    public class UnlaunchHttpException : Exception
    {
        public UnlaunchHttpException(string msg) : base(msg)
        {

        }
    }
}
