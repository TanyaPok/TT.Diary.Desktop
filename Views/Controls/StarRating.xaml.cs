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
            get => (SolidColorBrush) GetValue(RatingColorProperty);
            set => SetValue(RatingColorProperty, value);
        }

        public static readonly DependencyProperty DefaultColorProperty = DependencyProperty.Register(
            "DefaultColor",
            typeof(SolidColorBrush),
            typeof(StarRating),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LemonChiffon)));

        public SolidColorBrush DefaultColor
        {
            get => (SolidColorBrush) GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        public static readonly DependencyProperty RatingValueProperty = DependencyProperty.Register(
            "RatingValue",
            typeof(int),
            typeof(StarRating),
            new PropertyMetadata(0, new PropertyChangedCallback(RatingValueChanged)));

        public int RatingValue
        {
            get => (int) GetValue(RatingValueProperty);
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
            if (sender is not StarRating parent) return;
            var ratingValue = (int) e.NewValue;
            var stars = parent.Children.OfType<ToggleButton>().ToArray();

            if (stars.Length != STARS_COUNT)
                throw new ArgumentOutOfRangeException($"Stars' count cannot be greater than {STARS_COUNT}.");

            for (var i = 0; i < stars.Count(); i++)
            {
                stars[i].IsChecked = i < ratingValue;
            }
        }

        private void RatingButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton button) return;
            var rating = int.Parse((string) button.Tag);

            if (button.IsChecked.HasValue && !button.IsChecked.Value && RatingValue == rating)
            {
                RatingValue = 0;
            }
            else
            {
                RatingValue = rating;
            }
        }
    }
}