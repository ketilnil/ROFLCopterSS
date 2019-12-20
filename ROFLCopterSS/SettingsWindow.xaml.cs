using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ROFLCopterSS
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            SetRadioButtonValue(App.Settings.Speed);
            Missile.IsChecked = App.Settings.Missile;
        }


        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Button_Click_OK(object sender, RoutedEventArgs e)
        {
            App.Settings.Speed = GetRadioButtonValue();
            App.Settings.Missile = (bool)Missile.IsChecked;
            this.Close();
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //var val = ((CheckBox)sender).IsChecked;
            //MessageBox.Show($"IsChecked={ val }");
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //var val = ((RadioButton)sender).IsChecked;
            //var cont = ((RadioButton)sender).Content;
            //MessageBox.Show($"IsChecked={ val } Content={ cont }");
        }


        private string GetRadioButtonValue()
        {
            if ((bool)SpeedSlow.IsChecked) return "slow";
            if ((bool)SpeedMedium.IsChecked) return "medium";
            if ((bool)SpeedFast.IsChecked) return "fast";

            return "medium";
        }


        private void SetRadioButtonValue(string speed)
        {
            switch (speed)
            {
                case "slow":
                    {
                        SpeedSlow.IsChecked = true;
                        break;
                    }
                case "medium":
                    {
                        SpeedMedium.IsChecked = true;
                        break;
                    }
                case "fast":
                    {
                        SpeedFast.IsChecked = true;
                        break;
                    }
                default:
                    {
                        SpeedMedium.IsChecked = true;
                        break;
                    }
            }
        }
    }
}
