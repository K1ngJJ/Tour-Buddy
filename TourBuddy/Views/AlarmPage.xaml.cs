using TourBuddy.ViewModels;
using Microsoft.Maui.Controls;

namespace TourBuddy.Views
{
    public partial class AlarmPage : ContentPage
    {
        private readonly AlarmViewModel _viewModel;

        public AlarmPage(AlarmViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Call the OnPageDisappearing method in the ViewModel to stop the sound
            (BindingContext as AlarmViewModel)?.OnPageDisappearing();
        }
    }
}
