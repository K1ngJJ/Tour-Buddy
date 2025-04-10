using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System;
using Microsoft.Maui.Controls;
using TourBuddy.Models;
using System.Threading.Tasks;

namespace TourBuddy.ViewModels
{
    public partial class AlarmViewModel : ObservableObject
    {
        // Properties
        [ObservableProperty]
        private TimeSpan _alarmTime;

        [ObservableProperty]
        private string _alarmLabel;

        [ObservableProperty]
        private string _selectedSound;

        [ObservableProperty]
        private double _volume;

        // Commands
        public IRelayCommand SaveAlarmCommand { get; }
        public IRelayCommand CancelCommand { get; }

        // Constructor
        public AlarmViewModel()
        {
            SaveAlarmCommand = new AsyncRelayCommand(SaveAlarm);  // AsyncRelayCommand for async methods
            CancelCommand = new RelayCommand(Cancel);  // This can remain the same if Cancel is synchronous
        }

        // Save Alarm Method
        private async Task SaveAlarm()
        {
            // Ensure AlarmTime is not zero or invalid
            if (AlarmTime == TimeSpan.Zero)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please set a valid alarm time.", "OK");
                return;
            }

            // Create the alarm object
            var alarm = new AlarmModel
            {
                AlarmTime = AlarmTime,
                AlarmLabel = AlarmLabel,
                SelectedSound = SelectedSound,
                Volume = Volume,
                IsEnabled = true,
                NotificationId = new Random().Next(1000, 9999)
            };

            // Calculate the time until the alarm should go off
            var timeUntilAlarm = alarm.AlarmTime - DateTime.Now.TimeOfDay;
            if (timeUntilAlarm < TimeSpan.Zero)
            {
                timeUntilAlarm += TimeSpan.FromDays(1);  // If the time has passed for today, set it for the next day
            }

            // Calculate NotifyTime
            var notifyTime = DateTime.Now.Add(timeUntilAlarm);
            Console.WriteLine($"NotifyTime: {notifyTime}");  // Log for debugging

            var request = new NotificationRequest
            {
                NotificationId = 1137,
                Title = "Helllo Notification",
                Subtitle = "This is subtitle",
                Description = "It's me",
                BadgeNumber = 42,
                Sound = "notif",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1),
                    NotifyRepeatInterval = TimeSpan.FromDays(1),
                    RepeatType = NotificationRepeat.Daily,
                },
            };
            LocalNotificationCenter.Current.Show(request);

            // Show a confirmation alert
            var alarmDateTime = DateTime.Today.Add(alarm.AlarmTime);
            await Application.Current.MainPage.DisplayAlert("Alarm Saved", $"Alarm set for {alarmDateTime:hh\\:mm tt}", "OK");


            // Navigate back to the previous page
            await Shell.Current.GoToAsync("..");
        }


        // Cancel Method
        private void Cancel()
        {
            // Optionally, navigate back without saving
            Shell.Current.GoToAsync("..");
        }
    }
}
