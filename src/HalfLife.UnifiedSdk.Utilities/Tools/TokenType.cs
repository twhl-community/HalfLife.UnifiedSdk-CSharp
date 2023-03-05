namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>
    /// Possible values for the <see cref="Tokenizer"/> token type.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// No token was parsed.
        /// </summary>
        None = 0,

        /// <summary>
        /// A valid token was parsed.
        /// </summary>
        Text,

        /// <summary>
        /// A comment was parsed.
        /// </summary>
        Comment
    }
}
