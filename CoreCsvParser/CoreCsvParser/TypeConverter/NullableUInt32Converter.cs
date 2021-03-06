﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class NullableUInt32Converter : NullableInnerConverter<UInt32>
    {
        public NullableUInt32Converter()
            : base(new UInt32Converter())
        {
        }

        public NullableUInt32Converter(IFormatProvider formatProvider)
            : base(new UInt32Converter(formatProvider))
        {
        }

        public NullableUInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new UInt32Converter(formatProvider, numberStyles))
        {
        }
    }
}
