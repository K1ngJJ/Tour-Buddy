using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TourBuddy.Models;
using TourBuddy.Services.Alarm;
using System.Diagnostics;

namespace TourBuddy.ViewModels
{
    public partial class AlarmViewModel : ObservableObject
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _currentPlayer;

        [ObservableProperty] private int _id;
        [ObservableProperty] private TimeSpan _alarmTime;
        [ObservableProperty] private string _alarmLabel;
        [ObservableProperty] private string _selectedSound;

        public ObservableCollection<DayOfWeek> SelectedRepeatDays { get; set; } = new();
        public bool IsVibrateOn { get; set; }

        public ObservableCollection<AlarmModel> Alarms { get; } = new();
        public ObservableCollection<string> AvailableSounds { get; } = new();

        public IRelayCommand SaveAlarmCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IRelayCommand UploadSoundCommand { get; }
        public IRelayCommand PlaySoundCommand { get; }
        public ICommand StopSoundCommand { get; }


        private readonly IAlarmStorage _alarmStorage;

        public AlarmViewModel(IAlarmStorage alarmStorage, IAudioManager audioManager)
        {
            _alarmStorage = alarmStorage;
            _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));
            SaveAlarmCommand = new AsyncRelayCommand(SaveAlarm);
            CancelCommand = new RelayCommand(Cancel);
            UploadSoundCommand = new AsyncRelayCommand(UploadSound);
            PlaySoundCommand = new RelayCommand(PlaySound);
            StopSoundCommand = new Command(StopSound);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadAlarms();
                LoadDefaultSounds();
            });
        }

        private void LoadDefaultSounds()
        {
            AvailableSounds.Clear();

            // Add custom sounds saved to local storage (if any)
            var folder = FileSystem.AppDataDirectory;
            var mp3Files = Directory.GetFiles(folder, "*.mp3");
            foreach (var file in mp3Files)
            {
                AvailableSounds.Add(Path.GetFileName(file));
            }
        }

        private async Task UploadSound()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
                    {
                        { DevicePlatform.Android, new[] { "audio/mpeg", "audio/mp3" } },
                        { DevicePlatform.iOS, new[] { "audio/mp3" } },
                        { DevicePlatform.WinUI, new[] { "audio/mp3" } },
                    }),
                    PickerTitle = "Select an MP3 file"
                });

                if (result != null && result.FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    var destinationPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                    using var sourceStream = await result.OpenReadAsync();
                    using var destStream = File.Create(destinationPath);
                    await sourceStream.CopyToAsync(destStream);

                    AvailableSounds.Add(result.FileName);
                    SelectedSound = result.FileName;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Invalid File", "Please select a valid MP3 file.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to upload sound: {ex.Message}", "OK");
            }
        }
        private void PlaySound()
        {
            try
            {
                StopSound(); // stop if something is already playing

                if (!string.IsNullOrEmpty(SelectedSound))
                {
                    var soundFilePath = Path.Combine(FileSystem.AppDataDirectory, SelectedSound);

                    if (File.Exists(soundFilePath))
                    {
                        _currentPlayer = _audioManager.CreatePlayer(soundFilePath);
                        _currentPlayer.PlaybackEnded += (s, e) =>
                        {
                            // Loop the audio
                            _currentPlayer.Play();
                        };

                        _currentPlayer.Play();
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Error", "Sound file not found.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PlaySound error: {ex.Message}");
            }
        }

        private void StopSound()
        {
            if (_currentPlayer != null && _currentPlayer.IsPlaying)
            {
                _currentPlayer.Stop();
                _currentPlayer.Dispose();
                _currentPlayer = null;
            }
        }

        public void OnPageDisappearing()
        {
            StopSound();  // Stop the sound when the page disappears
        }



        public async Task LoadAlarms()
        {
            Alarms.Clear();
            var alarmsFromStorage = await _alarmStorage.LoadAllAsync(); // Load alarms from storage
            foreach (var alarm in alarmsFromStorage)
            {
                Alarms.Add(alarm); // Add them to the ObservableCollection
            }
        }


        private async Task SaveAlarm()
        {
            if (AlarmTime == TimeSpan.Zero)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please set a valid alarm time.", "OK");
                return;
            }

            var timeUntilAlarm = AlarmTime - DateTime.Now.TimeOfDay;
            if (timeUntilAlarm < TimeSpan.Zero)
                timeUntilAlarm += TimeSpan.FromDays(1);

            var notifyTime = DateTime.Now.Add(timeUntilAlarm);
            Console.WriteLine($"NotifyTime: {notifyTime}");

            var alarm = new AlarmModel
            {
                AlarmTime = AlarmTime,
                AlarmLabel = AlarmLabel,
                SelectedSound = SelectedSound,
                IsEnabled = true,
                NotificationId = new Random().Next(1000, 9999),
                NotifyTime = notifyTime
            };

            await _alarmStorage.SaveAsync(alarm);
            Alarms.Add(alarm);

            // Set the correct sound path based on the selected sound
            string soundPath = SelectedSound;
            if (!string.IsNullOrWhiteSpace(SelectedSound) && !SelectedSound.EndsWith(".mp3"))
            {
                soundPath = SelectedSound + ".mp3";
            }

            // Create the notification request with the selected sound
            var request = new NotificationRequest
            {
                NotificationId = alarm.NotificationId,
                Title = "Alarm Buddy",
                Subtitle = alarm.AlarmLabel,
                Description = "Tap me to Stop!",
                BadgeNumber = 42,
                Sound = soundPath,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime,
                    NotifyRepeatInterval = TimeSpan.FromDays(1),
                    RepeatType = NotificationRepeat.Daily,
                },
                Android =
                {
                    LaunchAppWhenTapped = true,
                    ChannelId = "alarm_channel",
                },
                ReturningData =  alarm.Id.ToString(), 
            };

       

            LocalNotificationCenter.Current.Show(request);

            // Set a timer to play the sound when the alarm goes off
            var timer = new System.Threading.Timer(async _ =>
            {
                // Check if the current time matches the alarm time
                if (DateTime.Now >= notifyTime)
                {
                    string soundFilePath = Path.Combine(FileSystem.AppDataDirectory, soundPath);
                    if (File.Exists(soundFilePath))
                    {
                        var player = _audioManager.CreatePlayer(soundFilePath); // Using the injected IAudioManager
                        player.PlaybackEnded += (s, e) =>
                        {
                            player.Play();
                        };
                        player.Play();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Sound file not found.", "OK");
                    }
                }
            }, null, timeUntilAlarm, Timeout.InfiniteTimeSpan);

            var alarmDateTime = DateTime.Today.Add(alarm.AlarmTime);
            await Application.Current.MainPage.DisplayAlert("Alarm Saved", $"Alarm set for {alarmDateTime:hh\\:mm tt}", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private void Cancel()
        {
            Shell.Current.GoToAsync("..");
        }

    }
}
