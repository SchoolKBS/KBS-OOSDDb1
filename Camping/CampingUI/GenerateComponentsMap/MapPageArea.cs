using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CampingUI.NewFolder
{
    public class MapPageArea
    {
        private static int _selectionThicknes = 4;
        public static Border GenerateArea(Area area)
        {
            var coordinates = area.GetAreaPositions();

            Canvas canvasArea = new Canvas
            {
                Background = new SolidColorBrush(StaticUIMethods.GetColorFromInt(area.Color)),
            };
            canvasArea.Children.Add(new TextBlock
            {
                Text = area.Name,
                FontSize = 16,
                FontWeight = FontWeights.Bold

            });
            Border border = new Border
            {
                Width = coordinates[2],
                Height = coordinates[3],
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = canvasArea,
                Name = "Canvas_" + area.AreaID.ToString(),
            };
            Canvas.SetTop(border, coordinates[1]);
            Canvas.SetLeft(border, coordinates[0]);
            Canvas.SetZIndex(border, -1);
            return border;
        }
    }
}
