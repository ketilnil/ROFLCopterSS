using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


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


        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            Application.Current.Shutdown();
        }

        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TransformGroup group = new TransformGroup();
            double width = this.MainGrid.RenderSize.Width;
            
            DoubleAnimation animation = new DoubleAnimation((width / 2) * -1, width / 2 + Roflcopter.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 10)));
            animation.RepeatBehavior = RepeatBehavior.Forever;
            TranslateTransform tt = new TranslateTransform(-Roflcopter.ActualWidth * 2, 0);
            Roflcopter.RenderTransform = group;
            Roflcopter.Width = 300;
            Roflcopter.Height = 300;
            group.Children.Add(tt);
            tt.BeginAnimation(TranslateTransform.XProperty, animation);
            WindowState = WindowState.Maximized;
            Mouse.OverrideCursor = Cursors.None;
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
