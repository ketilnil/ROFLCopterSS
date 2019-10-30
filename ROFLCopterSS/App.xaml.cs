using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;


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
                        window.Show();
                    }
                }


                var roflCopter = new ROFLCopter();
                roflCopter.AttachToWindow(MainWindow);
            }
        }
    }
}
