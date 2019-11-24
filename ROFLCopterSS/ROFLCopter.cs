using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly TranslateTransform  _translateX;
        private readonly TranslateTransform  _translateY;

        private Grid                _activeGrid;


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

            var rotate = new RotateTransform(10);
            _translateX = new TranslateTransform(_copter.ActualWidth * 2, 0);
            _translateY = new TranslateTransform(0, _copter.ActualHeight * 2);


            var group = new TransformGroup();
            _copter.RenderTransform = group;
            _copter.Width = 300;
            _copter.Height = 300;

            group.Children.Add(rotate);
            group.Children.Add(_translateX);
            group.Children.Add(_translateY);

            SetActiveGrid();
            Play();
        }


        private void SetActiveGrid()
        {
            var index = _targetGrids.IndexOf(_activeGrid);
            index = (++index == _targetGrids.Count ? 0 : index);
            
            _activeGrid = _targetGrids[index];

            Debug.WriteLine($"Active Grid Index: { index }");
        }


        public void Play()
        {
            _activeGrid.Children.Add(_copter);
            double width  = _activeGrid.RenderSize.Width;
            double height = _activeGrid.RenderSize.Height;

            var animateX = new DoubleAnimation((width / 2) * -1, width / 2 + _copter.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 10)));

            //var animateY = new DoubleAnimation(height / 2 / 2, (height / 2 / 2) * -1, new Duration(new TimeSpan(0, 0, 0, 5)));
            var animateY = new DoubleAnimation(_copter.ActualHeight * 2, (_copter.ActualHeight * 2) * -1, new Duration(new TimeSpan(0, 0, 0, 5)));

            var easing = new SineEase
            {
                EasingMode = EasingMode.EaseInOut
            };

            animateY.EasingFunction = easing;
            animateY.AutoReverse = true;

            EventHandler handler = null;
            handler = (sender, args) =>
            {
                animateX.Completed -= handler;
                _activeGrid.Children.Remove(_copter);
                SetActiveGrid();
                Play();
            };

            animateX.Completed += handler;

            _translateY.BeginAnimation(TranslateTransform.YProperty, animateY);
            _translateX.BeginAnimation(TranslateTransform.XProperty, animateX);
        }
    }
}
