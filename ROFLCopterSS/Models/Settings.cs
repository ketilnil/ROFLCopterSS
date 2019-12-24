using Microsoft.Win32;

namespace ROFLCopterSS
{
    public class Settings
    {
        const string REG_KEY = @"Software\LNS\ROFLCopterSS";
        const string REG_SPEED = "Speed";
        const string REG_MISSILE = "Missile";


        public string Speed 
        { 
            get
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
                {
                    if (key == null)
                        return "medium";
                    else
                        return key.GetValue(REG_SPEED, null) as string;
                }
            }

            set
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
                {
                    if (key == null)
                    {
                        using (var newkey = Registry.CurrentUser.CreateSubKey(REG_KEY))
                        {
                            newkey.SetValue(REG_SPEED, value);
                        }
                    }
                    else
                    {
                        key.SetValue(REG_SPEED, value);
                    }
                }
            }
        }


        public bool Missile 
        { 
            get
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
                {
                    if (key == null) return false;

                    var val = key.GetValue(REG_MISSILE, null) as string;
                    if (bool.TryParse(val, out bool res))
                        return res;
                    else
                        return false;
                }
            }

            set
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
                {
                    if (key == null)
                    {
                        using (var newkey = Registry.CurrentUser.CreateSubKey(REG_KEY))
                        {
                            newkey.SetValue(REG_MISSILE, value);
                        }
                    }
                    else
                    {
                        key.SetValue(REG_MISSILE, value);
                    }
                }
            }
        }

    }
}
