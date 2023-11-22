using CampingCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private bool _isSortedAscending;
        private double _maxPriceRange;
        private bool _wrongFilter = false;

        public PlacesOverviewPage()
        {
            InitializeComponent();
            this._camping = new Camping(); // Creates a camping.
            _maxPriceRange = _camping.Places.Max(i => i.PricePerNight);
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}"; //Set the _maxPriceRange as a standard
            PersonCountTextBox.Text = $"{PersonCount}"; //Set the text in the textbox to 0
            _placesSortedAndOrFiltered = _camping.Places; //get all the places to the variable
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;   // For all items in the ListBox use the camping places.  
        }

        // Function (EventHandler) to check if the text in the PersonCountTextBox is empty or not
        // and to show the placeholdertext or the filled in text
        private void PersonCountTextBox_Changed(Object sender, TextChangedEventArgs e)
        {
            if (PersonCountTextBox.Text != "") PersonCountPlaceholder.Visibility = Visibility.Hidden;
            else PersonCountPlaceholder.Visibility = Visibility.Visible;
            if (PersonCountTextBox.Background.Equals(Brushes.Red))
            {
                PersonCountTextBox.Background = Brushes.White;
                _wrongFilter = false;
            }
        }

        // Function (EventHandler) to check if the MaxPriceRangeTextBox has been changed to reset
        // the background color to white incase the color was previously red
        private void MaxPriceRangeTextBox_Changed(object sender, TextChangedEventArgs e)
        {
            if (MaxPriceRangeTextBox.Background.Equals(Brushes.Red))
            {
                MaxPriceRangeTextBox.Background = Brushes.White;
                _wrongFilter = false;
            }
        }

        //Function (EventHandler) to reset the datepickers to backgroundcolor white incase they were red before
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ArrivalDatePicker.Background.Equals(Brushes.Red) && DepartureDatePicker.Background.Equals(Brushes.Red))
            {
                ArrivalDatePicker.Background = Brushes.White;
                DepartureDatePicker.Background = Brushes.White;
                _wrongFilter = false;
            }
        }

        // Function (EventHandler) to know which radiobutton is pressed regarding the power boolean
        private void PowerRadioButton_Selected(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (radioButton.Content.ToString().Equals("Wel stroom")) _hasPower = true;
            else if (radioButton.Content.ToString().Equals("Geen stroom")) _hasPower = false;
            else _hasPower = null;
        }

        // Function to get the PersonCount from the PersonCountTextBox text
        private void SetPersonCountFromPersonCountTextBox()
        {
            int number;
            if (!string.IsNullOrEmpty(PersonCountTextBox.Text))
            {
                if (int.TryParse(PersonCountTextBox.Text, out number) && number >= 0)// Checks if int can be parsed and if number is bigger or equal to 0
                {
                    PersonCount = number;
                }
                else
                {
                    PersonCountTextBox.Background = Brushes.Red;
                    _wrongFilter = true;
                }

            }
            else
            {
                PersonCount = 0;
                PersonCountTextBox.Text = $"{PersonCount}";
            }
        }

        // Function to get the max price range from the MaxPriceRangeTextBox text
        private void SetMaxPriceFromMaxPriceRangeTextBox()
        {
            double number;
            if (!string.IsNullOrEmpty(MaxPriceRangeTextBox.Text))
            {
                string MaxPriceRangeText = MaxPriceRangeTextBox.Text.Replace(".", ",");
                if (double.TryParse(MaxPriceRangeText, out number) && number >= 0)       // Checks if int can be parsed and if number is bigger or equal to 0
                {
                    _maxPriceRange = number;
                }
                else
                {
                    MaxPriceRangeTextBox.Background = Brushes.Red;
                    _wrongFilter = true;
                }
                    
            }
            else
            {
                _maxPriceRange = _camping.Places.Max(i => i.PricePerNight);
                MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";
            }
        }

        //Function to set the _arrivalDate and _departureDate
        private void SetArrivalAndDepartureDates()
        {
            _arrivalDate = GetDatePickerDate(ArrivalDatePicker);
            _departureDate = GetDatePickerDate(DepartureDatePicker);
            if (_arrivalDate >= _departureDate || _arrivalDate.Date < DateTime.Now)
            {
                ArrivalDatePicker.Background = Brushes.Red;
                DepartureDatePicker.Background = Brushes.Red;
                _wrongFilter = true;
            }
        }


        // Function to get the Date entered in a DatePicker (ArrivalDatePicker or DepartureDatePicker)
        // Returns a date
        private DateTime GetDatePickerDate(DatePicker datePicker)
        {
            DateTime date = DateTime.Today.AddDays(-1);
            if (datePicker.SelectedDate.HasValue)
            {
                date = datePicker.SelectedDate.Value.Date;
            }
            return date;
            //Kijken in de reserveringen lijst op die specifieke plek, en kijken of de gekozen tijdperiode nog niet bestaat
        }

        // Function (EventHandler) to apply the filters chosen after the "Pas filters toe" button is pressed
        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            SetPersonCountFromPersonCountTextBox();
            SetMaxPriceFromMaxPriceRangeTextBox();
            SetArrivalAndDepartureDates();
            _placesSortedAndOrFiltered = _camping.Places;
            Filter(_arrivalDate, _departureDate, PersonCount, _maxPriceRange, _hasPower);

        }

        // Function to filter the places List based on either or choice on arrival and departure date, amount of people possible on the place,
        // The max price a guest is willing to pay and if it has power or not 
        private void Filter(DateTime arrivalDate, DateTime departureDate, int personCount, double maxPriceRange, bool? hasPower)
        {

            if (!_wrongFilter)
            {
                GetFilteredListOnPrice(maxPriceRange);
                GetFilteredListOnPersonCount(personCount);
                GetFilteredListOnDate(arrivalDate, departureDate);
                GetFilteredListOnPower(hasPower);
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
            }

        }

        // Function (EventHandler) to remove all the filters and reset the filters to their default state.
        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            ArrivalDatePicker.SelectedDate = null;
            DepartureDatePicker.SelectedDate = null;
            PowerRadioButton3.IsChecked = true;
            PersonCount = 0;
            _maxPriceRange = _camping.Places.Max(i => i.PricePerNight);
            PersonCountTextBox.Text = $"{PersonCount}"; ;
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";
            PlacesListView.ItemsSource = _camping.Places;
            ResetBackgroundsFilters();
            _wrongFilter = false;
        }

        //Function to reset all the filters input fields to the standard background color
        private void ResetBackgroundsFilters()
        {
            ArrivalDatePicker.Background = Brushes.White;
            DepartureDatePicker.Background = Brushes.White;
            MaxPriceRangeTextBox.Background = Brushes.White;
            PersonCountTextBox.Background = Brushes.White;
        }


        // Function to sort the list on placenumbers 
        // Returns a bool to know which way the list is sorted now
        private bool SortColumnPlaceNumber(bool isSorted)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PlaceNumber).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PlaceNumber).Select(i => i);
            isSorted = !isSorted;
            return isSorted;
        }

        // Function to sort the list on price 
        // Returns a bool to know which way the list is sorted now
        private bool SortColumnPrice(bool isSorted)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PricePerNight).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PricePerNight).Select(i => i);
            isSorted = !isSorted;
            return isSorted;
        }

        // Function to sort the list on amount of possible people on a place 
        // Returns a bool to know which way the list is sorted now
        private bool SortColumnPersonCount(bool isSorted)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PersonCount).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PersonCount).Select(i => i);
            isSorted = !isSorted;
            return isSorted;
        }

        // Function (EventHandler) to sort the list of places based on the clicked column name and corresponding data
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

        // Function to filter the list of places on the bool hasPower
        private void GetFilteredListOnPower(bool? hasPower)
        {
            if (hasPower != null)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.HasPower == _hasPower).Select(i => i));
            }
        }

        // Function to filter the list of places on the integer maxPriceRange 
        private void GetFilteredListOnPrice(double maxPriceRange)
        {
            if (maxPriceRange >= _camping.Places.Min(i => i.PricePerNight))
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.PricePerNight <= _maxPriceRange).Select(i => i));
            }
        }

        // Function to filter the list of places on the int personCount
        private void GetFilteredListOnPersonCount(int personCount)
        {
            if (personCount >= 0)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.PersonCount >= PersonCount).Select(i => i));
            }
        }

        // Function to filter the list of places on the arrival and departure date
        private void GetFilteredListOnDate(DateTime arrivalDate, DateTime departureDate)
        {
            if (arrivalDate.Date < departureDate.Date && arrivalDate.Date >= DateTime.Now.Date)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(GetAvailablePlacesBetweenDates(arrivalDate.Date, departureDate.Date));
            }
        }

        // Function to get the places available between the arrival and departure date
        // Returns an IEnumerable<Place>
        private IEnumerable<Place> GetAvailablePlacesBetweenDates(DateTime arrivalDate, DateTime departureDate)
        {
            List<Place> availablePlacesBetweenDates = new List<Place>();
            foreach (Place place in _camping.Places)
            {
                int counter = 0;
                //All reservations of place
                var reservationsOnPlace = _camping.Reservations.Where(i => i.place.PlaceNumber == place.PlaceNumber).Select(i => i);
                if (reservationsOnPlace.Count() > 0)
                {
                    foreach (Reservation reservation in reservationsOnPlace)
                    {
                        if ((arrivalDate <= reservation.StartDatum.Date && reservation.StartDatum.Date <= departureDate) //StartDate of a reservation is between the arrival and departure date
                        || (arrivalDate <= reservation.EindDatum.Date && reservation.EindDatum.Date <= departureDate) //EndDate of a reservation is between the arrival and departure date
                        || (reservation.StartDatum.Date <= arrivalDate && reservation.EindDatum.Date >= departureDate)) // Arrival and departure is between the reservation dates
                        {
                            counter++;
                        }
                    }
                }
                //counter will be 0 if no reservations interfere with the arrival and departure date
                if (counter == 0)
                {
                    availablePlacesBetweenDates.Add(place);
                }
            }
            return availablePlacesBetweenDates;
        }
    }
}
