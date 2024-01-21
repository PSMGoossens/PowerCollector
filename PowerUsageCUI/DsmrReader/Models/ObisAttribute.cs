using Ardalis.GuardClauses;

namespace PowerUsageCUI.DsmrReader.Models
{
    //[Obis("1-0:1.8.1", valueIndex=1, valueUnit="kWh")]
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ObisAttribute : Attribute
    {
        public string ObisIdentifier { get; set; }
        public int ValueIndex { get; set; }
        public string? ValueUnit { get; set; }

        public ObisAttribute(string obisIdentifier, int valueIndex = 0, string? valueUnit = null)
        {
            Guard.Against.NullOrEmpty(obisIdentifier);
            Guard.Against.OutOfRange<int>(valueIndex, nameof(valueIndex), 0, int.MaxValue);

            ObisIdentifier = obisIdentifier;
            ValueIndex = valueIndex;
            ValueUnit = valueUnit;
        }
    }
}
