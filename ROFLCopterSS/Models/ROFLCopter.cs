using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace ROFLCopterSS
{
    class ROFLCopter
    {
        private readonly Image               _copter;
        private readonly List<Grid>          _targetGrids;
        private readonly TranslateTransform  _translateXY;
        private readonly RotateTransform     _translatePitch;
        private readonly DoubleAnimation     _animateX;
        private readonly DoubleAnimation     _animateY;
        private readonly DoubleAnimation     _animatePitch;
        private readonly Random              _random = new Random(DateTime.Now.Second);


        private Grid                _activeGrid;

        private Missile             _missile;


        public ROFLCopter(List<Grid> targetGrids)
        {
            _copter = new Image();
            _copter.Stretch = Stretch.None;

            _targetGrids = targetGrids;

            var gif = new BitmapImage();
            gif.BeginInit();
            gif.UriSource = new Uri("..\\images\\roflcropped.gif", UriKind.Relative);
            gif.EndInit();

            ImageBehavior.SetAnimatedSource(_copter, gif);

            _translatePitch = new RotateTransform(10);
            _translateXY = new TranslateTransform();

            var group = new TransformGroup();
            _copter.RenderTransform = group;
            _copter.Width = 300;
            _copter.Height = 300;

            group.Children.Add(_translatePitch);
            group.Children.Add(_translateXY);

            var easing = new SineEase
            {
                EasingMode = EasingMode.EaseInOut
            };

            var easingPitch = new QuadraticEase
            {
                EasingMode = EasingMode.EaseOut
            };

            _animatePitch = new DoubleAnimation(0, 10, new Duration(new TimeSpan(0, 0, 5)))
            {
                EasingFunction = easingPitch,
                AutoReverse = true
            };

            _animateY = new DoubleAnimation()
            {
                EasingFunction = easing,
                AutoReverse = true
            };

            _animateX = new DoubleAnimation();
            _animateX.Completed += AnimationCompletedHandler;

            _copter.Loaded += (s, a) =>
            {
                Play();
            };

            SetActiveGrid();
            _activeGrid.Children.Add(_copter);
        }


        public void Play()
        {
            Debug.WriteLine($"Copter height: { _copter.ActualHeight }");
            Debug.WriteLine($"Copter width: { _copter.ActualWidth }");

            double width = _activeGrid.RenderSize.Width;
            _animateX.From = ((width / 2) * -1) - _copter.Width;
            _animateX.To   = width / 2 + _copter.Width;

            //_animateY.From = _copter.Height * 2;
            //_animateY.To   = (_copter.Height * 2) * -1;
            
            // HACK: Finn ut hvordan hente riktig height verdi
            _animateY.From = 115 * 2;
            _animateY.To = (115 * 2) * -1;

            SetSpeedFromSettings(_animateX, _animateY, _animatePitch);

            _translatePitch.BeginAnimation(RotateTransform.AngleProperty, _animatePitch);
            _translateXY.BeginAnimation(TranslateTransform.YProperty, _animateY);
            _translateXY.BeginAnimation(TranslateTransform.XProperty, _animateX);

            if (App.Settings.Missile)
            {
                var willFire = (_random.Next(1,100) % 2) == 0;

                if (willFire)
                {
                    var fireDelay = _random.Next(0, ((_animateX.Duration.TimeSpan.Seconds / 3) * 2) * 1000);
                    // Randomly delayed launch
                    Task.Delay(fireDelay).ContinueWith((t) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _missile = new Missile(_translateXY, _animateX.Duration, _activeGrid);
                        });
                    });
                }
            }
        }


        public void Cancel()
        {
            _translateXY.BeginAnimation(TranslateTransform.YProperty, null);
            _translateXY.BeginAnimation(TranslateTransform.XProperty, null);

            _missile?.Cancel();
        }


        private void SetSpeedFromSettings(DoubleAnimation x, DoubleAnimation y, DoubleAnimation pitch)
        {
            int seconds;

            switch (App.Settings.Speed)
            {
                case "slow":
                    seconds = 10;
                    break;
                case "medium":
                    seconds = 8;
                    break;
                case "fast":
                    seconds = 6;
                    break;
                default:
                    seconds = 8;
                    break;
            }

            x.Duration = new Duration(new TimeSpan(0, 0, 0, seconds));
            y.Duration = new Duration(new TimeSpan(0, 0, 0, seconds / 2));
            pitch.Duration = new Duration(new TimeSpan(0, 0, 0, seconds / 2));
        }


        private void SetActiveGrid()
        {
            var index = _targetGrids.IndexOf(_activeGrid);

            // Get index of next grid (screen)
            index = (++index == _targetGrids.Count ? 0 : index);
            
            _activeGrid = _targetGrids[index];
        }


        private void AnimationCompletedHandler(object sender, EventArgs args)
        {
            _missile?.Cancel();
            _activeGrid.Children.Remove(_copter);
            SetActiveGrid();
            _activeGrid.Children.Add(_copter);
        }
    }
}
