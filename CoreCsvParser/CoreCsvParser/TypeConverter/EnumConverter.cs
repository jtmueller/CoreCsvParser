// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CoreCsvParser.TypeConverter
{
    public class EnumConverter<TTargetType> : NonNullableConverter<TTargetType>
        where TTargetType : struct, Enum
    {
        private readonly bool _ignoreCase;

        public EnumConverter()
            : this(true)
        {
        }

        public EnumConverter(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out TTargetType result)
        {
            // as of this writing, there is not yet an overload of Enum.TryParse that takes a span.
            return Enum.TryParse(value.ToString(), _ignoreCase, out result);
        }
    }
}
