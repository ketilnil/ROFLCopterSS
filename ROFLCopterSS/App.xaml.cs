using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;

namespace ROFLCopterSS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ROFLCopter _copter;


        static public Settings Settings = new Settings();

        protected override void OnExit(ExitEventArgs e)
        {
            _copter?.Cancel();
            base.OnExit(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s"))
            {
                var grids = new List<Grid>();

                foreach (Screen s in Screen.AllScreens)
                {

#if DEBUG  
                    // Test on one screen, leave VS visible for debug output
                    //if (s.Primary)
                    //    continue;
#endif

                    var window = new MainWindow
                    {
                        Left = s.WorkingArea.Left,
                        Top = s.WorkingArea.Top,
                        Width = s.WorkingArea.Width,
                        Height = s.WorkingArea.Height
                    };

                    grids.Add(window.MainGrid);
                    window.Show();
                }


                _copter = new ROFLCopter(grids);
            }
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
                var wnd = new SettingsWindow();

                PlaceWindowOnParent(wnd, e.Args[0]);

                wnd.ShowDialog();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }


        private void PlaceWindowOnParent(SettingsWindow wnd, string args)
        {
            Regex regex = new Regex("\\/c[:|\\s](\\d*)", RegexOptions.IgnoreCase);
            if (regex.IsMatch(args))
            {
                var match = regex.Match(args).Groups[1].Value;

                if (Int32.TryParse(match, out int result))
                {
                    var parentHandle = new IntPtr(result);

                    ParentProcessUtilities.Rect screenSaverSettings = new ParentProcessUtilities.Rect();
                    if(ParentProcessUtilities.GetWindowRect(parentHandle, ref screenSaverSettings))
                    {
                        var parentWidth = screenSaverSettings.Right - screenSaverSettings.Left;
                        var diffWidth = wnd.Width - parentWidth;
                        var left = screenSaverSettings.Left - (diffWidth / 2);
                        wnd.Left = (left > 0 ? left : 0);

                        var parentHeight = screenSaverSettings.Bottom - screenSaverSettings.Top;
                        var diffHeight = parentHeight - wnd.Height;
                        var top = screenSaverSettings.Top + (diffHeight / 2);
                        wnd.Top = (top > 0 ? top : 0);
                    }
                    else
                    {
                        wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                }
                else
                {
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            else
            {
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }
    }
}
