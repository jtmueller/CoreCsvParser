// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CoreCsvParser.TypeConverter
{
    public class BoolConverter : NonNullableConverter<bool>
    {
        private readonly string trueValue;
        private readonly string falseValue;
        private readonly StringComparison comparison;

        public BoolConverter()
            : this("true", "false", StringComparison.OrdinalIgnoreCase) { }

        public BoolConverter(string trueValue, string falseValue, StringComparison comparison)
        {
            this.trueValue = trueValue;
            this.falseValue = falseValue;
            this.comparison = comparison;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out bool result)
        {
            result = false;

            if (value.Equals(trueValue, comparison)) 
            {
                result = true;
                return true;
            }

            if (value.Equals(falseValue, comparison))
            {
                result = false;
                return true;
            }

            return false;
        }
    }
}
