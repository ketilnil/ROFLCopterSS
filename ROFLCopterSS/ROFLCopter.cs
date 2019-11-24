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
        private readonly Image          _copter;
        private readonly List<Grid>     _targetGrids;

        private Grid                _activeGrid;
        private TranslateTransform  _translateX;


        public ROFLCopter(List<Grid> targetGrids)
        {
            _copter = new Image();
            _copter.Stretch = Stretch.None;

            _targetGrids = targetGrids;

            var gif = new BitmapImage();
            gif.BeginInit();
            gif.UriSource = new Uri("roflinvert.gif", UriKind.Relative);
            gif.EndInit();

            ImageBehavior.SetAnimatedSource(_copter, gif);

            var rotate = new RotateTransform(10);
            _translateX = new TranslateTransform(_copter.ActualWidth * 2, 0);

            var group = new TransformGroup();
            _copter.RenderTransform = group;
            _copter.Width = 300;
            _copter.Height = 300;

            group.Children.Add(rotate);
            group.Children.Add(_translateX);

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
            double width = _activeGrid.RenderSize.Width;

            var animation = new DoubleAnimation((width / 2) * -1, width / 2 + _copter.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 10)));
            //animation.RepeatBehavior = RepeatBehavior.Forever;

            //animation.Completed += Animation_Completed;

            EventHandler handler = null;
            handler = (sender, args) =>
            {
                animation.Completed -= handler;
                _activeGrid.Children.Remove(_copter);
                SetActiveGrid();
                Play();
            };

            animation.Completed += handler;

            _translateX.BeginAnimation(TranslateTransform.XProperty, animation);
        }
    }
}
