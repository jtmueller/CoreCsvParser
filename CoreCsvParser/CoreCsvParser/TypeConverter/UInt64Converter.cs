// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class UInt64Converter : NonNullableConverter<ulong>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public UInt64Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public UInt64Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Integer)
        {
        }

        public UInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out ulong result)
        {
            return ulong.TryParse(value, numberStyles, formatProvider, out result);
        }
    }
}