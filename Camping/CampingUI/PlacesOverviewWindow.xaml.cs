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
        private Camping _camping { get; set; }
        private IEnumerable<Place> _placesSortedAndOrFiltered { get; set; }
        private bool? _hasPower { get; set; }
        private int _personsSizeOfPlace = 0;
        private string ArrivalDateString { get; set; }
        private string DepartureDateString { get; set; }
        private bool _isSortedAscendingPlaceNumber, _isSortedAscendingPrice, _isSortedAscendingPersonsCount;

        public PlacesOverviewPage()
        {
            InitializeComponent();
            this._camping = new Camping(); // Creates a camping.
            _placesSortedAndOrFiltered = _camping.Places;
            if (_camping.Places.Count() > 0)
            {
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;   // For all items in the ListBox use the camping places.  
            }

        }
        //Function to set the _hasPower based on the value of PowerRadioButtons
        private void PowerRadioButton_Selected(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (radioButton.Content.ToString().Equals("Wel stroom")) _hasPower = true;
            else if (radioButton.Content.ToString().Equals("Geen stroom")) _hasPower = false;
            else _hasPower = null;
        }
        //Function to check if the PersonCountTextBox is filled to either display the placeholder or not
        private void PersonCountTextBox_Changed(Object sender, TextChangedEventArgs e)
        {
            if (PersonCountTextBox.Text != "") PersonCountPlaceholder.Visibility = Visibility.Hidden;
            else PersonCountPlaceholder.Visibility = Visibility.Visible;
        }
        private void SetPersonCountFromPersonCountTextBox()
        {
            int number;
            string TextFromPersonCountTextBox = PersonCountTextBox.Text;
            if (int.TryParse(TextFromPersonCountTextBox, out number))
            {
                _personsSizeOfPlace = number;
                if (_personsSizeOfPlace < 1)
                {
                    //throw new ArgumentOutOfRangeException(); //Getal kleiner dan 1
                }
                else
                {
                    if (_personsSizeOfPlace % 2 == 1)
                    {
                        _personsSizeOfPlace += 1;
                    }
                }
            }
            else
            {
                //throw new FormatException(); //Exceptie voor geen getal
            }
        }
        private string GetDatePickerDate(DatePicker datePicker)
        {
            if (datePicker.SelectedDate.HasValue)
            {
                return datePicker.SelectedDate.Value.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }
            //Kijken in de reserveringen lijst op die specifieke plek, en kijken of de gekozen tijdperiode nog niet bestaat
        }
        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            SetPersonCountFromPersonCountTextBox();
            //ArrivalDateString = GetDatePickerDate(ArrivalDatePicker);
            //DepartureDateString = GetDatePickerDate(DepartureDatePicker);
            if (_hasPower != null)
            {
                _placesSortedAndOrFiltered = _camping.Places.Where(i => i.HasPower == _hasPower)
                                                           .Where(i => i.NumberOfPeople >= _personsSizeOfPlace)
                                                           .Select(i => i);
            }   
            else 
            {
                _placesSortedAndOrFiltered = _camping.Places.Where(i => i.NumberOfPeople >= _personsSizeOfPlace)
                                                                     .Select(i => i);
            }
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;

        }
        private IEnumerable<Place> FilterOnPower()
        {
            return _camping.Places.Where(i => i.HasPower == _hasPower).Select(i => i);
        }
        private IEnumerable<Place> FilterOnPersonsCount()
        {
            return _camping.Places.Where(i => i.NumberOfPeople == _personsSizeOfPlace).Select(i => i);
        }
        private IEnumerable<Place> FilterOnDate()
        {
            //Per plaats
            //Vraag de reserverings datums op
            //Maak van verschil start en eind datum alle datums gereserveerd
            //Check op deze lijst, of de start of einddatums voorkomen in de gereserveerde datums lijst.
            //geef een lijst terug van plaatsen waarbij zowel begin als eind geen gereserveerde datum is.
            return null;
        }
        //Filter with one selected
        private void Filter(IEnumerable<Place> filter1)
        {
            PlacesListView.ItemsSource = filter1;
        }
        //Filter with 2 selected
        private void Filter(IEnumerable<Place> filter1, IEnumerable<Place> filter2)
        {
            PlacesListView.ItemsSource = filter1.Intersect(filter2);
        }
        //Filter with 3 selected
        private void Filter(IEnumerable<Place> filter1, IEnumerable<Place> filter2, IEnumerable<Place> filter3)
        {
            PlacesListView.ItemsSource = filter1.Intersect(filter2.Intersect(filter3));
        }
        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            //Datums leegmaken
            ArrivalDatePicker.SelectedDate = null;
            DepartureDatePicker.SelectedDate = null;
            //PowerCheckBox.IsChecked = false;
            PowerRadioButton3.IsChecked = true;
            PersonCountTextBox.Text = "";
            PlacesListView.ItemsSource = _camping.Places;
        }
        public void SetSorterColumn_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader gridViewColumnHeader = (GridViewColumnHeader)sender;
            if (gridViewColumnHeader.Tag.ToString().Equals("Placenumber"))
            {
                if (_isSortedAscendingPlaceNumber) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PricePerNight).Select(i => i);
                else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PricePerNight).Select(i => i);
                _isSortedAscendingPlaceNumber = !_isSortedAscendingPlaceNumber;
            }
            else if (gridViewColumnHeader.Tag.ToString().Equals("Price"))
            {
                if (_isSortedAscendingPrice) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PricePerNight).Select(i => i);
                else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PricePerNight).Select(i => i);
                _isSortedAscendingPrice = !_isSortedAscendingPrice;
            }
            else
            {
                if (_isSortedAscendingPersonsCount) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.NumberOfPeople).Select(i => i);
                else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.NumberOfPeople).Select(i => i);
                _isSortedAscendingPersonsCount = !_isSortedAscendingPersonsCount;
            }
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
        }
    }
}
