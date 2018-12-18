// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CoreCsvParser.Mapping;
using CoreCsvParser.Tokenizer;
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
        private readonly ITokenizer _tokenizer;

        public CsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
        {
            Options = options;
            _tokenizer = options.Tokenizer;
            _mapping = mapping;
        }

        public CsvParserOptions Options { get; }

        /// <summary>
        ///     Parses from a <see cref="Stream"/> using UTF8 Encoding.
        /// </summary>
        public IEnumerable<CsvMappingResult<TEntity>> Parse(Stream csvData) => Parse(csvData, Encoding.UTF8);

        /// <summary>
        ///     Parses from a <see cref="Stream"/>.
        /// </summary>
        public IEnumerable<CsvMappingResult<TEntity>> Parse(Stream csvData, Encoding encoding)
        {
            if (csvData is null)
                throw new ArgumentNullException(nameof(csvData));
            if (encoding is null)
                throw new ArgumentNullException(nameof(encoding));

            IEnumerable<string> read()
            {
                using (var reader = new StreamReader(csvData, encoding))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }

            return Parse(read());
        }

        /// <summary>
        ///     Parses from a <see cref="Stream"/> with UTF8 encoding, asynchronously using Pipelines for low memory allocation.
        /// </summary>
        public IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(Stream stream, CancellationToken ct = default) =>
            Piper.PipeStream(stream, Encoding.UTF8, this, ct);

        /// <summary>
        ///     Parses from a <see cref="Stream"/> asynchronously using Pipelines for low memory allocation.
        /// </summary>
        public IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(Stream stream, Encoding encoding, CancellationToken ct = default) =>
            Piper.PipeStream(stream, encoding, this, ct);

        /// <summary>
        ///     Parses from a <see cref="PipeReader"/> asynchronously using UTF8 Encoding.
        /// </summary>
        public IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(PipeReader reader, CancellationToken ct = default) =>
            Piper.EnumeratePipeAsync(reader, Encoding.UTF8, this, ct);

        /// <summary>
        ///     Parses from a <see cref="PipeReader"/> asynchronously.
        /// </summary>
        public IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(PipeReader reader, Encoding encoding, CancellationToken ct = default) =>
            Piper.EnumeratePipeAsync(reader, encoding, this, ct);

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
                .Skip(Options.SkipHeader ? 1 : 0)
                .Where(x => !string.IsNullOrWhiteSpace(x.line));

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

        public CsvMappingResult<TEntity> ParseLine(ReadOnlySpan<char> line, int lineNum)
        {
            var tokens = _tokenizer.Tokenize(line);
            return _mapping.Map(in tokens, lineNum);
        }

        public override string ToString() =>
            $"CsvParser (Options = {Options}, Mapping = {_mapping})";
    }
}
