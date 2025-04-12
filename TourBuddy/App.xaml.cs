using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using Plugin.Maui.Audio;
using TourBuddy.Services.Database;

namespace TourBuddy
{
    public partial class App : Application
    {
        private readonly ISQLiteService _sqliteService;
        private readonly SyncService _syncService;

        private IAudioPlayer? player;
        public App(ISQLiteService sqliteService, SyncService syncService)
        {
            InitializeComponent();
            _sqliteService = sqliteService;
            _syncService = syncService;

            MainPage = new AppShell();
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Initialize local database
            await _sqliteService.InitializeDatabaseAsync();

            // Start auto sync in background if enabled
            await _syncService.StartAutoSyncIfEnabledAsync();
        }

        private async void OnNotificationTapped(NotificationEventArgs e)
        {
            try
            {
                // Stop the sound
                player?.Stop();

                // Get the alarm ID passed from notification
                if (int.TryParse(e.Request.ReturningData, out int alarmId))
                {
                    // Optional: store it or pass as query to the details page
                    await Shell.Current.GoToAsync($"AlarmPage?alarmId={alarmId}");
                }
                else
                {
                    await Shell.Current.GoToAsync("AlarmPage");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

    }
}