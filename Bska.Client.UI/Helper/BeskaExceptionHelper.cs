
namespace Bska.Client.UI.Helper
{
    using System;

    public class BeskaUnexpectedException : Exception
    {
        public BeskaUnexpectedException(string message)
            : base(message)
        {
        }

        public BeskaUnexpectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ColdCallFileFormatException : Exception
    {
        public ColdCallFileFormatException(string message)
            : base(message)
        {
        }
        public ColdCallFileFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
