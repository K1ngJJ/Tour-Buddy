using TourBuddy.ViewModels;

namespace TourBuddy.Views;

public partial class ForgotPasswordPage : ContentPage
{
    private readonly ForgotPasswordViewModel _viewModel;
    public ForgotPasswordPage(ForgotPasswordViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LogoImage.Opacity = 0;
        LogoImage.Scale = 0.8;

        await LogoImage.FadeTo(1, 800, Easing.CubicInOut);
        await LogoImage.ScaleTo(1, 500, Easing.CubicOut);

    }
}