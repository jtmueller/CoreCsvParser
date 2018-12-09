// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableInt32ConverterTest : BaseConverterTest<int?>
    {
        protected override ITypeConverter<int?> Converter =>
            new NullableInt32Converter();

        protected override (string?, int?)[] SuccessTestData
        {
            get
            {
                return new (string?, int?)[] {
                    (int.MinValue.ToString(), int.MinValue),
                    (int.MaxValue.ToString(), int.MaxValue),
                    (null, default),
                    (string.Empty, default),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string?[] FailTestData => new[] { "a" };
    }

    [TestFixture]
    public class NullableInt32ConverterWithFormatProviderTest : NullableInt32ConverterTest
    {
        protected override ITypeConverter<int?> Converter =>
            new NullableInt32Converter(CultureInfo.InvariantCulture);
    }

    [TestFixture]
    public class NullableInt32ConverterWithFormatProviderAndNumberStylesTest : NullableInt32ConverterTest
    {
        protected override ITypeConverter<int?> Converter =>
            new NullableInt32Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
    }
}
