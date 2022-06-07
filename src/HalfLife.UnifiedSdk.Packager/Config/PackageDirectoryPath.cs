using HalfLife.UnifiedSdk.Utilities.Configuration;
using Newtonsoft.Json;

namespace HalfLife.UnifiedSdk.Packager.Config
{
    internal sealed class PackageDirectoryPath
    {
        /// <summary>
        /// Path to the directory to package.
        /// This is a path relative to the game directory.
        /// The symbol <c>%ModDirectory%</c> is replaced with the mod directory.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(PathConverter))]
        public string Path { get; set; } = string.Empty;

        public bool Required { get; set; } = true;
    }
}
