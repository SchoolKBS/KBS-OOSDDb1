using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CampingUI.GenerateComponentsMap
{
    public class MapPagePlace
    {
        public static Canvas Canvas;
        public static Border GeneratePlace(Canvas field, Place place, SolidColorBrush brush, bool AddPlaceBool)
        {
            var coordinates = place.GetPlacePositions();

            Border border = new Border
            {
                BorderBrush = Brushes.White,
                Width = 30,
                Height = 30,
                BorderThickness = new Thickness(1),
                Name = "Place_" + place.PlaceID.ToString(),
            };

            Canvas = new Canvas
            {
                Background = brush,
                Name = "Place" + place.PlaceID.ToString(),
            };

            border.Child = Canvas;
            Canvas.SetZIndex(Canvas, 100);
            Canvas.SetTop(border, coordinates[1]);
            Canvas.SetLeft(border, coordinates[0]);

            field.Children.Add(border);

            if (AddPlaceBool)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = place.PlaceID.ToString(),
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 20,
                };

                Canvas.Children.Add(textBlock);
                Canvas.Loaded += (sender, e) =>
                {
                    Canvas.SetTop(textBlock, (Canvas.ActualHeight - textBlock.ActualHeight) / 2);
                    Canvas.SetLeft(textBlock, (Canvas.ActualWidth - textBlock.ActualWidth) / 2);
                };
            }

            Canvas.SetTop(Canvas, (border.ActualHeight - Canvas.Height) / 2);
            Canvas.SetLeft(Canvas, (border.ActualWidth - Canvas.Width) / 2);
            return border;
        }
    }
}
