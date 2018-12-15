// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreCsvParser.Mapping;

namespace CoreCsvParser
{
    public class CsvParser<TEntity> : ICsvParser<TEntity> where TEntity : new()
    {
        private readonly CsvMapping<TEntity> _mapping;

        public CsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
        {
            Options = options;
            _mapping = mapping;
        }

        public CsvParserOptions Options { get; }

        public IEnumerable<CsvMappingResult<TEntity>> Parse(Stream csvData)
        {
            if (csvData is null)
                throw new ArgumentNullException(nameof(csvData));

            IEnumerable<string> read()
            {
                using (var reader = new StreamReader(csvData))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }

            return Parse(read());
        }

        public IEnumerable<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData)
        {
            if (csvData is null)
                throw new ArgumentNullException(nameof(csvData));

            var query = csvData
                .Select((line, index) => (line, index))
                .Skip(Options.SkipHeader ? 1 : 0);

            if (Options.DegreeOfParallelism > 1)
            {
                var parallelQuery = query.AsParallel();

                // If you want to get the same order as in the CSV file, this option needs to be set:
                if (Options.KeepOrder)
                {
                    parallelQuery = parallelQuery.AsOrdered();
                }

                query = parallelQuery.WithDegreeOfParallelism(Options.DegreeOfParallelism);
            }

            query = query.Where(x => !string.IsNullOrWhiteSpace(x.line));

            // Ignore lines that start with comment character(s):
            if (!string.IsNullOrWhiteSpace(Options.CommentCharacter))
            {
                query = query.Where(x => !x.line.StartsWith(Options.CommentCharacter));
            }

            var tokenizer = Options.Tokenizer;

            return query.Select(x => ParseLine(x.line, x.index));
        }

        public CsvMappingEnumerable<TEntity> Parse(in SpanSplitEnumerable csvData)
        {
            return new CsvMappingEnumerable<TEntity>(Options, _mapping, in csvData);
        }

        public CsvMappingResult<TEntity> ParseLine(ReadOnlySpan<char> line, int lineNum)
        {
            var tokens = Options.Tokenizer.Tokenize(line);
            return _mapping.Map(tokens, lineNum);
        }

        public override string ToString() =>
            $"CsvParser (Options = {Options}, Mapping = {_mapping})";
    }
}
