using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>
    /// Helper functions for converting between strings and floating point data types using the invariant culture,
    /// which matches the format used by the game.
    /// </summary>
    public static class ParsingUtilities
    {
        /// <summary>
        /// Tries to parse a <see cref="double"/> out of a <see cref="string"/> using the invariant culture.
        /// </summary>
        public static bool TryParseDouble(string s, [MaybeNullWhen(false)] out double result)
        {
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out result);
        }

        /// <summary>
        /// Returns the string representation of <paramref name="value"/> using the invariant culture.
        /// </summary>
        public static string DoubleToString(double value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Tries to parse a <see cref="float"/> out of a <see cref="string"/> using the invariant culture.
        /// </summary>
        public static bool TryParseFloat(string s, [MaybeNullWhen(false)] out float result)
        {
            return float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out result);
        }

        /// <summary>
        /// Returns the string representation of <paramref name="value"/> using the invariant culture.
        /// </summary>
        public static string FloatToString(float value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Parses <paramref name="value"/> into an array containing the components of a vector.
        /// </summary>
        public static string[] ParseVectorComponents(string value)
        {
            return Regex.Split(value, @"\s+");
        }

        private static ReadOnlySpan<float> ParseVectorValues(string value, Span<float> values)
        {
            values.Clear();

            var parts = ParseVectorComponents(value);

            //The game parses out up to values.Length components.
            //Any more is an error. A map that has too many vector components shouldn't be processed successfully.
            if (parts.Length > values.Length)
            {
                throw new ArgumentException($"Too many values ({parts.Length}) for vector of {values.Length}", nameof(value));
            }

            for (int i = 0; i < parts.Length; ++i)
            {
                _ = TryParseFloat(parts[i], out values[i]);
            }

            return values;
        }

        /// <summary>
        /// Parses a <see cref="Vector2"/> out of a <see cref="string"/> using the invariant culture.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> does not contain a <see cref="Vector2"/>.</exception>
        public static Vector2 ParseVector2(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var values = ParseVectorValues(value, stackalloc float[2]);

            return new Vector2(values[0], values[1]);
        }

        /// <summary>
        /// Returns the string representation of <see cref="Vector2"/> using the invariant culture.
        /// </summary>
        public static string Vector2ToString(Vector2 value)
        {
            return FormattableString.Invariant($"{value.X} {value.Y}");
        }

        /// <summary>
        /// Parses a <see cref="Vector3"/> out of a <see cref="string"/> using the invariant culture.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> does not contain a <see cref="Vector3"/>.</exception>
        public static Vector3 ParseVector3(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var values = ParseVectorValues(value, stackalloc float[3]);

            return new Vector3(values[0], values[1], values[2]);
        }

        /// <summary>
        /// Returns the string representation of <see cref="Vector3"/> using the invariant culture.
        /// </summary>
        public static string Vector3ToString(Vector3 value)
        {
            return FormattableString.Invariant($"{value.X} {value.Y} {value.Z}");
        }

        /// <summary>
        /// Parses a <see cref="Vector4"/> out of a <see cref="string"/> using the invariant culture.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> does not contain a <see cref="Vector4"/>.</exception>
        public static Vector4 ParseVector4(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var values = ParseVectorValues(value, stackalloc float[4]);

            return new Vector4(values[0], values[1], values[2], values[3]);
        }

        /// <summary>
        /// Returns the string representation of <see cref="Vector4"/> using the invariant culture.
        /// </summary>
        public static string Vector4ToString(Vector4 value)
        {
            return FormattableString.Invariant($"{value.X} {value.Y} {value.Z} {value.W}");
        }
    }
}
