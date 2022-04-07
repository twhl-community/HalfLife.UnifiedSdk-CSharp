using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>
    /// Tokenizer that operates on a text span, extracting one token at a time.
    /// </summary>
    public ref struct Tokenizer
    {
        /// <summary>
        /// List of special characters.
        /// </summary>
        public static readonly ImmutableArray<char> SpecialCharacters = ImmutableArray.Create('{', '}', '(', ')', '\'', ',');

        /// <summary>
        /// List of special characters including colon.
        /// </summary>
        public static readonly ImmutableArray<char> SpecialCharactersWithColon = SpecialCharacters.Add(':');

        private readonly ImmutableArray<char> _specialCharacters = SpecialCharacters;

        /// <summary>
        /// Text span being operated on.
        /// </summary>
        public ReadOnlySpan<char> Text { get; }

        /// <summary>
        /// The current token, or an empty span if no token has been extracted.
        /// </summary>
        public ReadOnlySpan<char> Token { get; private set; }

        /// <summary>
        /// The text that remains to be processed.
        /// </summary>
        public ReadOnlySpan<char> RemainingText { get; private set; }

        /// <summary>
        /// Constructs a new tokenizer that operates on the given text span.
        /// </summary>
        /// <param name="text">Text to process.</param>
        /// <param name="ignoreColon">If <see langword="true"/> the colon character <c>':'</c> will be treated as special.</param>
        public Tokenizer(ReadOnlySpan<char> text, bool ignoreColon = false)
        {
            Text = text;
            Token = ReadOnlySpan<char>.Empty;

            RemainingText = Text;

            if (ignoreColon)
            {
                _specialCharacters = SpecialCharactersWithColon;
            }
        }

        /// <summary>
        /// Extract the next token.
        /// </summary>
        /// <returns><see langword="true"/> if a token was extracted, <see langword="false"/> otherwise.</returns>
        public bool Next()
        {
            Token = ReadOnlySpan<char>.Empty;

            if (RemainingText.IsEmpty)
            {
                return false;
            }

            do
            {
                RemainingText = RemainingText.TrimStart();
            }
            while (SkipComments());

            if (RemainingText.IsEmpty)
            {
                return false;
            }

            if (RemainingText.StartsWith("\""))
            {
                return NextQuotedToken();
            }
            else
            {
                return NextUnquotedToken();
            }
        }

        private bool SkipComments()
        {
            bool skipped = false;

            while (!RemainingText.IsEmpty && RemainingText.StartsWith("//"))
            {
                skipped = true;

                var end = RemainingText.IndexOf('\n');

                if (end != -1)
                {
                    RemainingText = RemainingText[(end + 1)..];
                }
                else
                {
                    //There is no newline left, so the rest of the text is comments.
                    RemainingText = ReadOnlySpan<char>.Empty;
                }
            }

            return skipped;
        }

        private bool NextQuotedToken()
        {
            int endIndex = 1;

            while (endIndex < RemainingText.Length)
            {
                if (RemainingText[endIndex] == '\"')
                {
                    break;
                }

                ++endIndex;
            }

            Token = RemainingText[1..endIndex];

            if (endIndex < RemainingText.Length && RemainingText[endIndex] == '\"')
            {
                RemainingText = RemainingText[(endIndex + 1)..];
            }
            else
            {
                //Unexpected EOF.
                RemainingText = ReadOnlySpan<char>.Empty;
            }

            return true;
        }

        private bool NextUnquotedToken()
        {
            int endIndex = 0;

            //If it's a special character then it must be extracted as a single character token regardless of whether there are more characters.
            if (_specialCharacters.Contains(RemainingText[0]))
            {
                endIndex = 1;
            }
            else
            {
                while (endIndex < RemainingText.Length)
                {
                    if (char.IsWhiteSpace(RemainingText[endIndex]))
                    {
                        break;
                    }

                    ++endIndex;
                }
            }

            Token = RemainingText[0..endIndex];

            RemainingText = RemainingText[endIndex..];

            return true;
        }
    }
}
