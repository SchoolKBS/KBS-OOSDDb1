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
            var coordinates = area.GetAreaPositions();

            Canvas canvasArea = new Canvas
            {
                Width = coordinates[2],
                Height = coordinates[3],
                Background = new SolidColorBrush(StaticUIMethods.GetColor(area.Color)),
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
