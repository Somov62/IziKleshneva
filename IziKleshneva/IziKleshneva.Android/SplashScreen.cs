using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Felipecsl.GifImageViewLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IziKleshneva.Droid
{
    [Activity(Label = "IziKleshneva", Icon = "@mipmap/icon", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class SplashScreen : AppCompatActivity
    {
        private GifImageView gifImageView;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.SplashScreen);
            gifImageView = FindViewById<GifImageView>(Resource.Id.gifImageView);

            Stream input = Assets.Open("EzKleshneva.gif");
            byte[] bytes = ConvertFileToByteArray(input);
            gifImageView.SetBytes(bytes);
            gifImageView.StartAnimation();

            await SimulateStartup();
        }

        async Task SimulateStartup()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
        private byte[] ConvertFileToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }
    }
}