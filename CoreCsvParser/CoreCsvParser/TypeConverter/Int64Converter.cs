﻿// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class Int64Converter : NonNullableConverter<long>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public Int64Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public Int64Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Integer)
        {
        }

        public Int64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out long result)
        {
            return long.TryParse(value, numberStyles, formatProvider, out result);
        }
    }
}
