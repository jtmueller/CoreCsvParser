using CoreCsvParser.Mapping;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// TEMP FIX: (VS 2019 Preview 1)
namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks.Sources;

    internal struct ManualResetValueTaskSourceLogic<TResult>
    {
        private ManualResetValueTaskSourceCore<TResult> _core;
        public ManualResetValueTaskSourceLogic(IStrongBox<ManualResetValueTaskSourceLogic<TResult>> parent) : this() { }
        public short Version => _core.Version;
        public TResult GetResult(short token) => _core.GetResult(token);
        public ValueTaskSourceStatus GetStatus(short token) => _core.GetStatus(token);
        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags) => _core.OnCompleted(continuation, state, token, flags);
        public void Reset() => _core.Reset();
        public void SetResult(TResult result) => _core.SetResult(result);
        public void SetException(Exception error) => _core.SetException(error);
    }
}

namespace System.Runtime.CompilerServices
{
    internal interface IStrongBox<T> { ref T Value { get; } }
}
// END TEMP FIX

namespace CoreCsvParser
{
    // https://blogs.msdn.microsoft.com/dotnet/2018/07/09/system-io-pipelines-high-performance-io-in-net/

    internal class Piper
    {
        private static readonly char[] CRLF = new[] { '\r', '\n' };

        public static IAsyncEnumerable<CsvMappingResult<T>> 
            PipeStream<T>(Stream stream, Encoding encoding, CsvParser<T> parser, CancellationToken ct) 
            where T : new()
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            var pipe = new Pipe();
            var filling = FillPipeAsync(stream, pipe.Writer, ct);
            return EnumeratePipeAsync(pipe.Reader, encoding, parser, ct);
        }

        private static async ValueTask FillPipeAsync(Stream fileStream, PipeWriter writer, CancellationToken ct)
        {
            const int minimumBufferSize = 512;

            using (fileStream)
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        // Request a minimum of 512 bytes from the PipeWriter
                        var memory = writer.GetMemory(minimumBufferSize);
                        int bytesRead = await fileStream.ReadAsync(memory, ct).ConfigureAwait(false);
                        if (bytesRead == 0)
                            break;

                        // Tell the PipeWriter how much was read
                        writer.Advance(bytesRead);
                    }
                    catch
                    {
                        break;
                    }

                    // Make the data available to the PipeReader
                    var result = await writer.FlushAsync().ConfigureAwait(false);
                    if (result.IsCompleted)
                        break;
                }

                // Signal to the reader that we're done writing
                writer.Complete();
            }
        }

        public static async IAsyncEnumerable<CsvMappingResult<T>> 
            EnumeratePipeAsync<T>(PipeReader reader, Encoding encoding, CsvParser<T> parser, CancellationToken ct)
            where T : new()
        {
            int lineNum = 0;
            byte EOL = (byte)'\n';

            while (!ct.IsCancellationRequested)
            {
                var result = await reader.ReadAsync().ConfigureAwait(false);
                var buffer = result.Buffer;

                if (buffer.IsEmpty && result.IsCompleted)
                    break;

                // find the first EOL
                var position = buffer.PositionOf(EOL);

                while (position.HasValue)
                {
                    var line = buffer.Slice(0, position.Value);

                    if (lineNum > 0 || !parser.Options.SkipHeader)
                    {
                        var parsed = ProcessLine(in line, encoding, parser, lineNum);
                        if (parsed.HasValue)
                            yield return parsed.Value;
                    }
                    lineNum++;

                    // This is equivalent to position + 1
                    var next = buffer.GetPosition(1, position.Value);

                    // Skip what we've already processed including \n
                    buffer = buffer.Slice(next);

                    // Find the next EOL
                    position = buffer.PositionOf(EOL);
                }

                // We sliced the buffer until no more data could be processed
                // Tell the PipeReader how much we consumed and how much we have left to process
                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            // signal to the writer that we're done reading
            reader.Complete();
        }

        private static CsvMappingResult<T>? ProcessLine<T>(in ReadOnlySequence<byte> buffer, Encoding encoding, CsvParser<T> parser, int lineNum) where T : new()
        {
            var pool = MemoryPool<char>.Shared;
            int maxChars, len = 0;

            if (buffer.IsSingleSegment)
            {
                var bytes = buffer.First.Span;
                maxChars = encoding.GetMaxCharCount(bytes.Length);
                using (var chrMem = pool.Rent(maxChars))
                {
                    var chars = chrMem.Memory.Span;
                    len = encoding.GetChars(bytes, chars);
                    return ParseLine(parser, lineNum, chars[..len]);
                }
            }
            else
            {
                maxChars = encoding.GetMaxCharCount((int)buffer.Length);
                using (var chrMem = pool.Rent(maxChars))
                {
                    var chars = chrMem.Memory.Span;
                    foreach (var segment in buffer)
                    {
                        var bytes = segment.Span;
                        len += encoding.GetChars(bytes, chars[len..]);
                    }

                    return ParseLine(parser, lineNum, chars[..len]);
                }
            }
        }

        private static CsvMappingResult<T>? ParseLine<T>(CsvParser<T> parser, int lineNum, ReadOnlySpan<char> line) where T : new()
        {
            if (line.IsWhiteSpace())
                return null;

            ReadOnlySpan<char> commentChar = parser.Options.CommentCharacter;
            if (!commentChar.IsEmpty && line.StartsWith(commentChar))
                return null;

            return parser.ParseLine(line.TrimEnd(CRLF), lineNum);
        }
    }
}

