using CampingCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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
        private Camping _camping;
        private IEnumerable<Place> _placesSortedAndOrFiltered;
        private bool? _hasPower;
        public int PersonCount = 0;
        private DateTime _arrivalDate, _departureDate;
        private string _arrivalDateString;
        private string _departureDateString;
        private bool _isSortedAscending;
        public int MaxPrice;

        public PlacesOverviewPage()
        {
            InitializeComponent();
            this._camping = new Camping(); // Creates a camping.
            // Set the min and max price that can be filtered on based on the given places.
            MaxPrice = _camping.Places.Max(i => i.PricePerNight);
            MaxPriceRangeTextBox.Text = $"{MaxPrice}";
            PersonCountTextBox.Text = $"{PersonCount}";
            _placesSortedAndOrFiltered = _camping.Places;
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;   // For all items in the ListBox use the camping places.  
        }
        private void PersonCountTextBox_Changed(Object sender, TextChangedEventArgs e)
        {
            if (PersonCountTextBox.Text != "") PersonCountPlaceholder.Visibility = Visibility.Hidden;
            else PersonCountPlaceholder.Visibility = Visibility.Visible;
            PersonCountTextBox.Background = Brushes.White;
        }
        private void MaxPriceRangeTextBox_Changed(object sender, TextChangedEventArgs e)
        {
            MaxPriceRangeTextBox.Background = Brushes.White;
        }
        private void PowerRadioButton_Selected(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (radioButton.Content.ToString().Equals("Wel stroom")) _hasPower = true;
            else if (radioButton.Content.ToString().Equals("Geen stroom")) _hasPower = false;
            else _hasPower = null;
        }
        private void SetPersonCountFromPersonCountTextBox()
        {
            int number;
            if (!string.IsNullOrEmpty(PersonCountTextBox.Text))
            {
                if (int.TryParse(PersonCountTextBox.Text, out number) && number >= 0)       // Checks if int can be parsed and if number is bigger or equal to 0
                {
                    PersonCount = number;
                    if (PersonCount % 2 == 1)
                    {
                        PersonCount += 1;
                    }
                }
                else
                {
                    PersonCountTextBox.Background = Brushes.Red;
                    //Maybe an excpetion?

                }
            }
            else
            {
                PersonCount = 0;
                PersonCountTextBox.Text = $"{PersonCount}";
            }
        }
        private void SetMaxPriceFromMaxPriceRangeTextBox()
        {
            int number;
            if (!string.IsNullOrEmpty(MaxPriceRangeTextBox.Text))
            {
                if (int.TryParse(MaxPriceRangeTextBox.Text, out number) && number >= 0)       // Checks if int can be parsed and if number is bigger or equal to 0
                {
                    MaxPrice = number;
                }
                else
                {
                    MaxPriceRangeTextBox.Background = Brushes.Red;
                    //Maybe an excpetion?
                }
            }
        }

        private DateTime GetDatePickerDate(DatePicker datePicker)
        {
            DateTime date = new DateTime(10, 10, 10);
            if (datePicker.SelectedDate.HasValue)
            {
                date = datePicker.SelectedDate.Value;
            }
            return date;
            //Kijken in de reserveringen lijst op die specifieke plek, en kijken of de gekozen tijdperiode nog niet bestaat
        }

        //Apply
        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            SetPersonCountFromPersonCountTextBox();
            
            SetMaxPriceFromMaxPriceRangeTextBox();
            _placesSortedAndOrFiltered = _camping.Places;
            _arrivalDate = GetDatePickerDate(ArrivalDatePicker);
            _departureDate = GetDatePickerDate(DepartureDatePicker);
            Filter(_arrivalDate, _departureDate, PersonCount, MaxPrice, _hasPower);
        }

        private void Filter(DateTime arrivalDate, DateTime departureDate, int personCount, int maxPrice, bool? hasPower)
        {
            // Check for the arrivalDate and departureDate
            GetFilteredListOnPrice(maxPrice);
            GetFilteredListOnPersonCount(personCount);
            GetFilteredListOnDate(arrivalDate, departureDate);
            GetFilteredListOnPower(hasPower);
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;

        }

        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            ArrivalDatePicker.SelectedDate = null;
            DepartureDatePicker.SelectedDate = null;
            PowerRadioButton3.IsChecked = true;
            PersonCount = 0;
            MaxPrice = _camping.Places.Max(i => i.PricePerNight);
            PersonCountTextBox.Text = $"{PersonCount}"; ; 
            MaxPriceRangeTextBox.Text = $"{MaxPrice}";
            PlacesListView.ItemsSource = _camping.Places;
        }

        private bool SortColumnPlaceNumber(bool isSorted)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PlaceNumber).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PlaceNumber).Select(i => i);
            isSorted = !isSorted;
            return isSorted;
        }
        private bool SortColumnPrice(bool isSorted)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PricePerNight).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PricePerNight).Select(i => i);
            isSorted = !isSorted;
            return isSorted;
        }
        private bool SortColumnPersonCount(bool isSorted)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PersonCount).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PersonCount).Select(i => i);
            isSorted = !isSorted;
            return isSorted;
        }
        public void SetSorterColumn_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader gridViewColumnHeader = (GridViewColumnHeader)sender;
            if (gridViewColumnHeader.Tag.ToString().Equals("Placenumber")) 
                _isSortedAscending = SortColumnPlaceNumber(_isSortedAscending);
            else if (gridViewColumnHeader.Tag.ToString().Equals("Price"))
                 _isSortedAscending = SortColumnPrice(_isSortedAscending);
            else 
                _isSortedAscending = SortColumnPersonCount(_isSortedAscending);
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
        }

        private void GetFilteredListOnPower(bool? hasPower)
        {
            if (hasPower != null)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.HasPower == _hasPower).Select(i => i));
            }
        }
        private void GetFilteredListOnPrice(int maxPrice)
        {
            if (maxPrice >= _camping.Places.Min(i => i.PricePerNight))
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.PricePerNight <= MaxPrice).Select(i => i));
            }
        }
        private void GetFilteredListOnPersonCount(int personCount)
        {
            if (personCount >= 0)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.PersonCount >= PersonCount).Select(i => i));
            }
        }
        private void GetFilteredListOnDate(DateTime arrivalDate, DateTime departureDate)
        {
            if (arrivalDate != null && departureDate != null)
            {
                if (arrivalDate.Date < departureDate.Date && arrivalDate.Date >= DateTime.Now.Date)
                {
                    _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(GetAvailablePlacesBetweenDates(arrivalDate.Date, departureDate.Date));
                }
                else
                {
                    ArrivalDatePicker.Background = Brushes.Red;
                    DepartureDatePicker.Background = Brushes.Red;
                }
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            ArrivalDatePicker.Background = Brushes.White;
            DepartureDatePicker.Background = Brushes.White;
        }

        private IEnumerable<Place> GetAvailablePlacesBetweenDates(DateTime arrivalDate, DateTime departureDate)
        {
            List<Place> availablePlacesBetweenDates = new List<Place>();
            foreach(Place place in _camping.Places)
            {
                int counter = 0;
                //Lijst van reserveringen van 1 plek
                var reservationsOnPlace = _camping.Reservations.Where(i => i.place.PlaceNumber == place.PlaceNumber).Select(i => i); 
                //Als deze lijst iets heeft
                if(reservationsOnPlace.Count() > 0)
                {
                    foreach(Reservation reservation in reservationsOnPlace)
                    {
                        if ((arrivalDate <= reservation.StartDatum.Date && reservation.StartDatum.Date <= departureDate) //Startdatum valt binnen de ingevulde aankomst- en vertrekdatum
                        || (arrivalDate <= reservation.EindDatum.Date && reservation.EindDatum.Date <= departureDate) //Einddatum valt binnen de ingevulde aankomst- en vertrekdatum
                        || (reservation.StartDatum.Date <= arrivalDate && reservation.EindDatum.Date >= departureDate)) //aankomst en vertrekdatum valt binnen een reservering
                        {
                            counter++;
                        }
                    }
                }
                if(counter == 0)
                {
                    availablePlacesBetweenDates.Add(place);
                }
            }
            return availablePlacesBetweenDates;
        }
    }
}
