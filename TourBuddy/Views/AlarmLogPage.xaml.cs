using TourBuddy.ViewModels;
using Microsoft.Maui.Controls;

namespace TourBuddy.Views
{
    public partial class AlarmLogPage : ContentPage
    {
        private readonly AlarmViewModel _viewModel;

        public AlarmLogPage(AlarmViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
    }
}
