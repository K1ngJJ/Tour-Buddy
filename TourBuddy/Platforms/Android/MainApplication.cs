using Android.App;
using Android.Runtime;
using System.Threading.Tasks;

namespace TourBuddy
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            // Run the asynchronous method and wait for the result
            return Task.Run(() => MauiProgram.CreateMauiApp()).Result;
        }
    }
}



//using Android.App;
//using Android.Runtime;

//namespace TourBuddy
//{
//    [Application]
//    public class MainApplication : MauiApplication
//    {
//        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
//            : base(handle, ownership)
//        {
//        }

//        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
//    }
//}
