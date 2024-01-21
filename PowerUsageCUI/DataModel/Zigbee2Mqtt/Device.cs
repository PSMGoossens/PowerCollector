using Newtonsoft.Json;

namespace PowerUsageCUI.DataModel.Zigbee2Mqtt
{
    public class Device
    {
        public Device() { }

        public int ApplicationVersion { get; set; } = 0;

        public string DataCode { get; set; } = "";

        public string FriendlyName { get; set; } = "";

        public int HardwareVersion { get; set; } = 0;

        [JsonProperty("ieeeAddr")]
        public string IEEE_Address { get; set; } = "";

        public int ManufacturerID { get; set; } = 0;

        public string ManufacturerName { get; set; } = "";

        public string Model { get; set; } = "";

        public int NetworkAddress { get; set; } = 0;

        public string PowerSource { get; set; } = "";

        public int StackVersion { get;set; } = 0;

        public string Type { get; set; } = "";

        public int ZclVersion { get; set; } = 0;

    }
}
