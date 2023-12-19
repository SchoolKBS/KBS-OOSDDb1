using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CampingUI.GenerateComponentsMap
{
    public class MapPageStreet
    {
        private static Line _line;
        private static TextBlock _textblock;
        public static void GenerateStreet(Street street, SolidColorBrush? color)
        {
            var coordinates = street.GetStreetPositions();
            double deltaY;
            double deltaX;
            double firstX;
            double firstY;
            double secondX;
            double secondY;

            if(street.XCord1 < street.XCord2)
            {
                deltaX = street.XCord2 - street.XCord1;
                firstX = street.XCord1;
                secondX = street.XCord2;
            }
            else
            {
                deltaX = street.XCord1 - street.XCord2;
                firstX = street.XCord2;
                secondX = street.XCord1;
            }
            if(street.YCord1 > street.YCord2)
            {
                deltaY = street.YCord1 - street.YCord2;
                firstY = street.YCord1;
                secondY = street.YCord2;
            }
            else
            {
                deltaY = street.YCord2 - street.YCord1;
                firstY = street.YCord2;
                secondY = street.YCord1;
            }
           // MessageBox.Show($"{firstX}, {firstY}, {secondX}, {secondY}");
            double degrees = Math.Atan(deltaY / deltaX) * 180 / Math.PI;
            if (secondY < firstY && secondX > firstX)
            {
                degrees = 360 - (2 * degrees);
            }
            RotateTransform rotate = new RotateTransform();
            rotate.Angle = degrees;
            Line line = CreateCanvasStreet(street, coordinates);
            TextBlock textBlock = createTextBlock(rotate);
            Canvas.SetLeft(textBlock, firstX-(deltaX / 2));
            Canvas.SetTop(textBlock, firstY - (deltaY / 2));
            SetLine(line);
            SetTextBlock(textBlock);

            line.MouseEnter += (sender, e) =>
            {
                line.Stroke = Brushes.DarkCyan;
                textBlock.Text = street.Name;
                Canvas.SetZIndex(line, Canvas.GetZIndex(line) + 1);
                Canvas.SetZIndex(textBlock, Canvas.GetZIndex(textBlock) + 1);
            };
            line.MouseLeave += (sender, e) =>
            {
                line.Stroke = color;
                textBlock.Text = "";
                Canvas.SetZIndex(line, Canvas.GetZIndex(line) - 1);
                Canvas.SetZIndex(textBlock, Canvas.GetZIndex(textBlock) - 1);
            };
        }

        private static TextBlock createTextBlock(RotateTransform rotate)
        {
            return new TextBlock
            {
                Text = "",
                LayoutTransform = rotate,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Focusable = false,
            };
        }
        private static Line CreateCanvasStreet(Street street, int[] coordinates)
        {
            return new Line
            {
                X1 = coordinates[0],
                Y1 = coordinates[1],
                X2 = coordinates[2],
                Y2 = coordinates[3],
                Stroke = Brushes.Black,
                StrokeThickness = 15,
                Name = "Street_" + street.StreetID.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
        public static void SetLine(Line line)
        {
            _line = line;
        }
        public static void SetTextBlock(TextBlock textBlock)
        {
            _textblock = textBlock;
        }
        public static Line GetLine()
        {
            return _line;
        }
        public static TextBlock GetTextBlock()
        {
            return _textblock;
        }
    }
}
