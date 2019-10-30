using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAnimatedGif;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ROFLCopterSS
{
    class ROFLCopter
    {
        private Image _copter;


        public ROFLCopter()
        {
            _copter = new Image();
            _copter.Stretch = System.Windows.Media.Stretch.None;

            var gif = new BitmapImage();
            gif.BeginInit();
            gif.UriSource = new Uri("roflinvert.gif");
            gif.EndInit();

            ImageBehavior.SetAnimatedSource(_copter, gif);
        }

        public void AttachToWindow(Window target)
        {
            
        }


    }
}
