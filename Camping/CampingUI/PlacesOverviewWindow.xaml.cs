using CampingCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
    /// Interaction logic for PlacesOverviewPage.xaml
    /// </summary>
    public partial class PlacesOverviewPage : Page
    {
        public Camping Camping { get; set; }
        public bool HasPower { get; set; }
        public int PersonsSizeOfPlace { get; set; }

        public PlacesOverviewPage()
        {
            InitializeComponent();
            this.Camping = new Camping(); // Creates a camping.
            if (Camping.Places.Count() > 0)
            {
                PlacesListView.ItemsSource = Camping.Places;   // For all items in the ListBox use the camping places.  
            }
        }
        private void GetValueFromPowerCheckBox()
        {
            HasPower = (bool)PowerCheckBox.IsChecked;
        }
        private void PersonCountTextBox_Changed(Object sender, TextChangedEventArgs e)
        {
            if (PersonCountTextBox.Text != "") PersonCountPlaceholder.Visibility = Visibility.Hidden;
            else PersonCountPlaceholder.Visibility = Visibility.Visible;
        }
        private void GetTextFromTextbox()
        {
            int number;
            string TextFromPersonCountTextBox = PersonCountTextBox.Text;
            if (int.TryParse(TextFromPersonCountTextBox, out number))
            {
                PersonsSizeOfPlace = number;
                if (PersonsSizeOfPlace < 1)
                {
                    throw new ArgumentOutOfRangeException(); //Getal kleiner dan 1
                }
                else
                {
                    if (PersonsSizeOfPlace % 2 == 1)
                    {
                        PersonsSizeOfPlace += 1;
                    }
                }
            }
            else
            {
                PersonsSizeOfPlace = 0;
                //throw new FormatException(); //Exceptie voor geen getal
            }
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {

            GetValueFromPowerCheckBox();
            GetTextFromTextbox();
            var results = Camping.Places.Where(i => i.HasPower == HasPower)
                                        .Where(i => i.NumberOfPeople > PersonsSizeOfPlace)
                                        .Select(i => i);
            PlacesListView.ItemsSource = results;
        }

        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            //Datums leegmaken
            PowerCheckBox.IsChecked = false;
            PersonCountTextBox.Text = "";
            PlacesListView.ItemsSource = Camping.Places;
        }   
    }
}
