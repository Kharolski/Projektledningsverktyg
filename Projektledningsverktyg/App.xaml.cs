using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Helpers;
using Projektledningsverktyg.Views.Auth;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Projektledningsverktyg
{
    public partial class App : Application
    {
        public static Member CurrentUser { get; set; }

        private MainWindow _mainWindow;

        public App()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjektledningsDB.mdf");
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetDirectoryName(dbPath));

            using (var db = new ApplicationDbContext())
            {
                db.Database.CreateIfNotExists();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set Swedish culture for the application
            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv-SE");
            Services.ProtocolHandler.RegisterProtocol();

            if (_mainWindow == null)
            {
                _mainWindow = new MainWindow();
                Current.MainWindow = _mainWindow;
                _mainWindow.Show();
            }

            if (e.Args.Length > 0)
            {
                string uri = e.Args[0];
                if (uri.StartsWith("projektverktyg://reset"))
                {
                    var token = uri.Split('=').LastOrDefault();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _mainWindow.SwitchToView("ResetPassword");
                        if (_mainWindow.FindName("ResetPasswordScreen") is ResetPasswordView resetView)
                        {
                            resetView.SetResetToken(token);
                        }
                    }
                }
            }
        }


    }
}
