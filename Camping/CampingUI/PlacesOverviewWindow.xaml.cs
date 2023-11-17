using CampingCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            if (Camping.Places.Count() > 0)
            {
                PlacesListView.ItemsSource = Camping.Places;   // For all items in the ListBox use the camping places.  

            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        /*private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                PlacesListView.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            PlacesListView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }*/
    }
}
