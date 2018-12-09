// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableInt16ConverterTest : BaseConverterTest<short?>
    {
        protected override ITypeConverter<short?> Converter
        {
            get { return new NullableInt16Converter(); }
        }

        protected override (string?, short?)[] SuccessTestData
        {
            get
            {
                return new (string?, short?)[] {
                    (Int16.MinValue.ToString(), Int16.MinValue),
                    (Int16.MaxValue.ToString(), Int16.MaxValue),
                    (null, default),
                    (string.Empty, default),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", int.MinValue.ToString(), int.MaxValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableInt16ConverterWithFormatProviderTest : NullableInt16ConverterTest
    {
        protected override ITypeConverter<short?> Converter
        {
            get { return new NullableInt16Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableInt16ConverterWithFormatProviderAndNumberStylesTest : NullableInt16ConverterTest
    {
        protected override ITypeConverter<short?> Converter
        {
            get { return new NullableInt16Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }

}
