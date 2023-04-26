using Shiny.BluetoothLE;

namespace MauiShinyTest;

public partial class App : Application
{
	public App(IBleManager _bleManager)
	{
		InitializeComponent();

        //MainPage = new AppShell();
        MainPage = new MainPage(_bleManager);
    }
}
