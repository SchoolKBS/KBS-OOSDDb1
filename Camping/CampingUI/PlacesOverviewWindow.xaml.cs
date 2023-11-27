using CampingCore;
using CampingDataAccess;
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
        private int GetComboBoxResult()
        {
            return 0;
        }
        private void PersonCountTextBox_Changed(Object sender, TextChangedEventArgs e)
        {
            if (PersonCountTextBox.Text != "") PersonCountPlaceholder.Visibility = Visibility.Hidden;
            else PersonCountPlaceholder.Visibility = Visibility.Visible;
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {

            GetValueFromPowerCheckBox();
            GetTextFromTextbox();
            MessageBox.Show($"{HasPower}");
        }
        private void GetTextFromTextbox()
        {
            /*PersonsSizeOfPlace = 0;
            string TextFromPersonCountTextBox = PersonCountTextBox.ToString();
            if(int.TryParse(PersonsSizeOfPlace, out PersonsSizeOfPlace))
            {
                if(PersonsSizeOfPlace < 1)
                {
                    throw new ArgumentOutOfRangeException(); //Getal kleiner dan 1
                }
                else
                {
                    if(PersonsSizeOfPlace % 2 == 1)
                    {
                        PersonsSizeOfPlace++;
                    }
                }
            }
            else
            {
                throw new FormatException(); //Exceptie voor geen getal
            }*/
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

        public void AddPlaceOnClick(object sender, RoutedEventArgs e)
        {
            string[] TextInput =
            {
                PlaceNumber.Text,
                SurfaceArea.Text,
                PricePerPersonPerNight.Text,
                NumberOfPeople.Text,
                HasElectricity.SelectionBoxItem.ToString()
            };

            //Checks if the required textboxes are filled
            if (CheckIfInputIsNotNull(TextInput))
            {
                AddPlaceToDatabase();
            }
        }

        public void AddPlaceToDatabase()
        {
            try
            {
                //Parses the string inputs from textboxes to ints
                int placeNumber = Int32.Parse(PlaceNumber.Text);
                int surfaceArea = Int32.Parse(SurfaceArea.Text);
                int pricePerPersonPerNight = Int32.Parse(PricePerPersonPerNight.Text);
                int amountOfPeople = Int32.Parse(NumberOfPeople.Text);
                string electricity = HasElectricity.SelectionBoxItem.ToString();
                string placeDescription = PlaceDescription.Text;


                //Checks if the place has electricity 
                bool hasElectricity;
                if (electricity.Equals("Ja"))
                {
                    hasElectricity = true;
                }
                else
                {
                    hasElectricity = false;
                }

                //Make a new place with the input of the textboxes
                Place place = new Place(placeNumber, hasElectricity, surfaceArea, amountOfPeople, pricePerPersonPerNight, placeDescription);

                Database db = new Database();
                db.AddPlaceToDatabase(place);

                //Clears textboxes when the data is inserted in the database
                foreach (var textbox in AddPlace.Children)
                {
                    if (textbox is TextBox textBox)
                    {
                        textBox.Text = string.Empty;
                    }
                }

                AddPlaceMessage.Text = "Nieuwe plaats is toegevoegd";
                AddPlaceMessage.Foreground = Brushes.Green;

            } catch(Exception ex)
            {
                AddPlaceMessage.Text = "Ongeldigde input";
                AddPlaceMessage.Foreground = Brushes.Red;
            }

        }
        public bool CheckIfInputIsNotNull(string[] TextInput)
        {
            foreach (string input in TextInput)
            {
                if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                {
                    AddPlaceMessage.Text = "Nog niet alle benodigde velden zijn ingevuld";
                    AddPlaceMessage.Foreground = Brushes.Red;
                    return false;
                }
            }
            return true;
        }
    }
}
