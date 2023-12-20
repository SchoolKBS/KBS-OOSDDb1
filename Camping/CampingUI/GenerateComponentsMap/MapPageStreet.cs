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
        private static Line _streetLine;
        public static void GenerateStreet(Street street, SolidColorBrush? color)
        {
            var coordinates = street.GetStreetPositions();
            double deltaY;
            double deltaX;
            double firstX;
            double firstY;
            double secondX;
            double secondY;
            /*//if (street.XCord1 < street.XCord2)
            //{
            //deltaX = street.XCord2 - street.XCord1;
            //firstX = street.XCord1;
            // secondX = street.XCord2;
            //}
            // else
            //{
            //deltaX = street.XCord1 - street.XCord2;
            //firstX = street.XCord2;
            //secondX = street.XCord1;
            //}
            //if (street.YCord1 > street.YCord2)
            // {
            //deltaY = street.YCord1 - street.YCord2;
            //firstY = street.YCord1;
            //secondY = street.YCord2;
            //}
            //else
            //{
            //deltaY = street.YCord2 - street.YCord1;
            //firstY = street.YCord2;
            //secondY = street.YCord1;
            //}
            //double radials = Math.Atan(deltaY / deltaX);
            //double degrees = radials * 180 / Math.PI;

            // int offsetX = 0;
            //int offsetY = 0;
            //Line starts on the left, ends on the right
            //if (street.XCord1 <= street.XCord2)
            //{
            //Line starts lower and gets higher
            //if (street.YCord1 >= street.YCord2)
            //{
            // degrees *= -1;
            //offsetX = 15;
            // }
            //Line starts higher and gets lower
            // else
            //{
            //degrees *= 1;
            //offsetX = 10;
            //}
            //}
            //Line starts on the right, ends on the left
            //else
            // {
            //Line starts lower and gets higher
            //if (street.YCord1 >= street.YCord2)
            //{

            //degrees *= 1;
            //}
            //Line starts higher and gets lower
            //else
            //{
            //degrees *= -1;
            //offsetX = 35;
            //}
            //}
            //Vertical line
            //if (street.XCord1 == street.XCord2)
            //{
            //degrees = 90;
            //offsetX = 15;
            // }
            //Horizontal line
            // if (street.YCord1 == street.YCord2)
            //{
            // offsetY = 10;
            //}*/

            int angle = 0;
            if(street.YCord1 < street.YCord2 && street.XCord1 < street.XCord2)
            {
                angle = (int)(Math.Atan2(street.YCord2 - street.YCord1, street.XCord2 - street.XCord1) * (180/Math.PI));
            } else
            {
                angle = (int)(Math.Atan2(street.YCord1 - street.YCord2, street.XCord1 - street.XCord2) * (180/Math.PI));
            }
            if(street.YCord1 > street.YCord2 && street.XCord1 < street.XCord2)
            {
                angle = (int)(Math.Atan2(street.YCord1 - street.YCord2, street.XCord1 - street.XCord2) * (180 / Math.PI))+180;
            }
            if(street.YCord1 == street.YCord2) {
                angle = 0;
            }
            if(street.XCord1 == street.XCord2)
            {
                angle = 90;
            }
            RotateTransform rotate = new RotateTransform();
            rotate.Angle = angle;

            Line line = CreateStreet(street, coordinates);
            TextBlock textBlock = createTextBlock(rotate);
            //Canvas.SetLeft(textBlock, secondX - (deltaX / 2) +(offsetX * (radials)));
            //Canvas.SetTop(textBlock, firstY - (deltaY / 2) + (offsetY * (radials)));

            double textBlockX = ((street.XCord1 + street.XCord2 - textBlock.ActualWidth) / 2);
            double textBlockY = ((street.YCord1 + street.YCord2 - textBlock.ActualHeight) / 2);

            Canvas.SetLeft(textBlock, textBlockX);
            Canvas.SetTop(textBlock, textBlockY);

            textBlock.Text = street.Name;
            line.Stroke = color;

            line.MouseEnter += (sender, e) =>
            {
                line.Stroke = Brushes.DarkCyan;
            };

            line.MouseLeave += (sender, e) =>
            {
               if(_streetLine == null || line != _streetLine) {
                    line.Stroke = Brushes.Black;
                }
            };

            line.MouseDown += (sender, e) =>
            {
                if(_streetLine != null)
                {
                    _streetLine.Stroke = Brushes.Black;
                }
                line.Stroke = Brushes.DarkCyan;
                _streetLine = line;
            };

            SetLine(line);
            SetTextBlock(textBlock);
        }
       
        public static TextBlock createTextBlock(RotateTransform rotate)
        {
            return new TextBlock
            {
                Text = "",

                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = rotate,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Focusable = false,

            };
        }
        private static Line CreateStreet(Street street, int[] coordinates)
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
