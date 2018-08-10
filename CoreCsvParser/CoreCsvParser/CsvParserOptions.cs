﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using CoreCsvParser.Tokenizer;

namespace CoreCsvParser
{
    public class CsvParserOptions
    {
        public readonly ITokenizer Tokenizer;
        
        public readonly bool SkipHeader;

        public readonly string CommentCharacter;

        public readonly int DegreeOfParallelism;

        public readonly bool KeepOrder;

        public CsvParserOptions(bool skipHeader, char fieldsSeparator)
            : this(skipHeader, new QuotedStringTokenizer(fieldsSeparator))
        {
        }

        public CsvParserOptions(bool skipHeader, char fieldsSeparator, int degreeOfParallelism, bool keepOrder)
            : this(skipHeader, string.Empty, new QuotedStringTokenizer(fieldsSeparator), degreeOfParallelism, keepOrder)
        {
        }

        public CsvParserOptions(bool skipHeader, ITokenizer tokenizer)
            : this(skipHeader, string.Empty, tokenizer)
        {
        }

        public CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer)
            : this(skipHeader, commentCharacter, tokenizer, Environment.ProcessorCount, true)
        {
        }

        public CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer, int degreeOfParallelism, bool keepOrder)
        {
            SkipHeader = skipHeader;
            CommentCharacter = commentCharacter;
            Tokenizer = tokenizer;
            DegreeOfParallelism = degreeOfParallelism;
            KeepOrder = keepOrder;
        }

        public override string ToString()
        {
            return string.Format("CsvParserOptions (Tokenizer = {0}, SkipHeader = {1}, DegreeOfParallelism = {2}, KeepOrder = {3}, CommentCharacter = {4})",
                Tokenizer, SkipHeader, DegreeOfParallelism, KeepOrder, CommentCharacter);
        }
    }
}
