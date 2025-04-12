using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Plugin.Maui.Audio;
using System;
using System.Threading.Tasks;

namespace TourBuddy.Platforms.Android.Services
{
    [Service]
    public class AlarmForegroundService : Service
    {
        private IAudioPlayer _audioPlayer;
        private string _soundFilePath;

        public override IBinder OnBind(Intent intent)
        {
            return null; // Not binding, only running in the background.
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var soundFilePath = intent.GetStringExtra("soundFilePath");
            _soundFilePath = soundFilePath;

            if (!string.IsNullOrEmpty(_soundFilePath) && System.IO.File.Exists(_soundFilePath))
            {
                // Create a new player and start playback
                _audioPlayer = AudioManager.Current.CreatePlayer(_soundFilePath);
                _audioPlayer.Play();
                StartForeground(1, CreateNotification());
            }

            return StartCommandResult.Sticky;
        }

        private Notification CreateNotification()
        {
            var notification = new NotificationCompat.Builder(this, "alarm_channel")
                .SetContentTitle("Alarm")
                .SetContentText("The alarm is ringing!")
                .SetSmallIcon(TourBuddy.Resource.Drawable.ic_clock_black_24dp)
                .SetOngoing(true)
                .Build();

            return notification;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _audioPlayer?.Stop();
            _audioPlayer?.Dispose();
        }
    }
}
