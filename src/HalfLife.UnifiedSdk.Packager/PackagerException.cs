namespace HalfLife.UnifiedSdk.Packager
{
    internal sealed class PackagerException : Exception
    {
        public PackagerException()
        {
        }

        public PackagerException(string? message)
            : base(message)
        {
        }

        public PackagerException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
