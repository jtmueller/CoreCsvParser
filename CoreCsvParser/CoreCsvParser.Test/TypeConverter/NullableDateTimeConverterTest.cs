﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDateTimeConverterTest : BaseConverterTest<DateTime?>
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(); }
        }

        protected override (string?, DateTime?)[] SuccessTestData
        {
            get
            {
                return new (string?, DateTime?)[] {
                    ("2014/01/01", new DateTime(2014, 1, 1)),
                    ("9999/12/31", new DateTime(9999, 12, 31)),
                    (" ", default),
                    (null, default),
                    (string.Empty, default)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", "10000/01/01", "1753/01/32", "0/0/0" }; }
        }
    }

    [TestFixture]
    public class NullableDateTimeConverterWithFormatTest : NullableDateTimeConverterTest
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(string.Empty); }
        }
    }

    [TestFixture]
    public class NullableDateTimeConverterWithFormatAndCultureInfoTest : NullableDateTimeConverterTest
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(string.Empty, CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableDateTimeConverterWithFormatAndCultureInfoAndDateTimeStylesTest : NullableDateTimeConverterTest
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(string.Empty, CultureInfo.InvariantCulture, DateTimeStyles.None); }
        }
    }

}
