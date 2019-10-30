using System;
using System.Windows;
using System.Windows.Input;


namespace ROFLCopterSS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            Mouse.OverrideCursor = Cursors.None;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }


        Point OriginalLocation = new Point(int.MaxValue, int.MaxValue);
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //** take this if statement out if your not doing a preview
            //if (!IsPreviewMode) //disable exit functions for preview
            //{
            var currentPos = e.GetPosition(this);
            //see if originallocation has been set
            if (OriginalLocation.X == int.MaxValue &
                OriginalLocation.Y == int.MaxValue)
            {
                OriginalLocation = currentPos; //.Location;
            }
            //see if the mouse has moved more than 20 pixels 
            //in any direction. If it has, close the application.
            if (Math.Abs(currentPos.X - OriginalLocation.X) > 20 |
                Math.Abs(currentPos.Y - OriginalLocation.Y) > 20)
            {
                Application.Current.Shutdown();
            }

            //}
        }
    }
}
