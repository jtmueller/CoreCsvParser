// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class SByteConverterTest : BaseConverterTest<sbyte>
    {
        protected override ITypeConverter<sbyte> Converter
        {
            get { return new SByteConverter(); }
        }

        protected override (string?, sbyte)[] SuccessTestData
        {
            get
            {
                return new (string?, sbyte)[] {
                    (SByte.MinValue.ToString(), SByte.MinValue),
                    (SByte.MaxValue.ToString(), SByte.MaxValue),
                    ("0", 0),
                    ("-128", -128),
                    ("127", 127)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "-129", "128" }; }
        }
    }
}
