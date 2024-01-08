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
        public static void ResetComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.White;
            border.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        public static void SetErrorComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.Red;
            border.BorderThickness = new Thickness(3, 3, 3, 3);
        }
        public static Color GetColorFromInt(int ColorValue)
        {
            switch (ColorValue)
            {
                case 0:
                    return Colors.Red;
                case 1:
                    return Colors.Orange;
                case 2:
                    return Colors.Yellow;
                case 3:
                    return Colors.Green;
                case 4:
                    return Colors.Blue;
                case 5:
                    return Colors.Purple;
                case 6:
                    return Colors.Violet;
                case 7:
                    return Colors.Pink;
                default:
                    return Colors.Beige;
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
