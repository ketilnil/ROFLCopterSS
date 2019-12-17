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
        const string REG_KEY        = @"Software\LNS\ROFLCopterSS";
        const string REG_SPEED      = "Speed";
        const string REG_MISSILE    = "Missile";


        public SettingsWindow()
        {
            InitializeComponent();

            var speed = GetSpeedRegValue();
            SetRadioButton(speed);

            if (bool.TryParse(GetMissileRegValue(), out bool missile))
                Missile.IsChecked = missile;
            else
                Missile.IsChecked = false;
        }


        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Button_Click_OK(object sender, RoutedEventArgs e)
        {
            SaveSpeedRegValue(GetRadioButtonValue());
            SaveMissileRegValue((bool)Missile.IsChecked);
            this.Close();
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var val = ((CheckBox)sender).IsChecked;
            //MessageBox.Show($"IsChecked={ val }");
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var val = ((RadioButton)sender).IsChecked;
            var cont = ((RadioButton)sender).Content;
            //MessageBox.Show($"IsChecked={ val } Content={ cont }");
        }


        private string GetRadioButtonValue()
        {
            if ((bool)SpeedSlow.IsChecked) return "slow";
            if ((bool)SpeedMedium.IsChecked) return "medium";
            if ((bool)SpeedFast.IsChecked) return "fast";

            return "medium";
        }


        private void SetRadioButton(string speed)
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


        private string GetSpeedRegValue()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
            {
                if (key == null) return "medium";

                return key.GetValue(REG_SPEED, null) as string;
            }
        }


        private void SaveSpeedRegValue(string speed)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
            {
                if (key == null)
                {
                    using (var newkey = Registry.CurrentUser.CreateSubKey(REG_KEY))
                    {
                        newkey.SetValue(REG_SPEED, speed);
                    }
                }
                else
                {
                    key.SetValue(REG_SPEED, speed);
                }
            }
            //Registry.CurrentUser.SetValue(REG_SPEED, speed, RegistryValueKind.String);
        }


        private string GetMissileRegValue()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
            {
                if (key == null) return "medium";

                return key.GetValue(REG_MISSILE, null) as string;
            }
        }


        private void SaveMissileRegValue(bool missile)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
            {
                if (key == null) return;

                key.SetValue(REG_MISSILE, missile.ToString());
            }
            //Registry.CurrentUser.SetValue(REG_MISSILE, missile.ToString(), RegistryValueKind.String);
        }
    }
}
