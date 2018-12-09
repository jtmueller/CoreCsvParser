﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableUInt16ConverterTest : BaseConverterTest<UInt16?>
    {
        protected override ITypeConverter<ushort?> Converter
        {
            get { return new NullableUInt16Converter(); }
        }

        protected override (string?, ushort?)[] SuccessTestData
        {
            get
            {
                return new (string?, ushort?)[] {
                    (UInt16.MinValue.ToString(), UInt16.MinValue),
                    (UInt16.MaxValue.ToString(), UInt16.MaxValue),
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
            get { return new[] { "a", "-1000", Int16.MinValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableUInt16ConverterWithFormatProviderTest : NullableUInt16ConverterTest
    {
        protected override ITypeConverter<ushort?> Converter
        {
            get { return new NullableUInt16Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableUInt16ConverterWithFormatProviderAndNumberStylesTest : NullableUInt16ConverterTest
    {
        protected override ITypeConverter<ushort?> Converter
        {
            get { return new NullableUInt16Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}
