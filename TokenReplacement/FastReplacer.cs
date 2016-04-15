using System;
using System.Collections.Generic;
using System.Text;

namespace GeeksWithBlogsToMarkdown.TokenReplacement
{
    /// <summary>
    /// FastReplacer is a utility class similar to StringBuilder, with fast Replace function.
    /// FastReplacer is limited to replacing only properly formatted tokens.
    /// Use ToString() function to get the final text.
    /// </summary>
    public class FastReplacer
    {
        private readonly string _tokenOpen;
        private readonly string _tokenClose;

        /// <summary>
        /// All tokens that will be replaced must have same opening and closing delimiters, such as "{" and "}".
        /// </summary>
        /// <param name="tokenOpen">Opening delimiter for tokens.</param>
        /// <param name="tokenClose">Closing delimiter for tokens.</param>
        /// <param name="caseSensitive">Set caseSensitive to false to use case-insensitive search when replacing tokens.</param>
        public FastReplacer(string tokenOpen, string tokenClose, bool caseSensitive = true)
        {
            if (string.IsNullOrEmpty(tokenOpen) || string.IsNullOrEmpty(tokenClose))
                throw new ArgumentException("Token must have opening and closing delimiters, such as \"{\" and \"}\".");

            _tokenOpen = tokenOpen;
            _tokenClose = tokenClose;

            var stringComparer = caseSensitive ? StringComparer.Ordinal : StringComparer.InvariantCultureIgnoreCase;
            _occurrencesOfToken = new Dictionary<string, List<TokenOccurrence>>(stringComparer);
        }

        private readonly FastReplacerSnippet _rootSnippet = new FastReplacerSnippet("");

        private class TokenOccurrence
        {
            public FastReplacerSnippet Snippet;
            public int Start; // Position of a token in the snippet.
            public int End; // Position of a token in the snippet.
        }

        private readonly Dictionary<string, List<TokenOccurrence>> _occurrencesOfToken;

        public void Append(string text)
        {
            var snippet = new FastReplacerSnippet(text);
            _rootSnippet.Append(snippet);
            ExtractTokens(snippet);
        }

        /// <returns>Returns true if the token was found, false if nothing was replaced.</returns>
        public bool Replace(string token, string text)
        {
            ValidateToken(token, text, false);
            List<TokenOccurrence> occurrences;
            if (_occurrencesOfToken.TryGetValue(token, out occurrences) && occurrences.Count > 0)
            {
                _occurrencesOfToken.Remove(token);
                var snippet = new FastReplacerSnippet(text);
                foreach (var occurrence in occurrences)
                    occurrence.Snippet.Replace(occurrence.Start, occurrence.End, snippet);
                ExtractTokens(snippet);
                return true;
            }
            return false;
        }

        /// <returns>Returns true if the token was found, false if nothing was replaced.</returns>
        public bool InsertBefore(string token, string text)
        {
            ValidateToken(token, text, false);
            List<TokenOccurrence> occurrences;
            if (_occurrencesOfToken.TryGetValue(token, out occurrences) && occurrences.Count > 0)
            {
                var snippet = new FastReplacerSnippet(text);
                foreach (var occurrence in occurrences)
                    occurrence.Snippet.InsertBefore(occurrence.Start, snippet);
                ExtractTokens(snippet);
                return true;
            }
            return false;
        }

        /// <returns>Returns true if the token was found, false if nothing was replaced.</returns>
        public bool InsertAfter(string token, string text)
        {
            ValidateToken(token, text, false);
            List<TokenOccurrence> occurrences;
            if (_occurrencesOfToken.TryGetValue(token, out occurrences) && occurrences.Count > 0)
            {
                var snippet = new FastReplacerSnippet(text);
                foreach (var occurrence in occurrences)
                    occurrence.Snippet.InsertAfter(occurrence.End, snippet);
                ExtractTokens(snippet);
                return true;
            }
            return false;
        }

        public bool Contains(string token)
        {
            ValidateToken(token, token, false);
            List<TokenOccurrence> occurrences;
            if (_occurrencesOfToken.TryGetValue(token, out occurrences))
                return occurrences.Count > 0;
            return false;
        }

        private void ExtractTokens(FastReplacerSnippet snippet)
        {
            int last = 0;
            while (last < snippet.Text.Length)
            {
                // Find next token position in snippet.Text:
                int start = snippet.Text.IndexOf(_tokenOpen, last, StringComparison.InvariantCultureIgnoreCase);
                if (start == -1)
                    return;
                int end = snippet.Text.IndexOf(_tokenClose, start + _tokenOpen.Length, StringComparison.InvariantCultureIgnoreCase);
                if (end == -1)
                    throw new ArgumentException($"Token is opened but not closed in text \"{snippet.Text}\".");
                int eol = snippet.Text.IndexOf('\n', start + _tokenOpen.Length);
                if (eol != -1 && eol < end)
                {
                    last = eol + 1;
                    continue;
                }

                // Take the token from snippet.Text:
                end += _tokenClose.Length;
                string token = snippet.Text.Substring(start, end - start);
                string context = snippet.Text;
                ValidateToken(token, context, true);

                // Add the token to the dictionary:
                var tokenOccurrence = new TokenOccurrence { Snippet = snippet, Start = start, End = end };
                List<TokenOccurrence> occurrences;
                if (_occurrencesOfToken.TryGetValue(token, out occurrences))
                    occurrences.Add(tokenOccurrence);
                else
                    _occurrencesOfToken.Add(token, new List<TokenOccurrence> { tokenOccurrence });

                last = end;
            }
        }

        private void ValidateToken(string token, string context, bool alreadyValidatedStartAndEnd)
        {
            if (!alreadyValidatedStartAndEnd)
            {
                if (!token.StartsWith(_tokenOpen, StringComparison.InvariantCultureIgnoreCase))
                    throw new ArgumentException(
                        $"Token \"{token}\" should start with \"{_tokenOpen}\". Used with text \"{context}\".");
                int closePosition = token.IndexOf(_tokenClose, StringComparison.InvariantCultureIgnoreCase);
                if (closePosition == -1)
                    throw new ArgumentException(
                        $"Token \"{token}\" should end with \"{_tokenClose}\". Used with text \"{context}\".");
                if (closePosition != token.Length - _tokenClose.Length)
                    throw new ArgumentException(
                        $"Token \"{token}\" is closed before the end of the token. Used with text \"{context}\".");
            }

            if (token.Length == _tokenOpen.Length + _tokenClose.Length)
                throw new ArgumentException($"Token has no body. Used with text \"{context}\".");
            if (token.Contains("\n"))
                throw new ArgumentException($"Unexpected end-of-line within a token. Used with text \"{context}\".");
            if (token.IndexOf(_tokenOpen, _tokenOpen.Length, StringComparison.InvariantCultureIgnoreCase) != -1)
                throw new ArgumentException(
                    $"Next token is opened before a previous token was closed in token \"{token}\". Used with text \"{context}\".");
        }

        public override string ToString()
        {
            int totalTextLength = _rootSnippet.GetLength();
            var sb = new StringBuilder(totalTextLength);
            _rootSnippet.ToString(sb);
            if (sb.Length != totalTextLength)
                throw new InvalidOperationException(
                    $"Internal error: Calculated total text length ({totalTextLength}) is different from actual ({sb.Length}).");
            return sb.ToString();
        }
    }
}
