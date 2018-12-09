﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class UInt32ConverterTest : BaseConverterTest<uint>
    {
        protected override ITypeConverter<uint> Converter
        {
            get { return new UInt32Converter(); }
        }

        protected override (string?, uint)[] SuccessTestData
        {
            get
            {
                return new (string?, uint)[] {
                    (UInt32.MinValue.ToString(), UInt32.MinValue),
                    (UInt32.MaxValue.ToString(), UInt32.MaxValue),
                    ("0", 0),
                    ("1000", 1000)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", "-1000", string.Empty, "  ", null, Int32.MinValue.ToString() }; }
        }
    }
}
