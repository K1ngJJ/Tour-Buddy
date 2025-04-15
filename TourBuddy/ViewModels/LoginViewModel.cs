using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TourBuddy.Helpers;
using TourBuddy.Services.Auth;
using TourBuddy.Views;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using TourBuddy.Services.Google; 

namespace TourBuddy.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly GoogleAuthService _googleAuthService;
        private readonly IAuthService _authService;

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }
        public ICommand GoToForgotPasswordCommand { get; }

        public LoginViewModel(IAuthService authService, GoogleAuthService googleAuthService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            Title = "Login";

            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () => await GoToRegisterAsync());
            GoToForgotPasswordCommand = new Command(async () => await GoToForgotPasswordAsync());
        }

        public async Task LoginWithGoogleAsync()
        {
            try
            {
                // Authenticate user via Google
                var googleServiceUser = await _googleAuthService.AuthenticateAsync();

                // Map the GoogleAuthService.GoogleUser to your Models.GoogleUser
                var modelGoogleUser = new GoogleUser
                {
                    Name = googleServiceUser.Name,
                    Email = googleServiceUser.Email,
                    Picture = googleServiceUser.Picture,
                };

                // Use modelGoogleUser for login or registration
                Console.WriteLine($"User logged in: {modelGoogleUser.Name}");

                // Check if user exists in the database and register/login
                var loggedInUser = await _authService.LoginOrRegisterWithGoogleAsync(modelGoogleUser);

                // If login/registration is successful, navigate to the ProfilePage
                if (loggedInUser != null)
                {
                    Console.WriteLine("Login successful. Navigating to profile page...");
                    await Shell.Current.GoToAsync("//ProfilePage");
                }
                else
                {
                    Console.WriteLine("Failed to log in or register the user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Google login: {ex.Message}");
            }
        }

        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Validate input
                var (isValid, message) = ValidationHelper.ValidateLoginInput(Email, Password);
                if (!isValid)
                {
                    ErrorMessage = message;
                    return;
                }

                var user = await _authService.LoginAsync(Email, Password);

                if (user != null)
                {
                    // Clear sensitive data
                    Email = string.Empty;
                    Password = string.Empty;

                    // Navigate to main page/profile
                    await Shell.Current.GoToAsync("//ProfilePage");
                }
                else
                {
                    ErrorMessage = "Invalid credentials. Please check and try again.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                ErrorMessage = "An error occurred while trying to log in. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }

        private async Task GoToForgotPasswordAsync()
        {
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
