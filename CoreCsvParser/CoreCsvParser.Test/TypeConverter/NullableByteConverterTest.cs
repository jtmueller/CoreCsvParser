// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableByteConverterTest : BaseConverterTest<byte?>
    {
        protected override ITypeConverter<byte?> Converter
        {
            get { return new NullableByteConverter(); }
        }

        protected override (string?, byte?)[] SuccessTestData
        {
            get
            {
                return new (string?, byte?)[] {
                    (byte.MinValue.ToString(), byte.MinValue),
                    (byte.MaxValue.ToString(), byte.MaxValue),
                    ("0", 0),
                    ("255", 255),
                    (" ", default),
                    (null, default),
                    (string.Empty, default)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", "-1", "256" }; }
        }
    }

    [TestFixture]
    public class NullableByteConverterWithFormatProviderTest : NullableByteConverterTest
    {
        protected override ITypeConverter<byte?> Converter
        {
            get { return new NullableByteConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableByteConverterWithFormatProviderAndNumberStylesTest : NullableByteConverterTest
    {
        protected override ITypeConverter<byte?> Converter
        {
            get { return new NullableByteConverter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }

}
