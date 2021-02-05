using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace TT.Diary.Desktop.Views.Controls
{
    /// <summary>
    /// Interaction logic for StarRating.xaml
    /// </summary>
    public partial class StarRating : StackPanel
    {
        private const int STARS_COUNT = 5;

        public static readonly DependencyProperty RatingColorProperty = DependencyProperty.Register(
            "RatingColor",
            typeof(SolidColorBrush),
            typeof(StarRating),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gold)));

        public SolidColorBrush RatingColor
        {
            get => (SolidColorBrush)GetValue(RatingColorProperty);
            set => SetValue(RatingColorProperty, value);
        }

        public static readonly DependencyProperty DefaultColorProperty = DependencyProperty.Register(
            "DefaultColor",
            typeof(SolidColorBrush),
            typeof(StarRating),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LemonChiffon)));

        public SolidColorBrush DefaultColor
        {
            get => (SolidColorBrush)GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        public static readonly DependencyProperty RatingValueProperty = DependencyProperty.Register(
            "RatingValue",
            typeof(int),
            typeof(StarRating),
            new PropertyMetadata(0, new PropertyChangedCallback(RatingValueChanged)));

        public int RatingValue
        {
            get
            {

                return (int)GetValue(RatingValueProperty);
            }
            set
            {
                if (value < 0)
                {
                    SetValue(RatingValueProperty, 0);
                    return;
                }

                if (value > STARS_COUNT)
                {
                    SetValue(RatingValueProperty, STARS_COUNT);
                    return;
                }

                SetValue(RatingValueProperty, value);
            }
        }

        public StarRating()
        {
            InitializeComponent();
        }

        private static void RatingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StarRating parent = sender as StarRating;
            var ratingValue = (int)e.NewValue;
            ToggleButton[] stars = parent.Children.OfType<ToggleButton>().ToArray();

            if (stars.Length != STARS_COUNT)
                throw new ArgumentOutOfRangeException(string.Format("Stars' count cannot be greater than {0}.", STARS_COUNT));

            for (var i = 0; i < stars.Count(); i++)
            {
                stars[i].IsChecked = i < ratingValue;
            }
        }

        private void RatingButtonClickEventHandler(Object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            RatingValue = int.Parse((string)button.Tag);
        }
    }
}
