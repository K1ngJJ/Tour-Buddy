using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using TourBuddy.Platforms.Android.Services;

namespace TourBuddy
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Enable background audio on Android for alarm notifications
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    "alarm_channel", // Channel ID
                    "Alarm Notifications", // Channel Name
                    NotificationImportance.Default // Importance Level
                );
                var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
                notificationManager?.CreateNotificationChannel(channel);
            }
        }

        public static void StartAlarmService(string soundFilePath)
        {
            var intent = new Intent(Android.App.Application.Context, typeof(AlarmForegroundService));
            intent.PutExtra("soundFilePath", soundFilePath);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StopAlarmService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(AlarmForegroundService));
            Android.App.Application.Context.StopService(intent);
        }
    }
}
