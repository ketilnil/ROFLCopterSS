using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ROFLCopterSS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s"))
            {
                var grids = new List<Grid>();

                foreach (Screen s in Screen.AllScreens)
                {
                    
                    if (s != Screen.PrimaryScreen)
                    {
                        var window = new Blackout
                        {
                            Left = s.WorkingArea.Left,
                            Top = s.WorkingArea.Top,
                            Width = s.WorkingArea.Width,
                            Height = s.WorkingArea.Height
                        };

                        grids.Add(window.MainGrid);
                        window.Show();
                    }
                    else
                    {
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
                }


                var roflCopter = new ROFLCopter(grids);

                //roflCopter.AttachToWindow(MainWindow);
            }
        }
    }
}
