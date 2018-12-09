// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class StringConverterTest : BaseConverterTest<string>
    {
        protected override ITypeConverter<string> Converter
        {
            get { return new StringConverter(); }
        }

        protected override (string?, string)[] SuccessTestData
        {
            get
            {
                return new (string?, string)[] {
                    (string.Empty, string.Empty),
                    (" ", " "),
                    ("Abc", "Abc"),
                    (null, string.Empty)
                };
            }
        }

        protected override string?[] FailTestData =>
            new string[] {  }; // Should never fail, because values are passed through.
    }
}
