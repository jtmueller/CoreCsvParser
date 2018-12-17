// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CoreCsvParser.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CoreCsvParser
{
    public static class CsvParserExtensions
    {
        public static IEnumerable<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this ICsvParser<TEntity> csvParser, string fileName, Encoding encoding)
            where TEntity : new()
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            var lines = File.ReadLines(fileName, encoding);
            return csvParser.Parse(lines);
        }

        public static CsvMappingEnumerable<TEntity> ReadFromString<TEntity>(this ICsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, string csvData)
            where TEntity : new()
        {
            return ReadFromSpan(csvParser, csvReaderOptions, csvData.AsSpan());
        }

        public static CsvMappingEnumerable<TEntity> ReadFromSpan<TEntity>(this ICsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, ReadOnlySpan<char> csvData)
            where TEntity : new()
        {
            var parts = csvData.Split(csvReaderOptions.NewLine, StringSplitOptions.RemoveEmptyEntries);
            return csvParser.Parse(parts);
        }

        public static IAsyncEnumerable<CsvMappingResult<TEntity>> ReadFromFileAsync<TEntity>(this ICsvParser<TEntity> csvParser, string fileName, Encoding encoding, CancellationToken? ct = null)
            where TEntity : new()
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            return csvParser.ParseAsync(File.OpenRead(fileName), encoding);
        }

        public static IAsyncEnumerable<CsvMappingResult<TEntity>> ReadFromFileAsync<TEntity>(this ICsvParser<TEntity> csvParser, FileInfo file, Encoding encoding, CancellationToken? ct = null)
            where TEntity : new()
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            return csvParser.ParseAsync(file.OpenRead(), encoding);
        }
    }
}
