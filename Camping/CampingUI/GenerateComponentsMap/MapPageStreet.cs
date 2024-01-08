using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

            if (street.XCord1 > street.XCord2 || (street.XCord1 == street.XCord2 && street.YCord2 > street.YCord1))
            {
                (street.XCord1, street.XCord2) = (street.XCord2, street.XCord1);
                (street.YCord1, street.YCord2) = (street.YCord2, street.YCord1);
            }

            RotateTransform rotate = new RotateTransform();
            rotate.Angle = CalcAngle(street);

            Line line = CreateStreet(street, coordinates);
            TextBlock textBlock = createTextBlock(rotate);
            textBlock.Text = street.Name;

            double textBlockX;
            double textBlockY;
            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));
            CalcTextBlockXY(street, textBlock.ActualWidth, out textBlockX, out textBlockY);
            
            Canvas.SetLeft(textBlock, textBlockX);
            Canvas.SetTop(textBlock, textBlockY);

            line.Stroke = color;
            SetLine(line);
            SetTextBlock(textBlock);
        }
        public static void CalcTextBlockXY(Street street, double textblockActualWidth, out double textBlockX, out double textBlockY)
        {
            if (street.XCord1 < street.XCord2)
            {
                textBlockX = street.XCord1;
                textBlockY = street.YCord1;
            }
            else
            {
                textBlockX = street.XCord2;
                textBlockY = street.YCord2;
            }
            double LineLenght = Math.Sqrt(Math.Pow(street.XCord2 - street.XCord1, 2) + Math.Pow(street.YCord1 - street.YCord2, 2));

            if (street.XCord1 == street.XCord2)
            {
                textBlockX += 10;
                textBlockY += (LineLenght / 2 - textblockActualWidth / 2);
            }
            else if (street.YCord1 == street.YCord2)
            {
                textBlockX += (LineLenght / 2 - textblockActualWidth / 2);
                textBlockY -= 10;
            }
            else
            {
                textBlockY -= 10 * (Math.Cos((street.YCord2 - street.YCord1) / LineLenght)) - Street.CalcSideLenght(street, textblockActualWidth, false);
                textBlockX += 10 * (Math.Sin((street.YCord2 - street.YCord1) / LineLenght)) + Street.CalcSideLenght(street, textblockActualWidth, true);
            }


        }
        public static int CalcAngle(Street street)
        {
            if (street.YCord1 < street.YCord2 && street.XCord1 < street.XCord2)
            {
                return (int)(Math.Atan2(street.YCord2 - street.YCord1, street.XCord2 - street.XCord1) * (180 / Math.PI));
            }
            else if (street.YCord1 > street.YCord2 && street.XCord1 < street.XCord2)
            {
                return (int)(Math.Atan2(street.YCord1 - street.YCord2, street.XCord1 - street.XCord2) * (180 / Math.PI)) + 180;
            }
            else if (street.YCord1 == street.YCord2)
            {
                return 0;
            }
            else 
            {
                return 90;
            }
        }
        public static TextBlock createTextBlock(RotateTransform rotate)
        {
            return new TextBlock
            {
                Text = "",

                IsHitTestVisible = false,
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
