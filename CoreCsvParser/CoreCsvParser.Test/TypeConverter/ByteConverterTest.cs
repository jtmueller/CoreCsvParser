// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class byteConverterTest : BaseConverterTest<byte>
    {
        protected override ITypeConverter<byte> Converter
        {
            get { return new ByteConverter(); }
        }

        protected override (string?, byte)[] SuccessTestData
        {
            get
            {
                return new (string?, byte)[] {
                    (byte.MinValue.ToString(), byte.MinValue),
                    (byte.MaxValue.ToString(), byte.MaxValue),
                    ("0", 0),
                    ("255", 255)
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "-1", "256" }; }
        }
    }
}
