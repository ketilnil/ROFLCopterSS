﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ROFLCopterSS
{
    public class Missile
    {

        const string asciiMissile = @"
      \\        \  
´.',==|||TROLLOLOLOL>>
      //        /
";


        private readonly TextBlock              _missile;

        private readonly TranslateTransform     _translateXY;
        private readonly RotateTransform        _translatePitch;
        private readonly DoubleAnimation        _animateX;
        private readonly DoubleAnimation        _animateY;
        private readonly DoubleAnimation        _animatePitch;
        private readonly Grid                   _activeGrid;

        private Timer _timer;


        public Missile(TranslateTransform copterTransform, Duration duration, Grid grid)
        {
            _missile = new TextBlock()
            {
                Text = asciiMissile,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                FontFamily = new FontFamily("Courier New"),
                //Background = new SolidColorBrush(Color.FromRgb(40,40,40)),
                Width = 180,
                Height = 70

            };

            var easingX = new ExponentialEase
            {
                EasingMode = EasingMode.EaseInOut,
                Exponent = 3
            };

            var easingY = new SineEase
            {
                EasingMode = EasingMode.EaseInOut,
            };

            var easingPitch = new QuadraticEase
            {
                EasingMode = EasingMode.EaseOut
            };

            var missileTimeSpan = new TimeSpan(0,0, duration.TimeSpan.Seconds / 2);
            double width = grid.RenderSize.Width;
            //_animateX = new DoubleAnimation((width / 2) * -1, (width / 2) + _missile.ActualWidth, new Duration(missileTimeSpan));
            _animateX = new DoubleAnimation(copterTransform.X, width * 2, new Duration(missileTimeSpan))
            {
                EasingFunction = easingX
            };

            _animateY = new DoubleAnimation(copterTransform.Y + 100, copterTransform.Y + 200, new Duration(new TimeSpan(0, 0, 1)))
            {
                EasingFunction = easingY
            };

            _animatePitch = new DoubleAnimation(0, -5, new Duration(new TimeSpan(0, 0, 1)))
            {
                EasingFunction = easingPitch,
                AutoReverse = true
            };

            _translatePitch = new RotateTransform(0);
            _translateXY = new TranslateTransform(copterTransform.X, copterTransform.Y);

            var group = new TransformGroup();
            _missile.RenderTransform = group;

            group.Children.Add(_translatePitch);
            group.Children.Add(_translateXY);

            _missile.Loaded += (s, a) =>
            {
                _animateX.Completed += AnimationCompletedHandler;

                _translatePitch.BeginAnimation(RotateTransform.AngleProperty, _animatePitch);
                _translateXY.BeginAnimation(TranslateTransform.XProperty, _animateX);
                _translateXY.BeginAnimation(TranslateTransform.YProperty, _animateY);
#if DEBUG
                _timer = new Timer(Debugger, copterTransform, 0, 1000);
#endif
            };

            _activeGrid = grid;
            _activeGrid.Children.Add(_missile);
        }


#if DEBUG
        private void Debugger(object state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var x = ((TranslateTransform)state).X;
                //var x = _translateX.X;
                //var x = "Hei";
                //Debug.WriteLine($"X: { x }");
            });
        }
#endif


        public void Cancel()
        {
            _translateXY.BeginAnimation(TranslateTransform.YProperty, null);
            _translateXY.BeginAnimation(TranslateTransform.XProperty, null);
            _activeGrid.Children.Remove(_missile);

            _timer?.Dispose();
            _timer = null;
        }


        private void AnimationCompletedHandler(object sender, EventArgs args)
        {
            this.Cancel();
        }
    }
}
