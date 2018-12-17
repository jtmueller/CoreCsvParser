using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreCsvParser.Mapping;
using CoreCsvParser.TypeConverter;
using System.Threading.Tasks;

namespace CoreCsvParser.Benchmark
{
    public class LocalWeatherData : IEquatable<LocalWeatherData>
    {
        public int RowNum { get; set; }
        public string? WBAN { get; set; }
        public DateTime Date { get; set; }
        public string? SkyCondition { get; set; }

        public bool Equals(LocalWeatherData other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return RowNum == other.RowNum
                && WBAN == other.WBAN
                && Date == other.Date
                && SkyCondition == other.SkyCondition;
        }

        public override bool Equals(object obj)
        {
            if (obj is LocalWeatherData wd)
                return Equals(wd);
            return false;
        }

        public override int GetHashCode()
        {
            return 17 ^ RowNum.GetHashCode() ^ (WBAN?.GetHashCode() ?? 0) ^ Date.GetHashCode() ^ (SkyCondition?.GetHashCode() ?? 0);
        }

        public override string ToString()
        {
            return $"{RowNum}: {Date}, {WBAN}, {SkyCondition}";
        }
    }

    public class LocalWeatherDataMapper : CsvMapping<LocalWeatherData>
    {
        public LocalWeatherDataMapper()
        {
            MapProperty(0, x => x.WBAN);
            MapProperty(1, x => x.Date, new DateTimeConverter("yyyyMMdd"));
            MapProperty(4, x => x.SkyCondition);
        }
    }

    public class LocalWeatherDataMapperWithRowNum : CsvMapping<LocalWeatherData>
    {
        public LocalWeatherDataMapperWithRowNum()
        {
            MapProperty(0, x => x.RowNum);
            MapProperty(1, x => x.WBAN);
            MapProperty(2, x => x.Date, new DateTimeConverter("yyyyMMdd"));
            MapProperty(5, x => x.SkyCondition);
        }
    }

    [MemoryDiagnoser]
    public class CsvBenchmark
    {
        [Benchmark]
        public void LocalWeatherRead_One_Core()
        {
            var csvParserOptions = new CsvParserOptions(true, ',', 1, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var a = csvParser
                .ReadFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII)
                .ToList();
        }

        [Benchmark]
        public void LocalWeatherRead_4_Cores()
        {
            var csvParserOptions = new CsvParserOptions(true, ',', 4, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var a = csvParser
                .ReadFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII)
                .ToList();
        }

        [Benchmark]
        public async Task LocalWeatherPipeline()
        {
            var csvParserOptions = new CsvParserOptions(true, ',', 1, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var items = new List<LocalWeatherData>();
            await foreach (var (isValid, item) in csvParser.ReadFromFileAsync(@"C:\Temp\201503hourly.txt", Encoding.ASCII))
            {
                if (isValid)
                    items.Add(item);
            }
            //Console.WriteLine($"Parsed {items.Count:N0} lines.");
        }

        public static async Task CompareLoaders()
        {
            // use this in C# interactive to create subsets of the big file with a specific number of rows
            /*
            void SubsetFile(string baseFilePath, int rows)
            {
                var lines = System.IO.File.ReadLines(baseFilePath)
                    .Take(rows + 1)
                    .Select((ln, i) => i == 0 ? "RowNum," + ln : $"{i},{ln}");

                var path = System.IO.Path.GetDirectoryName(baseFilePath);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(baseFilePath) + "-first-" + rows;
                var ext = System.IO.Path.GetExtension(baseFilePath);

                var newPath = System.IO.Path.Combine(path, fileName) + ext;

                System.IO.File.WriteAllLines(newPath, lines);
            }
            */

            Console.WriteLine("Starting direct file read...");
            var csvParserOptions = new CsvParserOptions(true, ',', 1, true);
            var csvMapper = new LocalWeatherDataMapperWithRowNum();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var read = csvParser.ReadFromFile(@"C:\Temp\201503hourly-first-1000.txt", Encoding.ASCII)
                .Select(x =>
                {
                    if (x.IsValid)
                        return x.Result;
                    else
                        throw x.Error;
                });

            var readItems = new List<LocalWeatherData>(read);
            Console.WriteLine($"Read {readItems.Count:N0} lines.");
            Console.WriteLine("Starting Pipeline file read...");

            var pipedItems = new List<LocalWeatherData>(readItems.Count);

            try
            {
                await foreach (var item in csvParser.ReadFromFileAsync(@"C:\Temp\201503hourly-first-1000.txt", Encoding.ASCII))
                {
                    if (item.IsValid)
                        pipedItems.Add(item.Result);
                    else
                        Console.Error.WriteLine(item.Error);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Console.WriteLine("Total read: {0}. Last successful read: {1}", pipedItems.Count, pipedItems.Last());
                return;
            }

            if (readItems.Count > pipedItems.Count)
            {
                Console.WriteLine($"Items missing: {readItems.Count - pipedItems.Count:N0}.");
                foreach (var item in readItems.Except(pipedItems))
                {
                    Console.WriteLine(item.ToString());
                }
            }
            else
            {
                Console.WriteLine("Read items: {0:N0}.", readItems.Count);
                Console.WriteLine("Piped items: {0:N0}.", pipedItems.Count);
            }

            Console.WriteLine();
        }
    }

    public class Program
    {
        //public static async Task Main(string[] args)
        //{
        //    await CsvBenchmark.CompareLoaders();
        //}

        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CsvBenchmark>();
            //new CsvBenchmark().LocalWeatherPipeline();
        }
    }
}
