using ReactiveUI;
using System.Reactive.Linq;

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

    public static class Extensions
    {
        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext);


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext, Action<Exception> onError)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext, onError);
    }
}
