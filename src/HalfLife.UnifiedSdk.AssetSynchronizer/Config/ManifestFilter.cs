using HalfLife.UnifiedSdk.Utilities.Configuration;
using Newtonsoft.Json;

namespace HalfLife.UnifiedSdk.AssetSynchronizer.Config
{
    internal class ManifestFilter
    {
        /// <summary>
        /// Path relative to the root of the assets directory to start looking for files.
        /// </summary>
        [JsonConverter(typeof(PathConverter))]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Path relative to the root of the game directory to copy files to.
        /// </summary>
        [JsonConverter(typeof(PathConverter))]
        public string Destination { get; set; } = string.Empty;

        /// <summary>
        /// Pattern to filter files with. Supports wildcards.
        /// </summary>
        [JsonConverter(typeof(PathConverter))]
        public string Pattern { get; set; } = string.Empty;

        /// <summary>
        /// Whether to recurse into subdirectories. Default <see langword="false"/>.
        /// </summary>
        public bool Recursive { get; set; }
    }
}
