namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>
    /// Possible values for <c>rendermode</c> keyvalue.
    /// </summary>
    public enum RenderMode
    {
        /// <summary>src</summary>
        RenderNormal,
        /// <summary>c*a+dest*(1-a)</summary>
        RenderTransColor,
        /// <summary>src*a+dest*(1-a)</summary>
        RenderTransTexture,
        /// <summary>src*a+dest -- No Z buffer checks</summary>
        RenderGlow,
        /// <summary>src*srca+dest*(1-srca)</summary>
        RenderTransAlpha,
        /// <summary>src*a+dest</summary>
        RenderTransAdd
    }
}
