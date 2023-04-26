using MauiShinyTest.Popup;
using Shiny.BluetoothLE;

namespace MauiShinyTest.Extension
{
    public class BluetoothBleDelegate : IBleDelegate
    {
        public bool StateBLE = false;

        public Task OnAdapterStateChanged(AccessState state)
        {
            _ = DialogServices.SnackBarMsg($"Bluetooth Status: {state}");

            return Task.CompletedTask;
        }


        //public Task OnConnected(IPeripheral peripheral)
        //{
        //    _ = DialogServices.SnackBarMsg("Peripheral Connected: " + peripheral.Name);

        //    return Task.CompletedTask;
        //}

        public Task OnPeripheralStateChanged(IPeripheral peripheral)
        {
            if (peripheral.Status == ConnectionState.Connected)
                _ = DialogServices.SnackBarMsg("Peripheral Connected: " + peripheral.Name);
            else
                _ = DialogServices.SnackBarMsg("Peripheral Disconnected: " + peripheral.Name);

            return Task.CompletedTask;
        }
    }
}
