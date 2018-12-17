// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class ByteConverter : NonNullableConverter<byte>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public ByteConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public ByteConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Integer)
        {
        }

        public ByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out byte result)
        {
            return byte.TryParse(value, numberStyles, formatProvider, out result);
        }
    }
}
