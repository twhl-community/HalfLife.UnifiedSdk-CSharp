namespace HalfLife.UnifiedSdk.Formats
{
    public class ConverterException : Exception
    {
        /// <summary>
        /// If not <c>-1</c>, the line number where the error was encountered.
        /// </summary>
        public int LineNumber { get; init; } = -1;

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (LineNumber != -1)
                {
                    return $"Line {LineNumber}: {base.Message}";
                }

                return base.Message;
            }
        }

        /// <inheritdoc/>
        public ConverterException()
        {
        }

        /// <inheritdoc/>
        public ConverterException(string? message)
            : base(message)
        {
        }

        /// <inheritdoc/>
        public ConverterException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
