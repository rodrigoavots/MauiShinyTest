using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace MauiShinyTest.Popup
{
    public static class DialogServices
    {
        public static async Task SnackBarMsg(string msg, bool durationShort = false)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            string text = msg;
            ToastDuration duration = durationShort ? ToastDuration.Long : ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(text, duration, fontSize);

            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
