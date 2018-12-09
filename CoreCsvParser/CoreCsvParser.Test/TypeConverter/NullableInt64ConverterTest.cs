// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableInt64ConverterTest : BaseConverterTest<long?>
    {
        protected override ITypeConverter<long?> Converter => new NullableInt64Converter();

        protected override (string?, long?)[] SuccessTestData
        {
            get
            {
                return new (string?, long?)[] {
                    (long.MinValue.ToString(), long.MinValue),
                    (long.MaxValue.ToString(), long.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    (" ", default),
                    (null, default),
                    (string.Empty, default)
                };
            }
        }

        protected override string?[] FailTestData => new[] { "a" };
    }

    [TestFixture]
    public class NullableInt64ConverterWithFormatProviderTest : NullableInt64ConverterTest
    {
        protected override ITypeConverter<long?> Converter =>
            new NullableInt64Converter(CultureInfo.InvariantCulture);
    }

    [TestFixture]
    public class NullableInt64ConverterWithFormatProviderAndNumberStylesTest : NullableInt64ConverterTest
    {
        protected override ITypeConverter<long?> Converter =>
            new NullableInt64Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
    }
}
