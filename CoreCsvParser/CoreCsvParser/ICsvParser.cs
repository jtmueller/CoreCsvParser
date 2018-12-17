// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using CoreCsvParser.Mapping;

namespace CoreCsvParser
{
    public interface ICsvParser<TEntity>
        where TEntity : new()
    {
        IEnumerable<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData, CancellationToken? ct = null);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(IAsyncEnumerable<string> csvData, CancellationToken? ct = null);

        IEnumerable<CsvMappingResult<TEntity>> Parse(Stream csvData);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(Stream stream, Encoding encoding, CancellationToken? ct = null);

        CsvMappingEnumerable<TEntity> Parse(in SpanSplitEnumerable csvData);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(PipeReader reader, Encoding encoding, CancellationToken? ct = null);
    }

    public readonly ref struct CsvMappingEnumerable<T> where T : new()
    {
        private readonly CsvParserOptions _options;
        private readonly SpanSplitEnumerable _lines;
        private readonly CsvMapping<T> _mapping;

        public CsvMappingEnumerable(CsvParserOptions options, CsvMapping<T> mapping, in SpanSplitEnumerable lines)
        {
            _options = options;
            _mapping = mapping;
            _lines = lines;
        }

        public CsvMappingEnumerator<T> GetEnumerator() => new CsvMappingEnumerator<T>(_options, _mapping, in _lines);

        public CsvMappingResult<T>[] ToArray() => ToList().ToArray();

        public List<CsvMappingResult<T>> ToList()
        {
            var list = new List<CsvMappingResult<T>>();
            foreach (var result in this)
            {
                list.Add(result);
            }
            return list;
        }

        public List<CsvMappingResult<T>> Where(Func<CsvMappingResult<T>, bool> predicate)
        {
            var list = new List<CsvMappingResult<T>>();
            foreach (var result in this)
            {
                if (predicate(result))
                    list.Add(result);
            }
            return list;
        }
    }

    public ref struct CsvMappingEnumerator<T> where T : new()
    {
        private readonly CsvParserOptions _options;
        private SpanSplitEnumerator _lines;
        private readonly CsvMapping<T> _mapping;
        private int _curLine;

        public CsvMappingEnumerator(CsvParserOptions options, CsvMapping<T> mapping, in SpanSplitEnumerable lines)
        {
            _options = options;
            _mapping = mapping;
            _lines = lines.GetEnumerator();
            Current = default;
            _curLine = 0;
        }

        public CsvMappingResult<T> Current { get; private set; }

        public bool MoveNext()
        {
            var hasNext = _lines.MoveNext();
            if (!hasNext)
            {
                Current = default;
                return false;
            }

            if (_curLine == 0 && _options.SkipHeader)
            {
                hasNext = _lines.MoveNext();
                _curLine++;

                if (!hasNext)
                {
                    Current = default;
                    return false;
                }
            }

            while (_lines.Current.IsWhiteSpace())
            {
                hasNext = _lines.MoveNext();
                _curLine++;

                if (!hasNext)
                {
                    Current = default;
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(_options.CommentCharacter) && _lines.Current.StartsWith(_options.CommentCharacter))
            {
                hasNext = _lines.MoveNext();
                _curLine++;

                if (!hasNext)
                {
                    Current = default;
                    return false;
                }
            }

            var tokens = _options.Tokenizer.Tokenize(_lines.Current);
            Current = _mapping.Map(tokens, _curLine++);
            return true;
        }
    }
}
