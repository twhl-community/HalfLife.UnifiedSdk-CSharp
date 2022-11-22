namespace HalfLife.UnifiedSdk.KeyValueMatcher
{
    /// <summary>
    /// What to print when a match is found.
    /// </summary>
    internal enum PrintMode
    {
        /// <summary>
        /// Print nothing.
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// Print the key and value.
        /// </summary>
        KeyValue,

        /// <summary>
        /// Print the entire entity.
        /// </summary>
        Entity
    }
}
