using System;
using System.Linq;
using System.Text;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>
    /// utility functions for strings.
    /// </summary>
    public static class StringUtilities
    {
        /// <summary>
        /// Concatenates two strings containing comments.
        /// If <paramref name="lhs"/> already has text in it <paramref name="rhs"/> will be added on a new line.
        /// </summary>
        public static string ConcatComments(string lhs, ReadOnlySpan<char> rhs)
        {
            var rhsString = rhs.Trim().ToString();

            if (string.IsNullOrEmpty(lhs))
            {
                lhs = rhsString;
            }
            else
            {
                lhs += '\n';
                lhs += rhsString;
            }

            return lhs;
        }

        /// <summary>
        /// Adds a prefix to every line.
        /// </summary>
        public static string AddPrefixToAllLines(string text, string prefix)
        {
            StringBuilder builder = new(text.Length + (text.Count(c => c == '\n') * prefix.Length));

            ReadOnlySpan<char> rawText = text;

            while (!rawText.IsEmpty)
            {
                int end = rawText.IndexOf('\n');

                if (end == -1)
                {
                    end = rawText.Length;
                }
                else
                {
                    ++end;
                }

                builder.Append(prefix);
                builder.Append(rawText[0..end]);

                rawText = rawText[end..];
            }

            return builder.ToString();
        }

        /// <summary>
        /// Indents lines in the string by <paramref name="indentLevel"/> levels using <paramref name="indentChar"/>.
        /// Line endings are assumed to be <c>\n</c> only.
        /// </summary>
        /// <param name="text">Text to indent.</param>
        /// <param name="indentLevel">How many occurrences of the indent character to add.</param>
        /// <param name="indentChar">Characer to use as indentation.</param>
        /// <param name="indentFirstLine">Whether to also indent the first line.</param>
        public static string IndentLines(string text, int indentLevel, char indentChar, bool indentFirstLine)
        {
            StringBuilder builder = new(text.Length + (text.Count(c => c == '\n') * indentLevel));

            ReadOnlySpan<char> rawText = text;

            bool isFirstLine = true;

            while (!rawText.IsEmpty)
            {
                int end = rawText.IndexOf('\n');

                if (end == -1)
                {
                    end = rawText.Length;
                }
                else
                {
                    ++end;
                }

                if (indentFirstLine || !isFirstLine)
                {
                    builder.Append(indentChar, indentLevel);
                }

                builder.Append(rawText[0..end]);

                if (isFirstLine)
                {
                    isFirstLine = false;
                }

                rawText = rawText[end..];
            }

            return builder.ToString();
        }
    }
}
