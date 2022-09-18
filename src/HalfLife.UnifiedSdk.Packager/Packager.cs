using Microsoft.Extensions.FileSystemGlobbing;
using Serilog;
using Serilog.Core;
using System.Collections.Immutable;
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

        public static void CreatePackage(ILogger logger, PackagerOptions options)
        {
            var completePackageName = options.PackageName + PackageExtension;

            logger.Information("Creating archive {PackageName}", completePackageName);

            var includedFiles = options.ListOmittedFiles ? new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) : null;

            using var archive = ZipFile.Open(completePackageName, ZipArchiveMode.Create);

            foreach (var directory in options.Directories)
            {
                logger.Information("Adding mod directory \"{Path}\"", directory.Path);

                var matcher = new Matcher();

                matcher.AddIncludePatterns(directory.IncludePatterns);
                matcher.AddExcludePatterns(directory.ExcludePatterns);

                foreach (var file in matcher.GetResultsInFullPath(directory.Path))
                {
                    var relativePath = Path.GetRelativePath(options.RootDirectory, file);

                    includedFiles?.Add(file);

                    logger.Verbose("Adding file \"{RelativePath}\"", relativePath);

                    var newName = relativePath;

                    // Files ending with ".install" need to be renamed.
                    newName = Regex.Replace(newName, "\\.install$", "");

                    if (relativePath != newName)
                    {
                        logger.Verbose("Renaming \"{RelativePath}\" to \"{NewName}\"", relativePath, newName);
                    }

                    archive.CreateEntryFromFile(relativePath, newName);
                }

                if (options.ListOmittedFiles)
                {
                    var excludeMatcher = new Matcher();

                    excludeMatcher.AddIncludePatterns(directory.ExcludePatterns);

                    var excludedFiles = excludeMatcher
                        .GetResultsInFullPath(directory.Path)
                        .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

                    excludedFiles.ExceptWith(includedFiles!);

                    foreach (var file in excludedFiles)
                    {
                        var relativeFileName = Path.GetRelativePath(options.RootDirectory, file);
                        logger.Verbose("Excluded file \"{RelativePath}\"", relativeFileName);
                    }

                    foreach (var file in Directory.EnumerateFiles(directory.Path)
                        .Where(f => !includedFiles!.Contains(f) && !excludedFiles.Contains(f)))
                    {
                        var relativeFileName = Path.GetRelativePath(options.RootDirectory, file);
                        logger.Verbose("Ignored file \"{RelativePath}\"", relativeFileName);
                    }
                }
            }
        }

        public static void DeleteOldPackages(ILogger logger, string packageName, DirectoryInfo rootDirectory, string now)
        {
            var regex = new Regex($@"{Regex.Escape(packageName)}-(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d){PackageExtension}$");

            foreach (var file in rootDirectory.EnumerateFiles($"{packageName}-*{PackageExtension}"))
            {
                var match = regex.Match(file.FullName);

                if (match.Success)
                {
                    if (match.Groups[1].Captures[0].Value.CompareTo(now) < 0)
                    {
                        logger.Information("Deleting old package \"{PackageName}\"", file.FullName);
                        file.Delete();
                    }
                }
            }
        }
    }
}
