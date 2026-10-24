using System;
using System.IO;
using System.Windows;

namespace GeminiScreenPet
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Launch transparent global particle trail overlay window
            var trailOverlay = new TrailOverlay();
            trailOverlay.Show();
        }
    }
}
