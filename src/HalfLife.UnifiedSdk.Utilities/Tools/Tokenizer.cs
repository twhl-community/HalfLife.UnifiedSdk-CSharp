using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>
    /// Tokenizer that operates on a text span, extracting one token at a time.
    /// </summary>
    public ref struct Tokenizer
    {
        private readonly ref struct CommentParseResult
        {
            public readonly bool HasValue;
            public readonly ReadOnlySpan<char> Value;

            public CommentParseResult()
            {
                HasValue = false;
                Value = default;
            }

            public CommentParseResult(bool hasValue, ReadOnlySpan<char> value)
            {
                HasValue = hasValue;
                Value = value;
            }
        }

        /// <summary>
        /// List of special characters.
        /// </summary>
        public static readonly ImmutableArray<char> SpecialCharacters = ImmutableArray.Create('{', '}', '(', ')', '\'', ',');

        /// <summary>
        /// List of special characters including colon.
        /// </summary>
        public static readonly ImmutableArray<char> SpecialCharactersWithColon = SpecialCharacters.Add(':');

        private readonly ImmutableArray<char> _specialCharacters = SpecialCharacters;

        private readonly bool _allowComments;

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
        /// The type of the current token.
        /// </summary>
        public TokenType Type { get; private set; }

        /// <summary>
        /// Constructs a new tokenizer that operates on the given text span.
        /// </summary>
        /// <param name="text">Text to process.</param>
        /// <param name="ignoreColon">If <see langword="true"/> the colon character <c>':'</c> will be treated as special.</param>
        /// <param name="allowComments">
        /// If <see langword="true"/> comments will be parsed with <see cref="Type"/> set to <see cref="TokenType.Comment"/>.
        /// </param>
        public Tokenizer(ReadOnlySpan<char> text, bool ignoreColon = false, bool allowComments = false)
        {
            Text = text;
            Token = ReadOnlySpan<char>.Empty;
            Type = TokenType.None;

            RemainingText = Text;

            if (ignoreColon)
            {
                _specialCharacters = SpecialCharactersWithColon;
            }

            _allowComments = allowComments;
        }

        /// <summary>
        /// Extract the next token.
        /// </summary>
        /// <returns><see langword="true"/> if a token was extracted, <see langword="false"/> otherwise.</returns>
        public bool Next()
        {
            Token = ReadOnlySpan<char>.Empty;
            Type = TokenType.None;

            if (RemainingText.IsEmpty)
            {
                return false;
            }

            while (true)
            {
                RemainingText = RemainingText.TrimStart();

                var comment = TryParseComment();

                if (!comment.HasValue)
                {
                    break;
                }

                if (_allowComments)
                {
                    Token = comment.Value;
                    Type = TokenType.Comment;
                    return true;
                }
            }

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

        private CommentParseResult TryParseComment()
        {
            CommentParseResult result = new(false, new());

            while (!RemainingText.IsEmpty && RemainingText.StartsWith("//"))
            {
                var end = RemainingText.IndexOf('\n');

                if (end == -1)
                {
                    //There is no newline left, so the rest of the text is comments.
                    end = RemainingText.Length;
                }
                else
                {
                    ++end;
                }

                result = new CommentParseResult(true, RemainingText[2..end]);
                RemainingText = RemainingText[end..];

                // When parsing comments each comment line is its own token.
                if (_allowComments)
                {
                    break;
                }
            }

            return result;
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
                Type = TokenType.Text;
            }
            else
            {
                //Unexpected EOF.
                RemainingText = ReadOnlySpan<char>.Empty;
                Type = TokenType.None;
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

            Type = TokenType.Text;

            return true;
        }
    }
}
