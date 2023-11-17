using CampingCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for PlacesOverviewWindow.xaml
    /// </summary>
    public partial class PlacesOverviewWindow : Window
    {
        public Camping Camping { get; set; }
        public PlacesOverviewWindow()
        {
            InitializeComponent();
            
            
            this.Camping = new Camping(); // Creates a camping.

            ListBox listbox = new ListBox(); // Creates a ListBox to show in the WPF UI
            Grid.SetRow(listbox, 1);        // Adds a new row to the ListBox.
            listbox.ItemsSource = Camping.Places;   // For all items in the ListBox use the camping places.
            ListGrid.Children.Add(listbox);     // Adds each items inside the listBox to the grid UI.
            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
