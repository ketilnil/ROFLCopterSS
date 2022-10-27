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
using System.Linq;
using System.ComponentModel;

namespace ROFLCopterSS
{
    class ROFLCopter
    {
        private readonly Image               _copter;
        private readonly Grid                _targetGrid;
        private readonly TranslateTransform  _translateXY;
        private readonly RotateTransform     _translatePitch;
        private readonly DoubleAnimation     _animateX;
        private readonly DoubleAnimation     _animateY;
        private readonly DoubleAnimation     _animatePitch;
        private readonly Random              _random = new Random(DateTime.Now.Second);


        //private Grid                _activeGrid;

        private Missile             _missile;


        public ROFLCopter(Grid targetGrid)
        {
            _copter = new Image();
            _copter.Stretch = Stretch.None;
            _copter.Visibility = Visibility.Hidden;

            _targetGrid = targetGrid;

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

            var easingY = new SineEase
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
                EasingFunction = easingY,
                AutoReverse = true
            };

            _animateX = new DoubleAnimation();
            _animateX.Completed += OnAnimationCompleted;

            //_copter.Loaded += (s, a) =>
            //{
            //    //Debug.WriteLine("Copter loaded");
            //    Play();
            //};

            //_copter.Unloaded += (s, a) =>
            //{
            //    //Debug.WriteLine("Copter unloaded");
            //};

            //SetActiveGrid();
            _targetGrid.Children.Add(_copter);
        }


        public event Action Completed;


        public void Play()
        {
            double width = _targetGrid.RenderSize.Width;
            _animateX.From = ((width / 2) * -1) - _copter.Width;
            _animateX.To   = width / 2 + _copter.Width;

            double height = _targetGrid.RenderSize.Height;
            _animateY.From = (height / 2) / 2;
            _animateY.To = ((height / 2) / 2) * -1;
            
            SetSpeedFromSettings(_animateX, _animateY, _animatePitch);

            _translatePitch.BeginAnimation(RotateTransform.AngleProperty, _animatePitch);
            _translateXY.BeginAnimation(TranslateTransform.YProperty, _animateY);
            _translateXY.BeginAnimation(TranslateTransform.XProperty, _animateX);

            _copter.Visibility = Visibility.Visible;

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
                            _missile = new Missile(_translateXY, _animateX.Duration, _targetGrid);
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


        //private void SetActiveGrid()
        //{
        //    var index = _targetGrid.IndexOf(_activeGrid);
        //    Debug.WriteLine($"ActivegridIndex: {index}  { _activeGrid?.Tag }");

        //    // Get index of next grid (screen)
        //    index = (++index == _targetGrid.Count ? 0 : index);

        //    _activeGrid = _targetGrid[index];

        //    Debug.WriteLine($"New ActivegridIndex: {index}  {_activeGrid?.Tag}");
        //}


        private void OnAnimationCompleted(object sender, EventArgs args)
        {
            _missile?.Cancel();

            Completed?.Invoke();

            //_activeGrid.Children.Remove(_copter);

            ////var dummyGrid = new MainWindow().MainGrid;
            ////dummyGrid.Children.Add(_copter);
            ////dummyGrid.Children.Remove(_copter);

            //Debug.WriteLine("Copter removed");
            ////NameScope.GetNameScope(_activeGrid.Parent).
            // SetActiveGrid();
            //_activeGrid.Children.Add(_copter);
            
            //Debug.WriteLine("Copter added");

            //Play();
            ////Debug.WriteLine("Playing animation");

            //Debug.WriteLine($"Children: { _activeGrid.Children.Count }");
        }
    }
}
