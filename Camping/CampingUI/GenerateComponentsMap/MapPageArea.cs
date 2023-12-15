using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace CampingUI.NewFolder
{
    public class MapPageArea
    {
        public static Border GenerateArea(Area area)
        {
            byte rColor = byte.Parse(area.Color.Substring(0, 3));
            byte gColor = byte.Parse(area.Color.Substring(3, 3));
            byte bColor = byte.Parse(area.Color.Substring(6, 3));
            var coordinates = area.GetAreaPositions();

            Canvas canvasArea = new Canvas
            {
                Width = coordinates[2],
                Height = coordinates[3],
                Background = new SolidColorBrush(Color.FromRgb(rColor, gColor, bColor)),
                Name = "Canvas_" + area.AreaID.ToString(),
            };

            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = canvasArea,
            };

            Canvas.SetTop(border, coordinates[1]);
            Canvas.SetLeft(border, coordinates[0]);
            Canvas.SetZIndex(border, -1);
            return border;
        }
    }
}
