using HalfLife.UnifiedSdk.Utilities.Configuration;
using Newtonsoft.Json;

namespace HalfLife.UnifiedSdk.Packager
{
    internal class PackageManifest
    {
        /// <summary>
        /// List of files and directories to package.
        /// Filenames ending with ".install" will be renamed to remove this extension after being added to the archive.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(PathConverter))]
        public List<string> Include { get; set; } = new();

        /// <summary>
        /// Files to exclude from the archive.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(PathConverter))]
        public List<string> Exclude { get; set; } = new();
    }
}
