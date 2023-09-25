using Android.Content.PM;
using Android.Runtime;
using Org.Libsdl.App;
using Urho3DNet;

namespace RbfxTemplate
{
    [Activity(Label = "@string/app_name",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden,
        Theme = "@android:style/Theme.NoTitleBar.Fullscreen", HardwareAccelerated = true, MainLauncher = true)]
    public class MainActivity : UrhoActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Launcher.SdlTrapBackButton = true;
            Launcher.Run(_ => new UrhoApplication(_));

            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] global::Android.Content.PM.Permission[] grantResults)
        {
            //Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}