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
    }
}
