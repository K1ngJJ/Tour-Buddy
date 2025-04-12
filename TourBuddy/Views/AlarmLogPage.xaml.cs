using TourBuddy.ViewModels;
using TourBuddy.Models;
using Microsoft.Maui.Controls;
using TourBuddy.Services.Alarm; // Assuming you have a service for storage

namespace TourBuddy.Views
{
    public partial class AlarmLogPage : ContentPage
    {
        private readonly IAlarmStorage _alarmStorage;

        public AlarmLogPage(IAlarmStorage alarmStorage)
        {
            InitializeComponent();
            _alarmStorage = alarmStorage;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load alarms from storage when the page appears
            await LoadAlarms();
        }

        private async Task LoadAlarms()
        {
            var alarmsFromStorage = await _alarmStorage.LoadAllAsync(); // Load alarms from your storage service
            alarmsCollectionView.ItemsSource = alarmsFromStorage; // Set the collection view's item source to the list of alarms
        }

        private void OnAlarmSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedAlarm = e.CurrentSelection.FirstOrDefault() as AlarmModel;
            if (selectedAlarm == null)
                return;

            // Example action: Show alarm details
            DisplayAlert("Alarm Selected", $"Label: {selectedAlarm.AlarmLabel}\nTime: {selectedAlarm.AlarmTime}", "OK");

            // Deselect the item
            alarmsCollectionView.SelectedItem = null;
        }

        private async void OnDeleteSwipeInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is AlarmModel alarmToDelete)
            {
                await DeleteAlarm(alarmToDelete); // Delete alarm
            }
        }

        private async Task DeleteAlarm(AlarmModel alarmToDelete)
        {
            await _alarmStorage.DeleteAsync(alarmToDelete); // Assuming a method in your storage to delete the alarm
            await LoadAlarms(); // Reload the alarms after deletion
        }
    }
}
