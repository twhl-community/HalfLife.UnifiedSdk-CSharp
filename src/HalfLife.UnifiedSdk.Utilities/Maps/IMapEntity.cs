using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    internal interface IMapEntity
    {
        /// <summary>
        /// Gets the entity's keyvalues. This is expected to perform conversions internally, so it is advised to cache the results.
        /// </summary>
        ImmutableDictionary<string, string> KeyValues { get; }

        bool IsWorldspawn { get; }

        void SetKeyValue(string key, string value);

        void RemoveKeyValue(string key);

        /// <summary>Removes all keyvalues except for the classname.</summary>
        void RemoveAllKeyValues();
    }
}
