using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Utility functionality for <c>scripted_sequence</c>.</summary>
    public static class ScriptedSequenceUtilities
    {
        /// <summary>The <c>scripted_sequence</c> classname.</summary>
        public const string ClassName = "scripted_sequence";

        /// <summary>The <c>m_iszEntity</c> key name.</summary>
        public const string TargetKey = "m_iszEntity";

        /// <summary>The <c>m_iszIdle</c> key name.</summary>
        public const string IdleKey = "m_iszIdle";

        /// <summary>The <c>m_iszPlay</c> key name.</summary>
        public const string PlayKey = "m_iszPlay";

        /// <summary>List of keys to check for animation names.</summary>
        public static readonly ImmutableList<string> KeysToCheck = ImmutableList.Create(IdleKey, PlayKey);

        /// <summary>
        /// Given an NPC class name, model name and a dictionary of animation names, renames all animations used in scripted sequences.
        /// </summary>
        /// <param name="context">Context to operate on.</param>
        /// <param name="npcName">Class name of the NPC to check. Can be null if there is no class.</param>
        /// <param name="modelName">Name of the model to check.</param>
        /// <param name="animationRemap">Dictionary of animation names to rename.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="context"/>, <paramref name="modelName"/> or <paramref name="animationRemap"/> are null.
        /// </exception>
        public static void RenameAnimations(
            MapUpgradeContext context, string? npcName, string modelName, ImmutableDictionary<string, string> animationRemap)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(modelName);
            ArgumentNullException.ThrowIfNull(animationRemap);

            foreach (var script in context.Map.Entities
                .OfClass(ClassName)
                .Where(e => e.ContainsKey(TargetKey)))
            {
                var target = context.Map.Entities
                    .WhereTargetName(script.GetString(TargetKey))
                    .FirstOrDefault();

                if (target is null)
                {
                    continue;
                }

                if (target.ClassName != npcName && target.GetModel() != modelName)
                {
                    continue;
                }

                foreach (var key in KeysToCheck)
                {
                    if (script.GetStringOrNull(key) is { } animation
                        && animationRemap.TryGetValue(animation, out var replacement))
                    {
                        script.SetString(key, replacement);
                    }
                }
            }
        }
    }
}
