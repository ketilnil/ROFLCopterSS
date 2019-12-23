using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ROFLCopterSS
{
    public class Missile
    {

        const string asciiMissile = @"
\\\        \\  
=|||OLOLOLOLOLO>>
///        //
";


        private readonly TextBlock          _missile;

        private readonly TranslateTransform  _translateX;
        private readonly TranslateTransform  _translateY;
        private readonly RotateTransform     _translatePitch;
        private DoubleAnimation     _animateX;
        private DoubleAnimation     _animateY;
        private readonly DoubleAnimation     _animatePitch;
        private readonly Grid                _activeGrid;



        private Timer _timer;


        public Missile(double x, double y, Duration duration, Grid grid, TranslateTransform copterTransform)
        {

            _missile = new TextBlock()
            {
                Text = asciiMissile,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                FontFamily = new FontFamily("Courier New")

            };

            _missile.Loaded += (s, a) =>
            {
                var missileTimeSpan = new TimeSpan(0,0, duration.TimeSpan.Seconds / 2);
                double width = grid.RenderSize.Width;
                //_animateX = new DoubleAnimation((width / 2) * -1, (width / 2) + _missile.ActualWidth, new Duration(missileTimeSpan));
                _animateX = new DoubleAnimation(x, (width / 2) + _missile.ActualWidth, new Duration(missileTimeSpan));

                _animateY = new DoubleAnimation(y + 100, y + 200, new Duration(new TimeSpan(0,0,2)));

                _animateX.Completed += AnimationCompletedHandler;

                _translateX.BeginAnimation(TranslateTransform.XProperty, _animateX);
                //_translateY.BeginAnimation(TranslateTransform.YProperty, _animateY);

                _timer = new Timer(Debugger, copterTransform, 0, 1000);
            };

            _translateX = new TranslateTransform(x, y + 100);
            //_translateY = new TranslateTransform(x, y + 100);

            var group = new TransformGroup();
            _missile.RenderTransform = group;

            //group.Children.Add(_translatePitch);
            group.Children.Add(_translateX);
           // group.Children.Add(_translateY);

            //_missile.RenderTransform = _translateX;

            _activeGrid = grid;
            _activeGrid.Children.Add(_missile);
        }

        private void Debugger(object state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                var x = ((TranslateTransform)state).X;
                //var x = _translateX.X;
                //var x = "Hei";
                Debug.WriteLine($"X: { x }");
            });

        }


        //public void Launch()
        //{ 

        //}


        public void Cancel()
        {
            //_translateY.BeginAnimation(TranslateTransform.YProperty, null);
            _translateX.BeginAnimation(TranslateTransform.XProperty, null);
            _activeGrid.Children.Remove(_missile);

            _timer?.Dispose();
            _timer = null;
        }



        private void AnimationCompletedHandler(object sender, EventArgs args)
        {
            this.Cancel();
            //if (_missile.Parent is Grid grid)
            //{
            //    grid.Children.Remove(_missile);
            //}
        }
    }
}
