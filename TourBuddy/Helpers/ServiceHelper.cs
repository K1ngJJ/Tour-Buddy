using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBuddy.Helpers
{
    public static class ServiceHelper
    {
        public static T GetService<T>() where T : class
            => Current.GetService<T>() ?? throw new InvalidOperationException($"Service {typeof(T)} not registered.");

        public static IServiceProvider Current =>
            Application.Current?.Handler?.MauiContext?.Services
            ?? throw new NullReferenceException("MauiContext not available.");
    }
}
