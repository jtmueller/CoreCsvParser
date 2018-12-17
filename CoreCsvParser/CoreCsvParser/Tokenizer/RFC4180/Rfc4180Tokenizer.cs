// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CoreCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer 
    {
        private char _lastChar;
        private readonly char _quoteChar;
        private readonly char _delimiterChar;

        public RFC4180Tokenizer(Options options)
        {
            _quoteChar = options.QuoteCharacter;
            _delimiterChar = options.DelimiterCharacter;
        }

        public TokenEnumerable Tokenize(ReadOnlySpan<char> input)
        {
            return new TokenEnumerable(input, NextToken);
        }

        private ReadOnlySpan<char> NextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining, out bool foundToken)
        {
            chars = chars.TrimStart();

            if (chars.IsEmpty)
            {
                foundToken = _lastChar == _delimiterChar;
                _lastChar = (char)0;
                return remaining = ReadOnlySpan<char>.Empty;
            }

            char c = chars[0];
            _lastChar = c;

            if (c == _delimiterChar)
            {
                remaining = chars[1..];
                foundToken = true;
                return ReadOnlySpan<char>.Empty;
            }
            else
            {
                var result = ReadOnlySpan<char>.Empty;
                if (c == _quoteChar)
                {
                    result = ReadQuoted(chars, out chars);

                    chars = chars.TrimStart();

                    if (chars.Length <= 1)
                    {
                        remaining = ReadOnlySpan<char>.Empty;
                        foundToken = true;
                        return result;
                    }

                    if (chars[0] == _delimiterChar)
                    {
                        _lastChar = chars[0];
                        chars = chars[1..];
                    }

                    remaining = chars;
                    foundToken = true;
                    return result;
                }

                result = chars.ReadTo(_delimiterChar, out chars, trim: true);
                chars = chars.TrimStart();

                if (chars.IsEmpty)
                {
                    remaining = chars;
                    foundToken = true;
                    return result;
                }

                if (chars[0] == _delimiterChar)
                {
                    _lastChar = chars[0];
                    chars = chars[1..];
                }

                remaining = chars;
                foundToken = true;
                return result;
            }
        }

        private ReadOnlySpan<char> ReadQuoted(ReadOnlySpan<char> input, out ReadOnlySpan<char> remaining)
        {
            var chars = input;
            if (chars[0] == _quoteChar)
                chars = chars[1..];

            var result = chars.ReadTo(_quoteChar, out chars);

            if (chars[0] == _quoteChar)
                chars = chars[1..];

            if (chars.IsEmpty || chars[0] != _quoteChar)
            {
                remaining = chars;
                return result;
            }

            // If we got here, there's an escaped (doubled) quote char. We have to read to the next non-escaped quote,
            // and also un-escape any escaped quotes.

            // This allocation is unfortunate, but since we have to un-escape doubled quotes, we can't accomplish this
            // just by slicing. Since we're returning the value, we can't use a MemoryPool.

            Span<char> span = new char[input.Length];
            result.CopyTo(span);
            var curIdx = result.Length;
            do
            {
                span[curIdx++] = chars[0];
                chars = chars[1..];
                var read = chars.ReadTo(_quoteChar, out chars);
                read.CopyTo(span[curIdx..]);
                curIdx += read.Length;
                chars = chars[1..];
            } while (!chars.IsEmpty && chars[0] == _quoteChar);

            remaining = chars;
            return span[..curIdx];
        }

        public override string ToString() => $"RFC4180Tokenizer (Quote = {_quoteChar}, Delimiter = {_delimiterChar})";
    }
}
