// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDoubleConverterTest : BaseConverterTest<double?>
    {
        protected override ITypeConverter<double?> Converter
        {
            get { return new NullableDoubleConverter(); }
        }

        protected override (string?, double?)[] SuccessTestData
        {
            get
            {
                return new (string?, double?)[] {
                    (double.MinValue.ToString("R"), double.MinValue),
                    (double.MaxValue.ToString("R"), double.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    ("5e2", 500),
                    (" ", default),
                    (null, default),
                    (string.Empty, default)
                };
            }
        }

        public override void AssertAreEqual(double? expected, double? actual)
        {
            if (expected == default)
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.AreEqual(expected.Value, actual, double.Epsilon);
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableDoubleConverterWithFormatProviderTest : NullableDoubleConverterTest
    {
        protected override ITypeConverter<double?> Converter =>
            new NullableDoubleConverter(CultureInfo.InvariantCulture); 
    }

    [TestFixture]
    public class NullableDoubleConverterWithFormatProviderAndNumberStyleTest : NullableDoubleConverterTest
    {
        protected override ITypeConverter<double?> Converter =>
            new NullableDoubleConverter(CultureInfo.InvariantCulture, NumberStyles.Float | NumberStyles.AllowThousands);
    }
}
