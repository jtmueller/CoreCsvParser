// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CoreCsvParser.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;

namespace CoreCsvParser
{
    public interface ICsvParser<TEntity>
        where TEntity : new()
    {
        IEnumerable<CsvMappingResult<TEntity>> Parse(Stream csvData);

        IEnumerable<CsvMappingResult<TEntity>> Parse(Stream csvData, Encoding encoding);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(Stream stream, CancellationToken ct = default);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(Stream stream, Encoding encoding, CancellationToken ct = default);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(PipeReader reader, CancellationToken ct = default);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(PipeReader reader, Encoding encoding, CancellationToken ct = default);

        IEnumerable<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData, CancellationToken ct = default);

        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(IAsyncEnumerable<string> csvData, CancellationToken ct = default);

        CsvMappingEnumerable<TEntity> Parse(in SpanSplitEnumerable csvData);
    }
}
