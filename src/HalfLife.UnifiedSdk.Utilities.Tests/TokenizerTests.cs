using HalfLife.UnifiedSdk.Utilities.Tools;
using Xunit;

namespace HalfLife.UnifiedSdk.Utilities.Tests
{
    public class TokenizerTests
    {
        private static void TestSingleToken(string text, string token)
        {
            var tokenizer = new Tokenizer(text);

            Assert.True(tokenizer.Next());
            Assert.Equal(token, tokenizer.Token.ToString());

            Assert.False(tokenizer.Next());
            Assert.True(tokenizer.Token.IsEmpty);
        }

        private static void TestTwoTokens(string text, string token1, string token2)
        {
            var tokenizer = new Tokenizer(text);

            Assert.True(tokenizer.Next());
            Assert.Equal(token1, tokenizer.Token.ToString());

            Assert.True(tokenizer.Next());
            Assert.Equal(token2, tokenizer.Token.ToString());

            Assert.False(tokenizer.Next());
            Assert.True(tokenizer.Token.IsEmpty);
        }

        [Fact]
        public void Default_ExtractsNothing()
        {
            var tokenizer = new Tokenizer();

            Assert.True(tokenizer.Text.IsEmpty);
            Assert.True(tokenizer.Token.IsEmpty);

            Assert.False(tokenizer.Next());
            Assert.True(tokenizer.Token.IsEmpty);
        }

        [Fact]
        public void EmptyString_ExtractsNothing()
        {
            var tokenizer = new Tokenizer("");

            Assert.True(tokenizer.Text.IsEmpty);
            Assert.True(tokenizer.Token.IsEmpty);

            Assert.False(tokenizer.Next());
            Assert.True(tokenizer.Token.IsEmpty);
        }

        [Fact]
        public void Whitespace_ExtractsNothing()
        {
            var tokenizer = new Tokenizer(" \t\n\r");

            Assert.True(tokenizer.Token.IsEmpty);

            Assert.False(tokenizer.Next());
            Assert.True(tokenizer.Token.IsEmpty);
        }

        [Fact]
        public void SingleUnquotedToken_ExtractsToken() => TestSingleToken("foo", "foo");

        [Fact]
        public void SingleSpecialToken_ExtractsToken() => TestSingleToken("{", "{");

        [Theory]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("\'")]
        [InlineData(",")]
        public void SingleSpecialTokenWithExtra_ExtractsTokenOnly(string token)
        {
            var tokenizer = new Tokenizer($"{token}foo");

            Assert.True(tokenizer.Next());
            Assert.Equal(token, tokenizer.Token.ToString());

            Assert.True(tokenizer.Next());
            Assert.Equal("foo", tokenizer.Token.ToString());
        }

        [Fact]
        public void SingleColonTokenWithExtraAndNoColonEnabled_ExtractsWholeToken()
        {
            var tokenizer = new Tokenizer(":foo", false);

            Assert.True(tokenizer.Next());
            Assert.Equal(":foo", tokenizer.Token.ToString());
        }

        [Fact]
        public void SingleColonTokenWithExtraAndColonEnabled_ExtractsTokenOnly()
        {
            var tokenizer = new Tokenizer(":foo", true);

            Assert.True(tokenizer.Next());
            Assert.Equal(":", tokenizer.Token.ToString());
        }

        [Fact]
        public void TwoTokens_ExtractsBothTokens() => TestTwoTokens("foo bar", "foo", "bar");

        [Fact]
        public void TwoTokensWithNewline_ExtractsBothTokens() => TestTwoTokens("foo\nbar", "foo", "bar");

        [Fact]
        public void SingleQuotedToken_ExtractsToken() => TestSingleToken("\"foo\"", "foo");

        [Fact]
        public void SingleQuotedTokenWithoutEndQuote_ExtractsToken() => TestSingleToken("\"foo", "foo");

        [Fact]
        public void TwoQuotedTokensWithoutEndQuote_ExtractsTokens()
        {
            var tokenizer = new Tokenizer("\"foo \"bar\"");

            Assert.True(tokenizer.Next());
            Assert.Equal("foo ", tokenizer.Token.ToString());

            Assert.True(tokenizer.Next());
            Assert.Equal("bar\"", tokenizer.Token.ToString());

            Assert.False(tokenizer.Next());
            Assert.Equal("", tokenizer.Token.ToString());
        }

        [Fact]
        public void TwoTokensWithOneQuoted_ExtractsBothTokens() => TestTwoTokens("\"foo\" bar", "foo", "bar");

        [Fact]
        public void CommentWithoutNewline_ExtractsNothing()
        {
            var tokenizer = new Tokenizer("//comment");

            Assert.False(tokenizer.Next());
            Assert.True(tokenizer.Token.IsEmpty);
        }

        [Fact]
        public void CommentWithNewline_ExtractsNothing()
        {
            var tokenizer = new Tokenizer("//comment\n");

            Assert.False(tokenizer.Next());
            Assert.True(tokenizer.Token.IsEmpty);
        }

        [Fact]
        public void CommentWithToken_ExtractsToken()
        {
            var tokenizer = new Tokenizer("//comment\nfoo");

            Assert.True(tokenizer.Next());
            Assert.Equal("foo", tokenizer.Token.ToString());
        }
    }
}
