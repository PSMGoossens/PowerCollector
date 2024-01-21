﻿using PowerUsageCUI.DsmrReader.Models;
using System.ComponentModel;
using System.Globalization;

namespace PowerUsageCUI.DsmrReader.Converters
{
    /// <summary>
    /// Class that converts the OBIS version to a known type. 
    /// </summary>
    public class ObisVersionConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                switch (stringValue)
                {
                    case "20":
                        return ObisVersion.V20;
                    case "42":
                        return ObisVersion.V42;
                    case "50":
                        return ObisVersion.V50;
                    default:
                        throw new NotSupportedException($"Value {stringValue} is not a recognized ObisVersion");
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
