using TourBuddy.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TourBuddy.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;
    public LoginPage (LoginViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Clear any input when the page appears
        _viewModel.Email = string.Empty;
        _viewModel.Password = string.Empty;
    }
    private void OnFacebookSignInClicked(object sender, EventArgs e)
    {
        // Static placeholder for Facebook login
        DisplayAlert("Login", "Facebook Sign-In clicked", "OK");
        // You can later call a command or method from _viewModel
    }

    private void OnGoogleSignInClicked(object sender, EventArgs e)
    {
        // Static placeholder for Google login
        DisplayAlert("Login", "Google Sign-In clicked", "OK");
        // You can later call a command or method from _viewModel
    }

}
