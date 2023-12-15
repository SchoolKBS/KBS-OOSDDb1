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
    public class MapPageStreet
    {
        public static Grid GenerateStreet(Street street)
        {
            var coordinates = street.GetStreetPositions();

            RotateTransform rotate;
            if (coordinates[2] > coordinates[3])
            {
                rotate = new RotateTransform(0, 0, 0);
                coordinates[3] = 20;
            }
            else
            {
                rotate = new RotateTransform(90, 0, 0);
                coordinates[2] = 20;
            }
            Grid canvasStreet = CreateCanvasStreet(street, coordinates);
            TextBlock textBlock = createTextBlock(rotate);

            canvasStreet.Tag = textBlock;

            canvasStreet.Children.Add(textBlock);

            canvasStreet.MouseEnter += (sender, e) =>
            {
                canvasStreet.Background = Brushes.DarkCyan;
                TextBlock streetName = (TextBlock)((Grid)sender).Tag;
                streetName.Text = street.Name;

                Canvas.SetZIndex(canvasStreet, Canvas.GetZIndex(canvasStreet) + 1);
            };

            canvasStreet.MouseLeave += (sender, e) =>
            {
                canvasStreet.Background = Brushes.Black;
                TextBlock streetName = (TextBlock)((Grid)sender).Tag;
                streetName.Text = "";

                Canvas.SetZIndex(canvasStreet, Canvas.GetZIndex(canvasStreet) - 1);
            };

            Canvas.SetTop(canvasStreet, coordinates[1]);  // YCord1 to place from top.
            Canvas.SetLeft(canvasStreet, coordinates[0]); // XCord1 to place from left.

            return canvasStreet;
        }

        private static TextBlock createTextBlock(RotateTransform rotate)
        {
            return new TextBlock
            {
                Text = "",
                Foreground = Brushes.White,
                LayoutTransform = rotate,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
            };
        }

        private static Grid CreateCanvasStreet(Street street, int[] coordinates)
        {
            return new Grid
            {
                Width = coordinates[2],
                Height = coordinates[3],
                Background = Brushes.Black,
                Name = "Street_" + street.StreetID.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
    }
}
