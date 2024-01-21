using Microsoft.EntityFrameworkCore;

namespace PowerUsageCUI.DataModel.PowerModel
{

    /// <summary>
    /// Simple data model for three phase meter
    /// </summary>
    [PrimaryKey(nameof(DateTime), nameof(DeviceName))]
    public class ThreePhaseMeter
    {
        public DateTime DateTime { get; set; }
        public string DeviceName { get; set; }

        public float ActivePowerPhase1 { get; set; }
        public float ActivePowerPhase2 { get; set; }
        public float ActivePowerPhase3 { get; set; }
        public float TotalActivePower { get; set; }

        public ThreePhaseMeter(DateTime dateTime, string deviceName, float activePowerPhase1, float activePowerPhase2, float activePowerPhase3, float totalActivePower)
        {
            DateTime = dateTime;
            DeviceName = deviceName;
            ActivePowerPhase1 = activePowerPhase1;
            ActivePowerPhase2 = activePowerPhase2;
            ActivePowerPhase3 = activePowerPhase3;
            TotalActivePower = totalActivePower;
        }
    }
}
