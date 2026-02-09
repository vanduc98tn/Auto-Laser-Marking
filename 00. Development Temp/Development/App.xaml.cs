using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Development
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        public void Application_Startup(object sender, StartupEventArgs e)
        {
            //(UI Thread)
            this.DispatcherUnhandledException += (s, args) =>
            {
                args.Handled = true; 
                HandleGlobalException(args.Exception, "UI Thread Error", false);
            };

            //(Background Threads)
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                HandleGlobalException(args.ExceptionObject as Exception, "Critical Domain Error",true);
            };

            // (Async Tasks)
            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                args.SetObserved();
                HandleGlobalException(args.Exception, "Async Task Error",false);
            };

            if (!EnsureSingleInstance()) return;

            // Initialize the system
            try
            {
                SystemsManager.Instance.StartUp();
                UiManager.Instance.Startup();
            }
            catch (Exception ex)
            {
                HandleGlobalException(ex, "Startup Error",false);
            }

        }
        private void HandleGlobalException(Exception ex, string title, bool isTerminating)
        {
            string errorMessage = $"An unexpected error has occurred.:\n\n{ex?.Message}";
            MessageBox.Show(errorMessage, title, MessageBoxButton.OK, MessageBoxImage.Error);
            MyLogger logger = new MyLogger("GlobalException");
            logger.WriteLogSystem(ex, title);

            if (isTerminating)
            {
                Environment.Exit(1);
            }
        }
        private bool EnsureSingleInstance()
        {
            string appGuid = "Global\\MyUniqueAppID_Development";

            bool createdNew;
            _mutex = new Mutex(true, appGuid, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("The application has already run!", "Messenger", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Application.Current.Shutdown();
                return false;
            }
            return true;
        }

    }
}
