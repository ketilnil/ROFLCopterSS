using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace ROFLCopterSS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly List<ROFLCopter> _copters = new List<ROFLCopter>();
        private readonly List<Grid> _grids = new List<Grid>();
        private int _gridIndex = -1;


        static public Settings Settings = new Settings();

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (var copter in _copters)
            {
                copter.Cancel();
            }
            
            base.OnExit(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s"))
            {
                

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

                    var copter = new ROFLCopter(window.MainGrid);
                    copter.Completed += Copter_Completed;
                    _copters.Add(copter);

                    window.MainGrid.Tag = s.DeviceName;
                    
                    _grids.Add(window.MainGrid);
                    _gridIndex++;

                    window.Show();
                }

                _gridIndex = 0;
                _copters[_gridIndex].Play();


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


        private void Copter_Completed()
        {
            Debug.WriteLine("Copter_Completed");
            _copters[GetActiveGridIndex()].Play();                    
        }


        private int GetActiveGridIndex()
        {
            //var index = _grids.IndexOf(_activeGrid);
            Debug.WriteLine($"ActivegridIndex: { _gridIndex }  {_grids[_gridIndex]?.Tag}");

            // Get index of next grid (screen)
            _gridIndex = (++_gridIndex == _grids.Count ? 0 : _gridIndex);

            Debug.WriteLine($"New ActivegridIndex: {_gridIndex}  {_grids[_gridIndex]?.Tag}");
            
            return _gridIndex;
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
