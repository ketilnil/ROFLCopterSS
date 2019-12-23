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


        private Grid                _activeGrid;

        private Missile             _missile;

        private readonly Random _random = new Random(DateTime.Now.Second);
        


        public ROFLCopter(List<Grid> targetGrids)
        {
            _copter = new Image();
            _copter.Stretch = Stretch.None;

            _targetGrids = targetGrids;

            var gif = new BitmapImage();
            gif.BeginInit();
            gif.UriSource = new Uri("roflcropped.gif", UriKind.Relative);
            gif.EndInit();

            ImageBehavior.SetAnimatedSource(_copter, gif);

            //var rotate = new RotateTransform(10);
            _translatePitch = new RotateTransform(10);
            _translateXY = new TranslateTransform(_copter.ActualWidth * 2, _copter.ActualHeight * 2);

            var group = new TransformGroup();
            _copter.RenderTransform = group;
            _copter.Width = 300;
            _copter.Height = 300;

            group.Children.Add(_translatePitch);
            group.Children.Add(_translateXY);

            SetActiveGrid();



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


            //double height = _activeGrid.RenderSize.Height;
            Debug.WriteLine($"Copter height: { _copter.ActualHeight }");
            Debug.WriteLine($"Copter width: { _copter.ActualWidth }");
            _animateY = new DoubleAnimation(_copter.ActualHeight * 2, (_copter.ActualHeight * 2) * -1, new Duration(new TimeSpan(0, 0, 0, 5)))
            {
                EasingFunction = easing,
                AutoReverse = true
            };

            double width = _activeGrid.RenderSize.Width;
            _animateX = new DoubleAnimation((width / 2) * -1, width / 2 + _copter.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 10)));

            _animateX.Completed += AnimationCompletedHandler;


            //TODO: _copter.Loaded += (s, a) =>
            Play();
        }


        public void Play()
        {
            _activeGrid.Children.Add(_copter);

            Debug.WriteLine($"Copter height: { _copter.ActualHeight }");
            Debug.WriteLine($"Copter width: { _copter.ActualWidth }");

            double width = _activeGrid.RenderSize.Width;
            _animateX.From = (width / 2) * -1;
            _animateX.To   = width / 2 + _copter.ActualWidth;

            //_animateY.From = _copter.ActualHeight * 2;
            //_animateY.To   = (_copter.ActualHeight * 2) * -1;
            
            // HACK: Finn ut hvordan hente riktig height verdi
            _animateY.From = 115 * 2;
            _animateY.To = (115 * 2) * -1;

            SetSpeedFromSettings(_animateX, _animateY, _animatePitch);

            _translatePitch.BeginAnimation(RotateTransform.AngleProperty, _animatePitch);
            _translateXY.BeginAnimation(TranslateTransform.YProperty, _animateY);
            _translateXY.BeginAnimation(TranslateTransform.XProperty, _animateX);

            if (App.Settings.Missile)
            {
                var fire = (_random.Next(1,100) % 2) == 0;

                if (fire)
                {
                    var delay = _random.Next(0, (_animateX.Duration.TimeSpan.Seconds) * 1000);
                    // Randomly delayed launch
                    Task.Delay(delay).ContinueWith((t) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var point = _copter.TransformToAncestor((Window)_activeGrid.Parent).Transform(new Point(0,0));
                            Debug.WriteLine($"Copter X: { point.X }  Y: { point.Y }");

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
            index = (++index == _targetGrids.Count ? 0 : index);
            
            _activeGrid = _targetGrids[index];

            Debug.WriteLine($"Active Grid Index: { index }");
        }


        private void AnimationCompletedHandler(object sender, EventArgs args)
        {
            _missile?.Cancel();
            _activeGrid.Children.Remove(_copter);
            SetActiveGrid();
            Play();
        }
    }
}
