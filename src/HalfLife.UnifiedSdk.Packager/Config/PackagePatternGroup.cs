using HalfLife.UnifiedSdk.Utilities.Configuration;
using Newtonsoft.Json;

namespace HalfLife.UnifiedSdk.Packager.Config
{
    /// <summary>
    /// See the MSDN documentation on the Matcher class for more information on what kind of patterns are supported:
    /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.filesystemglobbing.matcher
    /// </summary>
    internal sealed class PackagePatternGroup
    {
        [JsonProperty(Required = Required.Always)]
        public List<PackageDirectoryPath> Paths { get; set; } = new();

        /// <summary>
        /// List of files and directories to package.
        /// Filenames ending with ".install" will be renamed to remove this extension after being added to the archive.
        /// </summary>
        [JsonProperty(Required = Required.Always, ItemConverterType = typeof(PathConverter))]
        public List<string> IncludePatterns { get; set; } = new();

        /// <summary>
        /// Files and directories to exclude from the archive.
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull, ItemConverterType = typeof(PathConverter))]
        public List<string> ExcludePatterns { get; set; } = new();
    }
}
