﻿using TourBuddy.Views;

namespace TourBuddy
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register routes
            Routing.RegisterRoute(nameof(AlarmPage), typeof(AlarmPage));
            Routing.RegisterRoute(nameof(AlarmLogPage), typeof(AlarmLogPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
        }
    }
}
