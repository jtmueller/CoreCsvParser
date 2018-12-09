// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class SingleConverterTest : BaseConverterTest<float>
    {
        protected override ITypeConverter<float> Converter
        {
            get { return new SingleConverter(); }
        }

        protected override (string?, float)[] SuccessTestData
        {
            get
            {
                return new (string?, float)[] {
                    (float.MinValue.ToString("R"), float.MinValue),
                    (float.MaxValue.ToString("R"), float.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    ("5e2", 500),
                };
            }
        }

        public override void AssertAreEqual(float expected, float actual)
        {
            Assert.AreEqual(expected, actual, float.Epsilon);
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, Double.MinValue.ToString(), Double.MaxValue.ToString() }; }
        }
    }
}
