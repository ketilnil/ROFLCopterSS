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
        private readonly Image              _copter;
        private readonly List<Grid>         _targetGrids;
        private readonly Random             _rnd = new Random();
        private readonly TranslateTransform _tt;

        private Grid            _activeGrid;
        private TransformGroup  _group;


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

            _tt = new TranslateTransform(_copter.ActualWidth * 2, 0);

            _group = new TransformGroup();
            _copter.RenderTransform = _group;
            _copter.Width = 300;
            _copter.Height = 300;

            _group.Children.Add(_tt);

            SetActiveGrid();
            Play();
        }


        private void SetActiveGrid()
        {
            var index = _rnd.Next(2);
            _activeGrid = _targetGrids[index];
            Debug.WriteLine($"Active Grid Index: { index }");
        }


        public void Play()
        {
            _activeGrid.Children.Add(_copter);
            double width = _activeGrid.RenderSize.Width;

            var animation = new DoubleAnimation((width / 2) * -1, width / 2 + _copter.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 10)));

            EventHandler handler = null;
            handler = (sender, args) =>
            {
                animation.Completed -= handler;
                _activeGrid.Children.Remove(_copter);
                SetActiveGrid();
                Play();
            };

            animation.Completed += handler;

            _tt.BeginAnimation(TranslateTransform.XProperty, animation);
        }
    }
}
