// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CoreCsvParser.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;

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

        public IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(Stream stream, Encoding encoding, CancellationToken ct = default)
        {
            return Piper.PipeStream(stream, encoding, this, ct);
        }

        /// <summary>
        ///     Parses a sequence of strings into a sequence of mapped entities.
        /// </summary>
        /// <param name="csvData">The CSV data to parse.</param>
        /// <param name="ct">When <see cref="CsvParserOptions.DegreeOfParallelism"/> is greater than one, this optional parameter allows cancellation of the parallel queries.</param>
        public IEnumerable<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData, CancellationToken ct = default)
        {
            if (csvData is null)
                throw new ArgumentNullException(nameof(csvData));

            var query = csvData
                .Select((line, index) => (line, index))
                .Skip(Options.SkipHeader ? 1 : 0);

            if (Options.DegreeOfParallelism > 1)
            {
                var parallelQuery = query.AsParallel().WithCancellation(ct);

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

            return query.Select(x => ParseLine(x.line, x.index));
        }

        public async IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(IAsyncEnumerable<string> csvData, CancellationToken ct = default)
        {
            if (csvData is null)
                throw new ArgumentNullException(nameof(csvData));

            var index = 0;
            var hasCommentChar = !string.IsNullOrWhiteSpace(Options.CommentCharacter);

            await foreach (var line in csvData)
            {
                if (ct.IsCancellationRequested)
                    break;

                if ((index == 0 && Options.SkipHeader) 
                    || string.IsNullOrWhiteSpace(line)
                    || (hasCommentChar && line.StartsWith(Options.CommentCharacter)))
                {
                    index++;
                    continue;
                }

                yield return ParseLine(line, index++);
            }
        }

        public CsvMappingEnumerable<TEntity> Parse(in SpanSplitEnumerable csvData)
        {
            return new CsvMappingEnumerable<TEntity>(Options, _mapping, in csvData);
        }

        public IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(PipeReader reader, Encoding encoding, CancellationToken ct = default)
        {
            return Piper.EnumeratePipeAsync(reader, encoding, this, ct);
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
