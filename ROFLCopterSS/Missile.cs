using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ROFLCopterSS
{
    public class Missile
    {

        private readonly TextBlock          _missile;

        private readonly TranslateTransform _translateX;
        private readonly DoubleAnimation    _animateX;



        public Missile()
        {
            _missile = new TextBlock()
            {
                Text = @"
  \\\\             \\  
  =||||TROLLOLOLOLOLOLOLO>>
  ////             //
"
            };

        }


        public void Launch()
        { 

        }


        public void Cancel()
        {

        }
    }
}
