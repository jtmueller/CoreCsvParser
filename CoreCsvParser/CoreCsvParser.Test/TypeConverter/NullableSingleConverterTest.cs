// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableSingleConverterTest : BaseConverterTest<float?>
    {
        protected override ITypeConverter<float?> Converter => new NullableSingleConverter();

        protected override (string?, float?)[] SuccessTestData
        {
            get
            {
                return new (string?, float?)[] {
                    (float.MinValue.ToString("R"), float.MinValue),
                    (float.MaxValue.ToString("R"), float.MaxValue),
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

        public override void AssertAreEqual(float? expected, float? actual)
        {
            if (expected == default)
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.AreEqual(expected.Value, actual, float.Epsilon);
            }
        }

        protected override string?[] FailTestData =>
            new[] { "a", double.MinValue.ToString(), double.MaxValue.ToString() };
    }

    [TestFixture]
    public class NullableSingleConverterWithFormatProviderTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<float?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableSingleConverterWithFormatProviderAndNumberStyleTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<float?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture, NumberStyles.Float | NumberStyles.AllowThousands); }
        }
    }

}
