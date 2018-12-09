// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableGuidConverterTest : BaseConverterTest<Guid?>
    {
        protected override ITypeConverter<Guid?> Converter =>
            new NullableGuidConverter();

        protected override (string?, Guid?)[] SuccessTestData
        {
            get
            {
                return new (string?, Guid?)[] {
                    ("02001000-0010-0000-0000-003200000000", Guid.Parse("02001000-0010-0000-0000-003200000000")),
                    (null, default),
                    (string.Empty, default),
                };
            }
        }

        protected override string?[] FailTestData =>
            new[] { "a", int.MinValue.ToString(), int.MaxValue.ToString() };
    }

    [TestFixture]
    public class NullableGuidConverterWithFormatTest : NullableGuidConverterTest
    {
        protected override ITypeConverter<Guid?> Converter =>
            new NullableGuidConverter("D");
    }
}
