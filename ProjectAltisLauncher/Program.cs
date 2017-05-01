﻿using System;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using ProjectAltisLauncher.Enums;
=======
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProjectAltisLauncher.Enums;
using ProjectAltisLauncher.Forms;
using ProjectAltisLauncher.Properties;
using Squirrel;
>>>>>>> squirrel

namespace ProjectAltisLauncher
{
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Settings.Default.filesDir = Path.Combine(appDataPath, "Project Altis");
                if(!Directory.Exists(Settings.Default.filesDir))
                    Directory.CreateDirectory(Settings.Default.filesDir);
                Directory.SetCurrentDirectory(Settings.Default.filesDir);
                Log.Initialize(LogType.Info);
                Log.Info("Loaded altis launcher v" + typeof(Program).Assembly.GetName().Version);
                Log.Info("Current time: " + DateTime.Now.ToLongDateString() + "|" + DateTime.Now.TimeOfDay);
                Log.Info("Working directory: " + Directory.GetCurrentDirectory());
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
                //It'll update in the background while it's running. The update is in effect the next time they restart app.
                MainAsync().Wait();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private static async Task MainAsync()
        {
            using (var manager = new UpdateManager("https://judge2020.com/altis"))
            {
                Log.Info("checking for update: " + manager.RootAppDirectory);
                await manager.UpdateApp();
            }
        }
    }
}