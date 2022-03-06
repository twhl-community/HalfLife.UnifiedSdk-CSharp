namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    /// <summary>The type of content a map has.</summary>
    public enum MapContentType
    {
        /// <summary>A map source, like <c>.map</c>.</summary>
        Source = 0,

        /// <summary>
        /// A compiled map, like <c>.bsp</c> or <c>.ent</c>.
        /// Compiled maps have a model key for brush entities filled in with the brush model index.
        /// </summary>
        Compiled
    }
}
