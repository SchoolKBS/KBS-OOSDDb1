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
                Name = "Canvas_" + area.AreaID.ToString(),
            };
            canvasArea.Children.Add(new TextBlock
            {
                Text = area.Name
            });
            Border border = new Border
            {
                Width = coordinates[2],
                Height = coordinates[3],
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = canvasArea,
            };
            Canvas.SetTop(border, coordinates[1]);
            Canvas.SetLeft(border, coordinates[0]);
            Canvas.SetZIndex(border, -1);
            return border;
        }
        public static Border SelectBorder(Border border, Area area)
        {
            var coordinates = area.GetAreaPositions();
            border.BorderThickness = new Thickness(_selectionThicknes);
            border.BorderBrush = Brushes.LightBlue;
            return border;
        }        
        public static Border DeselectBorder(Border border, Area area)
        {
            var coordinates = area.GetAreaPositions();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            return border;
        }
    }
}
