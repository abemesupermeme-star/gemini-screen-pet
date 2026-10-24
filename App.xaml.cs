using System;
using System.IO;
using System.Windows;

namespace GeminiScreenPet
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
                File.WriteAllText("crash.txt", ev.ExceptionObject.ToString());
            };
            DispatcherUnhandledException += (s, ev) =>
            {
                File.WriteAllText("crash_dispatcher.txt", ev.Exception.ToString());
            };
            base.OnStartup(e);
        }
    }
}
