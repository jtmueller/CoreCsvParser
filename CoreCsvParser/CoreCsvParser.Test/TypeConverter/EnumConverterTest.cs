﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    public enum TestEnum
    {
        A = 1
    }

    [TestFixture]
    public class EnumConverterCaseSensitiveTest : BaseConverterTest<TestEnum>
    {

        protected override ITypeConverter<TestEnum> Converter
        {
            get { return new EnumConverter<TestEnum>(false); }
        }

        protected override (string?, TestEnum)[] SuccessTestData
        {
            get
            {
                return new (string?, TestEnum)[] {
                    ("A", TestEnum.A),
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "B", string.Empty, "a", null }; }
        }
    }

    public class EnumConverterCaseInsensitiveTest : BaseConverterTest<TestEnum>
    {

        protected override ITypeConverter<TestEnum> Converter
        {
            get { return new EnumConverter<TestEnum>(true); }
        }

        protected override (string?, TestEnum)[] SuccessTestData
        {
            get
            {
                return new (string?, TestEnum)[] {
                    ("A", TestEnum.A),
                    ("a", TestEnum.A),
                };
            }
        }

        protected override string?[] FailTestData
        {
            get { return new[] { "B", " ", string.Empty, null }; }
        }
    }

    [TestFixture]
    public class EnumConverterGeneralTest
    {
        private struct NoEnum 
        {
        }

        [Test]
        public void CouldNotInstantiateNonEnumTest()
        {
            Assert.Throws<ArgumentException>(() => new EnumConverter<NoEnum>());
        }
    }
}
