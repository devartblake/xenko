// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SiliconStudio.Core;

namespace SiliconStudio.Presentation.ValueConverters
{
    public class EnumToDisplayName : OneWayValueConverter<EnumToDisplayName>
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "(None)";

            var stringValue = value.ToString();
            var type = value.GetType();
            var memberInfo = type.GetMember(stringValue).FirstOrDefault();
            if (memberInfo == null)
                return stringValue;

            var attribute = memberInfo.GetCustomAttribute(typeof(DisplayAttribute), false) as DisplayAttribute;
            if (attribute == null)
                return stringValue;

            return attribute.Name;
        }
    }
}