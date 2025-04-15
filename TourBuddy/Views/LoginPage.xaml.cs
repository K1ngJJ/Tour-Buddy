using TourBuddy.Services.Google;
using TourBuddy.Helpers;
using TourBuddy.Views;
using TourBuddy.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TourBuddy.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly GoogleAuthService _authService;
        private readonly LoginViewModel _viewModel;

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();

            _authService = ServiceHelper.GetService<GoogleAuthService>();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Fix: mark method as async and change return type to Task
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.Email = string.Empty;
            _viewModel.Password = string.Empty;

            // Optional: add a check if already logged in to avoid duplicate login
            //await _viewModel.LoginWithGoogleAsync();
        }

        private async void OnFacebookSignInClicked(object sender, EventArgs e)
        {
            try
            {
                await DisplayAlert("Login", "Facebook Sign-In clicked", "OK");
                await _viewModel.LoginWithGoogleAsync(); // Let ViewModel handle Google Auth + DB Login + Navigation
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }

        }

        private async void OnGoogleSignInClicked(object sender, EventArgs e)
        {
            try
            {
                var user = await _authService.AuthenticateAsync();

                if (user != null)
                {
                    await DisplayAlert("Success", $"Logged in as {user.Name}", "OK");
                    await Shell.Current.GoToAsync("//ProfilePage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to authenticate with Google", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
        }
    }
}