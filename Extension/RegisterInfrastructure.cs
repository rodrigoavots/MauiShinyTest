namespace MauiShinyTest.Extension
{
    public static partial class ConfigExtensions
    {
        public static MauiAppBuilder RegisterInfrastructure(this MauiAppBuilder builder)
        {
            //Shiny - bluetooth
            //builder.Services.AddBluetoothLE();
            builder.Services.AddBluetoothLE<BluetoothBleDelegate>();

            return builder;
        }
    }
}
