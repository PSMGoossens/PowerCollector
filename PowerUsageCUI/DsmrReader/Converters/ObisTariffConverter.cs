using PowerUsageCUI.DsmrReader.Models;
using System.ComponentModel;
using System.Globalization;

namespace PowerUsageCUI.DsmrReader.Converters
{
    public class ObisTariffConverter : TypeConverter
    {
        /// <summary>
        /// Convert the tariff from the meter to a known value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                switch (stringValue)
                {
                    case "0001":
                        return PowerTariff.Low;
                    case "0002":
                        return PowerTariff.Normal;
                    default:
                        throw new NotSupportedException($"Value {stringValue} is not a recognized ObisTariff");
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
