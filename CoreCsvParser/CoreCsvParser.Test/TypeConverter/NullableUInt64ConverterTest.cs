// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableUInt64ConverterTest : BaseConverterTest<ulong?>
    {
        protected override ITypeConverter<ulong?> Converter
        {
            get { return new NullableUInt64Converter(); }
        }

        protected override (string?, ulong?)[] SuccessTestData
        {
            get
            {
                return new (string?, ulong?)[] {
                    (UInt64.MinValue.ToString(), UInt64.MinValue),
                    (UInt64.MaxValue.ToString(), UInt64.MaxValue),
                    ("0", 0),
                    ("1000", 1000),
                    (" ", default),
                    (null, default),
                    (string.Empty, default)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", "-1000", Int16.MinValue.ToString(), Int64.MinValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableUInt64ConverterWithFormatProviderTest : NullableUInt64ConverterTest
    {
        protected override ITypeConverter<ulong?> Converter
        {
            get { return new NullableUInt64Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableUInt64ConverterWithFormatProviderAndNumberStylesTest : NullableUInt64ConverterTest
    {
        protected override ITypeConverter<ulong?> Converter
        {
            get { return new NullableUInt64Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}
