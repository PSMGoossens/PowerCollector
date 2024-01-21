using Newtonsoft.Json;
using System.ComponentModel;

namespace PowerUsageCUI.DataModel.Zigbee2Mqtt
{
    public class ThreePhaseMeter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonProperty("current_x")]
        public float CurrentX { get; set; } = 0.0f;

        [JsonProperty("current_y")]
        public float CurrentY { get; set; } = 0.0f;

        [JsonProperty("current_z")]
        public float CurrentZ { get; set; } = 0.0f;

        public Device Device { get; set; } = null;

        public float Energy { get; set; } = 0.0f;

        [JsonProperty("last_seen")]
        public DateTime? LastSeen { get; set; } = null;

        public int LinkQuality { get; set; } = 0;

        [JsonProperty("power_x")]
        public float PowerX { get; set; } = 0.0f;

        [JsonProperty("power_y")]
        public float PowerY { get; set; } = 0.0f;

        [JsonProperty("power_z")]
        public float PowerZ { get; set; } = 0.0f;

        public float TotalPower
        {
            get
            {
                return PowerX+PowerY+PowerZ;
            }
        }

        public float ProducedEnergy { get; set; } = 0.0f;

        [JsonProperty("voltage_x")]
        public float VoltageX { get; set; } = 0.0f;

        [JsonProperty("voltage_y")]
        public float VoltageY { get; set; } = 0.0f;

        [JsonProperty("voltage_z")]
        public float VoltageZ { get; set; } = 0.0f;

    }
}
