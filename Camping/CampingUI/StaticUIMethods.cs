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

    }
}
