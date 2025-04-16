using TourBuddy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

using Microsoft.Maui.Graphics;



namespace TourBuddy.Views;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;
    public RegisterPage(RegisterViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        //StartInfiniteLogoAnimation();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LogoImage.Opacity = 0;
        LogoImage.Scale = 0.8;

        await LogoImage.FadeTo(1, 800, Easing.CubicInOut);
        await LogoImage.ScaleTo(1, 500, Easing.CubicOut);

    }

    //private async void StartInfiniteLogoAnimation()
    //{
    //    while (true)
    //    {
    //        await LogoImage.ScaleTo(1.1, 1000, Easing.SinInOut);
    //        await LogoImage.ScaleTo(1.0, 1000, Easing.SinInOut);
    //    }
    //}
}
