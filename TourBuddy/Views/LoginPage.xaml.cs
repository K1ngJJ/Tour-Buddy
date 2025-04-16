using TourBuddy.Services.Google;
using TourBuddy.Services.Facebook;
using TourBuddy.Helpers;
using TourBuddy.Views;
using TourBuddy.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TourBuddy.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly GoogleAuthService _authGGService;
        private readonly FacebookAuthService _authFBService;
        private readonly LoginViewModel _viewModel;

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();

            _authGGService = ServiceHelper.GetService<GoogleAuthService>();
            _authFBService = ServiceHelper.GetService<FacebookAuthService>();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Fix: mark method as async and change return type to Task
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.Email = string.Empty;
            _viewModel.Password = string.Empty;

            LogoImage.Opacity = 0;
            LogoImage.Scale = 0.8;

            await LogoImage.FadeTo(1, 800, Easing.CubicInOut);
            await LogoImage.ScaleTo(1, 500, Easing.CubicOut);

            // Optional: add a check if already logged in to avoid duplicate login
            //await _viewModel.LoginWithGoogleAsync();
        }

        private async void OnFacebookSignInClicked(object sender, EventArgs e)
        {
            try
            {
                bool confirm = await DisplayAlert("Facebook Sign-In", "Do you want to sign in with your Facebook account?", "Continue", "Cancel");
                if (!confirm)
                    return;

                var success = await _viewModel.LoginWithFacebookAsync();

                if (success)
                {
                    await DisplayAlert("Facebook Sign-In", "You have successfully signed in with Facebook.", "OK");
                }
                else
                {
                    await DisplayAlert("Facebook Sign-In", "Facebook sign-in was canceled or failed.", "OK");
                }
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
                bool confirm = await DisplayAlert("Google Sign-In", "Do you want to sign in with your Google account?", "Continue", "Cancel");
                if (!confirm)
                    return;

                // Try login and get result
                var success = await _viewModel.LoginWithGoogleAsync();

                if (success)
                {
                    await DisplayAlert("Google Sign-In", "You have successfully signed in with Google.", "OK");
                }
                else
                {
                    await DisplayAlert("Google Sign-In", "Google sign-in was canceled or failed.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
        }


        //private async void OnFacebookSignInClicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        bool confirm = await DisplayAlert("Facebook Sign-In", "Do you want to sign in with your Facebook account?", "Continue", "Cancel");
        //        if (!confirm)
        //            return;

        //        await _viewModel.LoginWithFacebookAsync();

        //        await DisplayAlert("Facebook Sign-In", "You have successfully signed in with Facebook.", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", $"Facebook login failed: {ex.Message}", "OK");
        //    }
        //}


    }
}