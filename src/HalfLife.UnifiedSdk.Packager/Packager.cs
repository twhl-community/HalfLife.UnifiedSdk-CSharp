using Microsoft.Extensions.FileSystemGlobbing;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Packager
{
    /// <summary>
    /// Creates a mod package from a mod installation.
    /// </summary>
    internal static class Packager
    {
        public const string PackageExtension = ".zip";

        public static void CreatePackage(IConsole console, string packageName, string rootDirectory, IEnumerable<PackageDirectory> directories)
        {
            var completePackageName = packageName + PackageExtension;

            console.Out.WriteLine($"Creating archive {completePackageName}");

            using var archive = ZipFile.Open(completePackageName, ZipArchiveMode.Create);

            foreach (var directory in directories)
            {
                console.Out.WriteLine($"Adding mod directory \"{directory.Path}\"");

                var matcher = new Matcher();

                matcher.AddIncludePatterns(directory.IncludePatterns);
                matcher.AddExcludePatterns(directory.ExcludePatterns);

                foreach (var file in matcher.GetResultsInFullPath(directory.Path))
                {
                    var relativePath = Path.GetRelativePath(rootDirectory, file);

                    console.Out.WriteLine($"Adding file \"{relativePath}\"");

                    var newName = relativePath;

                    // Files ending with ".install" need to be renamed.
                    newName = Regex.Replace(newName, "\\.install$", "");

                    if (relativePath != newName)
                    {
                        console.Out.WriteLine($"Renaming \"{relativePath}\" to \"{newName}\"");
                    }

                    archive.CreateEntryFromFile(relativePath, newName);
                }
            }
        }
    }
}
