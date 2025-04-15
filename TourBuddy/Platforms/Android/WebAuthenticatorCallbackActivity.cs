using Android.App;
using Android.Content;
using Microsoft.Maui.Authentication;
using Android.Content.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBuddy.Platforms.Android
{
    [Activity(NoHistory = true, Exported = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
      DataScheme = "com.itdc.tourbuddy",
      DataPath = "/oauth2redirect")]
    internal class WebAuthenticatorCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
    {
    }
}
