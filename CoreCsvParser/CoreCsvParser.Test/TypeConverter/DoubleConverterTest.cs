// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class DoubleConverterTest : BaseConverterTest<double>
    {
        protected override ITypeConverter<double> Converter
        {
            get { return new DoubleConverter(); }
        }

        protected override (string?, double)[] SuccessTestData
        {
            get
            {
                return new (string?, double)[] {
                    (double.MinValue.ToString("R"), double.MinValue),
                    (double.MaxValue.ToString("R"), double.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    ("5e2", 500),
                };
            }
        }

        public override void AssertAreEqual(double expected, double actual)
        {
            Assert.AreEqual(expected, actual, double.Epsilon);
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}
