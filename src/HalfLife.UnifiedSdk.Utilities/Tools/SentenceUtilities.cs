using HalfLife.UnifiedSdk.Utilities.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Helper functions for dealing with sentences.</summary>
    public static class SentenceUtilities
    {
        private static readonly ImmutableDictionary<string, string> EntityKeyNames = new Dictionary<string, string>
        {
            { "scripted_sentence", "sentence" },
            { "ambient_generic", "message" }
        }.ToImmutableDictionary();

        /// <summary>Remaps sentences listed in <paramref name="replacementMap"/>.</summary>
        public static void ReplaceSentences(IEnumerable<Entity> entities, ImmutableDictionary<string, string> replacementMap)
        {
            ArgumentNullException.ThrowIfNull(entities);
            ArgumentNullException.ThrowIfNull(replacementMap);

            foreach (var entity in entities)
            {
                if (EntityKeyNames.TryGetValue(entity.ClassName, out var key))
                {
                    ReplaceSentencesCore(entity, key, replacementMap);
                }
            }
        }

        /// <summary>Remaps sentences listed in <paramref name="replacementMap"/>.</summary>
        public static void ReplaceSentences(Entity entity, string key, ImmutableDictionary<string, string> replacementMap)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(replacementMap);

            ReplaceSentencesCore(entity, key, replacementMap);
        }

        private static void ReplaceSentencesCore(Entity entity, string key, ImmutableDictionary<string, string> replacementMap)
        {
            if (entity.TryGetValue(key, out var name) && replacementMap.TryGetValue(name, out var replacement))
            {
                entity.SetString(key, replacement);
            }
        }
    }
}
