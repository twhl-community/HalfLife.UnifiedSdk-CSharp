using HalfLife.UnifiedSdk.Utilities.Logging;
using Serilog;
using System.Collections.Immutable;
using System.CommandLine;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace HalfLife.UnifiedSdk.MapUpgraderDocGenerator
{
    internal static class Program
    {
        private record struct Category(string DisplayName, int SortOrder);

        private const string RootNamespace = "HalfLife.UnifiedSdk.MapUpgrader.Upgrades.";
        private const string UpgradeNamePrefix = "T:" + RootNamespace;

        private static readonly ImmutableDictionary<string, Category> CategoryNames = new Dictionary<string, Category>
        {
            ["Common"] = new("Applied to all maps", 0),
            ["HalfLife"] = new("Half-Life-specific", 1),
            ["OpposingForce"] = new("Opposing Force-specific", 2),
            ["BlueShift"] = new("Blue Shift-specific", 3)
        }.ToImmutableDictionary();

        private static readonly Regex RedundantWhitespaceRegex = new("\n *");

        public static int Main(string[] args)
        {
            var xmlFileNameArgument = new Argument<FileInfo>("xml-file", description: "Name of the XML file to convert");

            var rootCommand = new RootCommand("Half-Life Unified SDK map upgrades documentation generator"
                +"\nOutput file uses input filename with '.md' extension")
            {
                xmlFileNameArgument
            };

            rootCommand.SetHandler((xmlFileName, logger) =>
            {
                if (!xmlFileName.Exists)
                {
                    logger.Error("The XML file \"{FileName}\" does not exist", xmlFileName.FullName);
                    return;
                }

                logger.Information("Reading documentation from file \"{FileName}\"", xmlFileName.FullName);

                var serializer = new XmlSerializer(typeof(XmlDocumentation));

                using var inputStream = xmlFileName.OpenText();

                var documentation = (XmlDocumentation?)serializer.Deserialize(inputStream);

                if (documentation is null)
                {
                    logger.Information("No documentation was read from the file");
                    return;
                }

                using var outputWriter = File.CreateText(Path.ChangeExtension(xmlFileName.FullName, "md"));

                WriteMarkdown(outputWriter, documentation, logger);
            }, xmlFileNameArgument, LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }

        private static void WriteMarkdown(StreamWriter writer, XmlDocumentation documentation, ILogger logger)
        {
            // Only output documentation that's in another namespace.
            var markdownDocumentation = documentation.Members
                .Where(m => m.Name.StartsWith(UpgradeNamePrefix) && m.Name.IndexOf('.', UpgradeNamePrefix.Length) != -1)
                .Select(ConvertMember)
                .ToList();

            var grouped = markdownDocumentation.GroupBy(d => d.Category);

            var ordered = grouped.OrderBy(g => GetOrder(g.Key));

            writer.WriteLine("Table of contents:");

            foreach (var group in ordered)
            {
                string category = group.Key;

                if (CategoryNames.TryGetValue(category, out var categoryInfo))
                {
                    category = categoryInfo.DisplayName;
                }

                writer.WriteLine($"* [{category}](#{category.ToLowerInvariant().Replace(' ', '-')})");
            }

            writer.WriteLine();

            foreach (var group in ordered)
            {
                string category = group.Key;

                if (CategoryNames.TryGetValue(category, out var categoryInfo))
                {
                    category = categoryInfo.DisplayName;
                }

                writer.WriteLine($"## {category}");
                writer.WriteLine();

                foreach (var doc in group.OrderBy(m => m.ClassName))
                {
                    writer.WriteLine($"### {doc.ClassName}");
                    writer.WriteLine();

                    var summary = doc.Summary;

                    // Get rid of the extra whitespace prefixing all but the first line.
                    summary = RedundantWhitespaceRegex.Replace(summary, "\n");
                    summary = ReplaceTags(summary);

                    if (summary.Contains('<'))
                    {
                        logger.Warning("Unhandled tags in documentation for \"{ClassName}\"", doc.ClassName);
                    }

                    writer.WriteLine(summary);
                    writer.WriteLine();
                }
            }
        }

        private static int GetOrder(string category)
        {
            if (CategoryNames.TryGetValue(category, out var categoryInfo))
            {
                return categoryInfo.SortOrder;
            }

            return int.MaxValue;
        }

        private static MarkdownMemberDocumentation ConvertMember(XmlMemberDocumentation input)
        {
            var truncatedName = input.Name[UpgradeNamePrefix.Length..];

            var namespaceDelimiter = truncatedName.LastIndexOf('.');

            var category = truncatedName[..namespaceDelimiter];
            var className = truncatedName[(namespaceDelimiter + 1)..];

            return new(category, className, input.Summary.Contents);
        }

        private static string ReplaceTags(string text)
        {
            // The contents of the summary tag are deserialized as XML so spaces between tags are removed.
            // This undoes that.
            text = text.Replace("><", "> <");
            text = text.Replace("<c>", "`");
            text = text.Replace("</c>", "`");

            return text;
        }
    }
}