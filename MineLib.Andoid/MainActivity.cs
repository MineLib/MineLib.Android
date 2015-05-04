using System.Reflection;

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;

using Microsoft.Xna.Framework;
using MineLib.PCL.Graphics;

using PCLStorage;

namespace MineLib.Android
{
    [Activity(Label = "MineLib.Andoid", MainLauncher = true, Icon = "@drawable/icon", AlwaysRetainTaskState = true, LaunchMode = LaunchMode.SingleInstance, 
        ScreenOrientation = ScreenOrientation.SensorLandscape, ConfigurationChanges = ConfigChanges.Orientation |ConfigChanges.Keyboard |ConfigChanges.KeyboardHidden)]
    public sealed class MainActivity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var client = new Client(new NetworkTCPAndroid(), true);
            client.LoadAssembly += ClientOnLoadAssembly;
            client.GetStorage += GetStorage;
            SetContentView((View) client.Services.GetService(typeof (View)));
            client.Run();
        }

        private Assembly ClientOnLoadAssembly(object sender, byte[] assembly)
        {
            return Assembly.Load(assembly);
        }

        private IFolder GetStorage(object sender)
        {
            return new FileSystemFolder(Application.ApplicationContext.ExternalCacheDir.Path.Replace("/cache", "/files"));
        }

    }
}

