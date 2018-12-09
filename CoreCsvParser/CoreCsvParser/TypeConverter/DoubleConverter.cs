// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class DoubleConverter : NonNullableConverter<double>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public DoubleConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public DoubleConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands)
        {
        }

        public DoubleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out double result)
        {
            return double.TryParse(value, numberStyles, formatProvider, out result)
                && double.IsFinite(result)
                && !double.IsNaN(result);
        }
    }
}
