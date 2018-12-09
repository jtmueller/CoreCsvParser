// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class SingleConverter : NonNullableConverter<float>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public SingleConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public SingleConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands)
        {
        }

        public SingleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out float result)
        {
            return float.TryParse(value, numberStyles, formatProvider, out result)
                && float.IsFinite(result)
                && !float.IsNaN(result);
        }
    }
}