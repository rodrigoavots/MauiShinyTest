using MauiShinyTest.Extension;
using MauiShinyTest.Popup;
using Shiny.BluetoothLE;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;

namespace MauiShinyTest;

public partial class MainPage : ContentPage
{

    public MainPage(IBleManager _bleManager)
    {
        InitializeComponent();
        BindingContext = this;
        bleManager = _bleManager;
    }



    public string SensorConnectionStatus { get; set; } = "Disconnected";
    public string SensorScanStatus { get; set; } = "Not found";
    public string SensorCharacteristicStatus { get; set; } = "Disconnected";

    readonly IBleManager bleManager;
    public IPeripheral peripheral;
    public IDisposable scanner, chsResult, wscObserver;
    public Guid uuid;
    public ScanResult scanResult;
    public BleCharacteristicInfo sensorCharacteristic;

    public List<byte> messageBytes;

    private bool hasDeviceStatusSubscription = false;


    async void OnScanClicked(object s, EventArgs e)
    {
        var access = await bleManager.RequestAccess();
        Console.WriteLine("Shiny Access: " + access);

        Stopwatch watch = Stopwatch.StartNew();

        scanner = bleManager.Scan().Subscribe(scanResult =>
        {
            SensorScanStatus = "Scanning...";
            OnPropertyChanged(nameof(SensorScanStatus));

            Debug.WriteLine("Found a device with name " + scanResult.Peripheral.Name);

            var adv = scanResult.AdvertisementData;

            if (adv.LocalName != null && adv.LocalName.Contains("BluVib"))
            {
                watch.Stop();
                SensorScanStatus = "Device found: " + scanResult.Peripheral.Name.ToString();
                OnPropertyChanged(nameof(SensorScanStatus));
                var elapsed = watch.ElapsedMilliseconds;
                Debug.WriteLine("Stopping scan after " + elapsed + " mS");
                peripheral = scanResult.Peripheral;
                scanner?.Dispose();
            }
        });
    }

    async void OnConnectClicked(object s, EventArgs e)
    {
        if (!hasDeviceStatusSubscription)
        {
            try
            {
                //
                await peripheral.WithConnectIf().Timeout(TimeSpan.FromSeconds(30)).ToTask();

                //*** Test All Characteristics
                var teste = await peripheral.GetAllCharacteristics().Timeout(TimeSpan.FromSeconds(20));

                wscObserver = peripheral.WhenStatusChanged().Subscribe(connectionState =>
                {
                    hasDeviceStatusSubscription = true;
                    switch (connectionState)
                    {
                        case Shiny.BluetoothLE.ConnectionState.Connected:
                            Debug.WriteLine("Device connected OK");
                            SensorConnectionStatus = "Connected to " + peripheral.Name.ToString();
                            OnPropertyChanged(nameof(SensorConnectionStatus));
                            break;

                        case Shiny.BluetoothLE.ConnectionState.Disconnected:
                            Debug.WriteLine("Device disconnected");
                            SensorConnectionStatus = "Disconnected";
                            OnPropertyChanged(nameof(SensorConnectionStatus));
                            break;

                        case Shiny.BluetoothLE.ConnectionState.Connecting:
                            Debug.WriteLine("Device in connecting state");
                            SensorConnectionStatus = "Connecting...";
                            OnPropertyChanged(nameof(SensorConnectionStatus));
                            break;


                        default:
                            break;

                    }

                });

                //*** Get Characteristc - ChBattery
                //sensorCharacteristic = await peripheral.GetCharacteristic(AppSettings.GetServiceUuid, AppSettings.ChBattery).Take(1)
                //      .Timeout(TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                await DialogServices.SnackBarMsg(ex.Message);
            }
        }

        Debug.WriteLine("Connecting...");

        if (peripheral.Status == ConnectionState.Disconnected) // .IsDisconnected())
            await peripheral.ConnectAsync(new ConnectionConfig());
    }

    async void OnStartNotify(object s, EventArgs e)
    {
        sensorCharacteristic = null;

        string seriveUUID = AppSettings.ServiceUUID;
        string charUUID = AppSettings.ChNotifyVib;

        chsResult = peripheral
            .NotifyCharacteristic(seriveUUID, charUUID)
            .Timeout(TimeSpan.FromSeconds(30))
            .SubOnMainThread(
                x =>
                {
                    //read
                    this.SetReadValue(x, false);
                }
                , ex =>
                {
                    _ = DialogServices.SnackBarMsg("Erro DoNotifyAsync!\n" + ex.Message);
                }
        );


        if (sensorCharacteristic != null)
        {
            Debug.WriteLine("Characteristic discovered: " + sensorCharacteristic.Uuid.ToString());
            SensorCharacteristicStatus = "Listening...";
            OnPropertyChanged(nameof(SensorCharacteristicStatus));
        }
    }
    void SetReadValue(BleCharacteristicResult result, bool fromUtf8) => Application.Current.Dispatcher.Dispatch(() => //Device.BeginInvokeOnMainThread(() =>
    {
        var testeResult = fromUtf8
                ? Encoding.UTF8.GetString(result.Data, 0, result.Data.Length)
                : BitConverter.ToString(result.Data).Replace("-", " ");
    });


    async void OnDisconnect(object s, EventArgs e)
    {

        if (sensorCharacteristic != null)
        {
            if (sensorCharacteristic.IsNotifying)
            {
                SensorCharacteristicStatus = "Not listening";
                OnPropertyChanged(nameof(SensorCharacteristicStatus));

                chsResult?.Dispose();
                wscObserver?.Dispose();
                sensorCharacteristic = null;
                hasDeviceStatusSubscription = false;
            }
        }

        if (peripheral.Status == ConnectionState.Disconnected) //.IsConnected())
        {
            peripheral.CancelConnection();
            SensorConnectionStatus = "Disconnected";
            OnPropertyChanged(nameof(SensorConnectionStatus));

        }

    }

}


