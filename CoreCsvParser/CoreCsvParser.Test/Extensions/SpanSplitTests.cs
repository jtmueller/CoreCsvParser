using NUnit.Framework;
using System;
using System.Linq;

namespace CoreCsvParser.Test.Extensions
{
    [TestFixture]
    public class SpanSplitTests
    {
        [Test]
        public void Basic_Split_Array_Test()
        {
            var input = "1,2,3,4,5";
            var expected = input.Split(',');
            var results = input.AsSpan().Split(',').ToArray();

            Assert.AreEqual(expected.Length, results.Length, "Wrong number of results!");

            foreach (var (exp, act) in expected.Zip(results))
            {
                Assert.AreEqual(exp, act);
            }
        }

        [Test]
        public void Basic_Split_Test()
        {
            var input = "1,2,3,4,5";
            var expected = input.Split(',');
            var results = input.AsSpan().Split(',').ToArray();

            Assert.AreEqual(expected.Length, results.Length, "Wrong number of results!");

            foreach (var (exp, act) in expected.Zip(results))
            {
                Assert.AreEqual(exp, act);
            }
        }

        [Test]
        public void Basic_SplitAny_Test()
        {
            var input = "1,2;3,4,;5,6|7|8";
            var expected = input.Split(',', ';', '|');
            var results = input.AsSpan().Split(',', ';', '|').ToArray();

            Assert.AreEqual(expected.Length, results.Length, "Wrong number of results!");

            foreach (var (exp, act) in expected.Zip(results))
            {
                Assert.AreEqual(exp, act);
            }
        }

        [Test]
        public void Split_Remove_Empty_Test()
        {
            var input = ",1,2,,3,,,4,5,";
            var expected = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var results = input.AsSpan().Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();

            Assert.AreEqual(expected.Length, results.Length, "Wrong number of results!");

            foreach (var (exp, act) in expected.Zip(results))
            {
                Assert.AreEqual(exp, act);
            }
        }

        [Test]
        public void SplitAny_Remove_Empty_Test()
        {
            var input = ";;1,,2;,;|,3;;;4,;5,6|,|7|8||";
            var splitChars = new[] { ',', ';', '|' };
            var expected = input.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            var results = input.AsSpan().Split(splitChars, StringSplitOptions.RemoveEmptyEntries).ToArray();

            Assert.AreEqual(expected.Length, results.Length, "Wrong number of results!");

            foreach (var (exp, act) in expected.Zip(results))
            {
                Assert.AreEqual(exp, act);
            }
        }

        [Test]
        public void Split_By_String_Test()
        {
            var input = string.Join(Environment.NewLine, new[] { "foo", "bar", "baz", "quux" });
            var expected = input.Split(Environment.NewLine);
            var results = input.AsSpan().Split(Environment.NewLine).ToArray();

            Assert.AreEqual(expected.Length, results.Length, "Wrong number of results!");

            foreach (var (exp, act) in expected.Zip(results))
            {
                Assert.AreEqual(exp, act);
            }
        }
    }
}
