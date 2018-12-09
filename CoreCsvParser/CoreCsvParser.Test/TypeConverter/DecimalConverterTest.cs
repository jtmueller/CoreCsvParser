// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class DecimalConverterTest : BaseConverterTest<decimal>
    {
        protected override ITypeConverter<decimal> Converter
        {
            get { return new DecimalConverter(); }
        }

        protected override (string?, decimal)[] SuccessTestData
        {
            get
            {
                return new (string?, decimal)[] {
                    (decimal.MinValue.ToString(), decimal.MinValue),
                    (decimal.MaxValue.ToString(), decimal.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}
