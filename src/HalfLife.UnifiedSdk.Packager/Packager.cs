using Microsoft.Extensions.FileSystemGlobbing;
using Serilog;
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

        public static void CreatePackage(
            ILogger logger, string packageName, string rootDirectory, IEnumerable<PackageDirectory> directories, bool verbose)
        {
            var completePackageName = packageName + PackageExtension;

            logger.Information("Creating archive {PackageName}", completePackageName);

            using var archive = ZipFile.Open(completePackageName, ZipArchiveMode.Create);

            foreach (var directory in directories)
            {
                logger.Information("Adding mod directory \"{Path}\"", directory.Path);

                var matcher = new Matcher();

                matcher.AddIncludePatterns(directory.IncludePatterns);
                matcher.AddExcludePatterns(directory.ExcludePatterns);

                foreach (var file in matcher.GetResultsInFullPath(directory.Path))
                {
                    var relativePath = Path.GetRelativePath(rootDirectory, file);

                    if (verbose)
                    {
                        logger.Information("Adding file \"{RelativePath}\"", relativePath);
                    }

                    var newName = relativePath;

                    // Files ending with ".install" need to be renamed.
                    newName = Regex.Replace(newName, "\\.install$", "");

                    if (relativePath != newName && verbose)
                    {
                        logger.Information("Renaming \"{RelativePath}\" to \"{NewName}\"", relativePath, newName);
                    }

                    archive.CreateEntryFromFile(relativePath, newName);
                }
            }
        }
    }
}
