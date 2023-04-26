using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MauiShinyTest;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private readonly string[] _permissions =
{
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.BluetoothPrivileged,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.Internet,
            Manifest.Permission.WakeLock,
            Manifest.Permission.ManageExternalStorage
    };

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        CheckPermissions();

        Platform.Init(this, savedInstanceState);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    private void CheckPermissions()
    {
        bool minimumPermissionsGranted = true;

        foreach (string permission in _permissions)
        {
            if (CheckSelfPermission(permission) != Permission.Granted)
            {
                minimumPermissionsGranted = false;
                break;
            }
        }

        // If any of the minimum permissions haven't been rranted, we request them from the user
        if (!minimumPermissionsGranted) RequestPermissions(_permissions, 0);
    }
}
