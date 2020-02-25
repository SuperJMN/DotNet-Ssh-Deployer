using System;

namespace DotNetSsh
{
    public class DeploymentException : Exception
    {
        public DeploymentException(string message) : base(message)
        {
        }

        public DeploymentException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}