namespace PowerUsageCUI.DataModel.Configuration
{
    public class P1Configuration 
    {
        public string Name { get; set; }
        public string IPAddress { get; set; } = "192.168.26.16";

        /// <summary>
        /// Request interval to aquire Http Data in seconds. 
        /// Default value = 5 seconds
        /// </summary>
        public uint RequestInterval { get; set; } = 1;

        /// <summary>
        /// Returns the HomeWizard URI
        /// </summary>
        public Uri Uri
        {
            get
            {
                return new Uri("http://{IPAddress}/api/v1/telegram");
            }
        }

    }
}
