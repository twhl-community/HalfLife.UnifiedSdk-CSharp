namespace HalfLife.UnifiedSdk.Utilities.Games
{
    /// <summary>Category that a map belongs to.</summary>
    public enum MapCategory
    {
        /// <summary>Campaign maps (c*a*, of*a*, ba_*, etc).</summary>
        Campaign = 0,

        /// <summary>Training maps (Hazard course, Boot camp).</summary>
        Training,

        /// <summary>Multiplayer maps. Also includes co-op and CTF maps.</summary>
        Multiplayer
    }
}
