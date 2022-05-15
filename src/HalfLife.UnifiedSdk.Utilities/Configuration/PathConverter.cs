using Newtonsoft.Json;
using System;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Configuration
{
    /// <summary>
    /// Converts paths to and from platform-specific form.
    /// </summary>
    public sealed class PathConverter : JsonConverter<string>
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            var path = value?.Replace(Path.DirectorySeparatorChar, '/');
            writer.WriteValue(path);
        }

        /// <inheritdoc/>
        public override string? ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return ((string?)reader.Value)?.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
