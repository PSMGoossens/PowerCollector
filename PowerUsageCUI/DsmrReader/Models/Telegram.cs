using PowerUsageCUI.DataModel.PowerModel;
using PowerUsageCUI.DsmrReader.Converters;
using System.ComponentModel;

namespace PowerUsageCUI.DsmrReader.Models
{
    public class Telegram
    { 

        public string MessageHeader { get; set; }
        [Obis("1-3:0.2.8")]
        public ObisVersion MessageVersion { get; set; }
        [Obis("0-0:96.1.1")]
        public string SerialNumberElectricityMeter { get; set; }
        [Obis("0-1:96.1.0")]
        public string SerialNumberGasMeter { get; set; }
        [Obis("0-0:1.0.0"), TypeConverter(typeof(ObisTimestampConverter))]
        public DateTime Timestamp { get; set; }
        [Obis("1-0:1.8.1", ValueUnit = "kWh")]
        public decimal PowerConsumptionTariff1 { get; set; } = 0M;
        [Obis("1-0:1.8.2", ValueUnit = "kWh")]
        public decimal PowerConsumptionTariff2 { get; set; } = 0M;
        [Obis("1-0:2.8.1", ValueUnit = "kWh")]
        public decimal PowerproductionTariff1 { get; set; } = 0M;
        [Obis("1-0:2.8.2", ValueUnit = "kWh")]
        public decimal PowerproductionTariff2 { get; set; } = 0M;
        [Obis("0-0:96.14.0")]
        public PowerTariff CurrentTariff { get; set; }
        [Obis("1-0:32.7.0", ValueUnit = "V")]
        public decimal InstantaneousElectricityVoltagePhase1 { get; set; } = 0M;
        [Obis("1-0:52.7.0", ValueUnit = "V")]
        public decimal InstantaneousElectricityVoltagePhase2 { get; set; } = 0M;
        [Obis("1-0:72.7.0", ValueUnit = "V")]
        public decimal InstantaneousElectricityVoltagePhase3 { get; set; } = 0M;
        [Obis("1-0:21.7.0", ValueUnit = "kW")]
        public decimal InstantaneousElectricityUsagePhase1 { get; set; } = 0M;
        [Obis("1-0:41.7.0", ValueUnit = "kW")]
        public decimal InstantaneousElectricityUsagePhase2 { get; set; } = 0M;
        [Obis("1-0:61.7.0", ValueUnit = "kW")]
        public decimal InstantaneousElectricityUsagePhase3 { get; set; } = 0M;
        [Obis("1-0:22.7.0", ValueUnit = "kW")]
        public decimal InstantaneousElectricityDeliveryPhase1 { get; set; } = 0M;
        [Obis("1-0:42.7.0", ValueUnit = "kW")]
        public decimal InstantaneousElectricityDeliveryPhase2 { get; set; } = 0M;
        [Obis("1-0:62.7.0", ValueUnit = "kW")]
        public decimal InstantaneousElectricityDeliveryPhase3 { get; set; } = 0M;
        [Obis("1-0:31.7.0", ValueUnit = "A")]
        public decimal InstantaneousCurrent { get; set; } = 0M;
        [Obis("0-1:24.2.1", 1, "m3")]
        public decimal GasUsage { get; set; } = 0M;
        [Obis("0-1:24.2.1", 0), TypeConverter(typeof(ObisTimestampConverter))]
        public DateTime GasTimestamp { get; set; }
        public string CRC { get; set; }
        public IList<string> Lines { get; set; } = new List<string>();

        /// <summary>
        /// Calculate the active power of Phase 1. Negative value means delievering power
        /// </summary>
        public float ActivePowerPhase1
        {
            get
            {
                return (float)(-InstantaneousElectricityDeliveryPhase1 + InstantaneousElectricityUsagePhase1) * 1000.0f ;
            }
        }

        /// <summary>
        /// Calculate the active power of Phase 2. Negative value means delievering power
        /// </summary>
        public float ActivePowerPhase2
        {
            get
            {
                return (float)(-InstantaneousElectricityDeliveryPhase2 + InstantaneousElectricityUsagePhase2) * 1000.0f;
            }
        }

        /// <summary>
        /// Calculate the active power of Phase 3. Negative value means delievering power
        /// </summary>
        public float ActivePowerPhase3
        {
            get
            {
                return (float)(-InstantaneousElectricityDeliveryPhase3 + InstantaneousElectricityUsagePhase3) * 1000.0f;
            }
        }

        /// <summary>
        /// Calculate the active power of all the phases. Negative value means delievering power
        /// </summary>
        public float ActiveTotalPower
        {
            get
            {
                return ActivePowerPhase1 + ActivePowerPhase2 + ActivePowerPhase3;
            }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Lines) + Environment.NewLine;
        }

        public static implicit operator ThreePhaseMeter(Telegram telegram)
        {
            return new ThreePhaseMeter(telegram.Timestamp, telegram.SerialNumberElectricityMeter,
               telegram.ActivePowerPhase1, telegram.ActivePowerPhase2, telegram.ActivePowerPhase3,
               telegram.ActiveTotalPower);
        }
    }

    [TypeConverter(typeof(ObisTariffConverter))]
    public enum PowerTariff
    {
        Low,
        Normal
    }

    [TypeConverter(typeof(ObisVersionConverter))]
    public enum ObisVersion
    {
        V20,
        V42,
        V50
    }
}
