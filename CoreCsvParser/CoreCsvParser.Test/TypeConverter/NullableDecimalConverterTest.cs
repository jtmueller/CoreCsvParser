// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDecimalConverterTest : BaseConverterTest<decimal?>
    {
        protected override ITypeConverter<decimal?> Converter
        {
            get { return new NullableDecimalConverter(); }
        }

        protected override (string?, decimal?)[] SuccessTestData
        {
            get
            {
                return new (string?, decimal?)[] {
                    (decimal.MinValue.ToString(), decimal.MinValue),
                    (decimal.MaxValue.ToString(), decimal.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    (" ", default),
                    (null, default),
                    (string.Empty, default)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableDecimalConverterWithFormatProviderTest : NullableDecimalConverterTest
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableDecimalConverterWithFormatProviderAndNumberStylesTest : NullableDecimalConverterTest
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(CultureInfo.InvariantCulture, NumberStyles.Number); }
        }
    }
}
