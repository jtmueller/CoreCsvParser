﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableTimeSpanConverterTest : BaseConverterTest<TimeSpan?>
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(); }
        }

        protected override (string?, TimeSpan?)[] SuccessTestData
        {
            get
            {
                return new (string?, TimeSpan?)[] {
                    (TimeSpan.MinValue.ToString(), TimeSpan.MinValue),
                    ("14", TimeSpan.FromDays(14)),
                    ("1:2:3", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3))),
                    (" ", default),
                    (null, default),
                    (string.Empty, default)
                };
            }
        }

        protected override string?[] FailTestData => new[] { "a" };
    }

    [TestFixture]
    public class NullableTimeSpanConverterWithFormatProviderTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter => new NullableTimeSpanConverter(string.Empty);
    }

    [TestFixture]
    public class NullableTimeSpanConverterWithFormatAndFormatProviderTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter =>
            new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture);
    }

    [TestFixture]
    public class NullableTimeSpanConverterWithFormatAndFormatProviderAndTimeSpanStyleTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter =>
            new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture, TimeSpanStyles.None);
    }
}
