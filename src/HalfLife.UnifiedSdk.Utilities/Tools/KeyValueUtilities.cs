using System;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Constants and utility functions for dealing with keyvalues.</summary>
    public static class KeyValueUtilities
    {
        /// <summary>The <c>classname</c> key.</summary>
        public const string ClassName = "classname";

        /// <summary>The <c>spawnflags</c> key.</summary>
        public const string SpawnFlags = "spawnflags";

        /// <summary>The <c>model</c> key.</summary>
        public const string Model = "model";

        /// <summary>The <c>targetname</c> key.</summary>
        public const string TargetName = "targetname";

        /// <summary>The <c>target</c> key.</summary>
        public const string Target = "target";

        /// <summary>The <c>globalname</c> key.</summary>
        public const string GlobalName = "globalname";

        /// <summary>The <c>delay</c> key.</summary>
        public const string Delay = "delay";

        /// <summary>The <c>origin</c> key.</summary>
        public const string Origin = "origin";

        /// <summary>The <c>angles</c> key.</summary>
        public const string Angles = "angles";

        /// <summary>The <c>rendermode</c> key.</summary>
        public const string RenderMode = "rendermode";

        /// <summary>The <c>renderamt</c> key.</summary>
        public const string RenderAmount = "renderamt";

        /// <summary>The <c>rendercolor</c> key.</summary>
        public const string RenderColor = "rendercolor";

        /// <summary>The <c>worldspawn</c> class name.</summary>
        public const string WorldspawnClassName = "worldspawn";

        /// <summary>
        /// A string containing the empty map. An empty map has a worldspawn entity with no additional entities or keyvalues.
        /// </summary>
        public static string EmptyMapString => $@"{{
""{ClassName}"" ""{WorldspawnClassName}""
}}
";

        /// <summary>Validates that <paramref name="value"/> is a valid class name.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is invalid.</exception>
        public static void ValidateClassName(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!Regex.IsMatch(value, @"^[_a-zA-Z]+[\w]*$"))
            {
                throw new ArgumentException($"Cannot set classname to invalid name \"{value}\"", nameof(value));
            }
        }

        /// <summary>Returns whether the given value is the <c>worldspawn</c> class name.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public static bool IsWorldspawnClass(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value == WorldspawnClassName;
        }

        /// <summary>Returns whether the given string is a BSP submodel index (<c>"*&lt;number&gt;"</c>).</summary>
        public static bool IsBSPSubModelIndex(string value)
        {
            return Regex.IsMatch(value, @"^\*\d+$");
        }

        /// <summary>Returns whether the given string is a BSP file name (<c>"maps/&lt;name&gt;.bsp"</c>).</summary>
        public static bool IsBSPModelFileName(string value)
        {
            return Regex.IsMatch(value, @"^maps(/\\).+\.bsp$");
        }

        /// <summary>Returns whether the given string is a studiomodel file name (<c>"maps/&lt;name&gt;.mdl"</c>).</summary>
        public static bool IsStudioModelFileName(string value)
        {
            return Regex.IsMatch(value, @"^models(/\\).+\.mdl$");
        }

        /// <summary>Returns whether the given string is a sprite model file name (<c>"maps/&lt;name&gt;.spr"</c>).</summary>
        public static bool IsSpriteModelFileName(string value)
        {
            return Regex.IsMatch(value, @"^sprites(/\\).+\.spr$");
        }

        /// <summary>Returns whether the given string is a any model file name.</summary>
        /// <seealso cref="IsBSPModelFileName(string)"/>
        /// <seealso cref="IsStudioModelFileName(string)"/>
        /// <seealso cref="IsSpriteModelFileName(string)"/>
        public static bool IsAnyModelFileName(string value)
        {
            return IsStudioModelFileName(value)
                || IsBSPModelFileName(value)
                || IsSpriteModelFileName(value);
        }

        /// <summary>Returns whether the given string is a wave sound file name (<c>"&lt;name&gt;.wav"</c>).</summary>
        public static bool IsSoundModelFileName(string value)
        {
            return Regex.IsMatch(value, @"^.+\.wav");
        }

        /// <summary>Tries to parse out the bsp submodel index from a value.</summary>
        /// <param name="value">Value to parse. Should be taken from a brush entity's <c>model</c> keyvalue.</param>
        /// <param name="index"></param>
        /// <returns><see langword="true"/> if the index could be succssfully parsed, <see langword="false"/> otherwise.</returns>
        /// <seealso cref="IsBSPSubModelIndex(string)"/>
        public static bool TryParseBSPSubModelIndex(string value, out int index)
        {
            if (IsBSPSubModelIndex(value))
            {
                value = value[1..];

                index = int.Parse(value);
                return true;
            }

            index = 0;
            return false;
        }
    }
}
