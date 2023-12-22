using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace CampingUI
{
    public static class StaticUIMethods
    {
        public static int ColorCount = 8;
        public static void SetErrorTextboxBorder(TextBox textbox)
        {
            textbox.BorderBrush = Brushes.Red;
            textbox.BorderThickness = new Thickness(3);
        }
        public static void SetErrorDatePickerBorder(DatePicker datePicker)
        {
            datePicker.BorderBrush = Brushes.Red;
            datePicker.BorderThickness = new Thickness(3);
        }
        public static void ResetTextboxBorder(TextBox textbox)
        {
            textbox.BorderBrush = Brushes.White;
            textbox.BorderThickness = new Thickness(1);
        }
        public static void ResetDatePickerBorder(DatePicker datePicker)
        {
            datePicker.BorderBrush = Brushes.White;
            datePicker.BorderThickness = new Thickness(1);
        }
        public static Color GetColorFromInt(int ColorValue)
        {
            switch (ColorValue)
            {
                case 0:
                    return System.Windows.Media.Colors.Red;
                case 1:
                    return System.Windows.Media.Colors.Orange;
                case 2:
                    return System.Windows.Media.Colors.Yellow;
                case 3:
                    return System.Windows.Media.Colors.Green;
                case 4:
                    return System.Windows.Media.Colors.Blue;
                case 5:
                    return System.Windows.Media.Colors.Purple;
                case 6:
                    return System.Windows.Media.Colors.Violet;
                case 7:
                    return System.Windows.Media.Colors.Pink;
                default:
                    return System.Windows.Media.Colors.Beige;
            }
        }        
        public static string GetColorNameFromInt(int ColorValue)
        {
            switch (ColorValue)
            {
                case 0:
                    return "Rood";
                case 1:
                    return "Oranje";
                case 2:
                    return "Geel";
                case 3:
                    return "Groen";
                case 4:
                    return "Blauw";
                case 5:
                    return "Paars";
                case 6:
                    return "Violet";
                case 7:
                    return "Roze";
                default:
                    return "-";
            }
        }
    }
}
