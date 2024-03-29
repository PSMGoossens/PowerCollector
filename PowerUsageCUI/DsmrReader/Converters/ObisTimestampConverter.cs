﻿using System.ComponentModel;
using System.Globalization;

namespace PowerUsageCUI.DsmrReader.Converters
{
    /// <summary>
    /// Class that convers Timestamp to a timestapt we except
    /// </summary>
    public class ObisTimestampConverter : TypeConverter
    {
        //Timestamps in format: YYMMddHHmmss[W|S]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 1); //remove 'W' or 'S'
                return DateTime.ParseExact(stringValue, "yyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
