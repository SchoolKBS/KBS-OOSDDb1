using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
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
        private Camping _camping;
        private IEnumerable<Place> _placesSortedAndOrFiltered;
        private bool? _hasPower;
        public int PersonCount = 0;
        private DateTime _arrivalDate, _departureDate;
        private bool _isSortedAscending = true;
        private double _maxPriceRange;
        private bool _wrongFilter = false;
        private string _headerTag;

        public PlacesOverviewPage(Camping camping, CampingRepository campingRepository)
        {
            InitializeComponent();
            this._camping = camping; // Creates a camping.
            _maxPriceRange = _camping.Places.Max(i => i.PricePerNight);
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}"; //Set the _maxPriceRange as a standard
            PersonCountTextBox.Text = $"{PersonCount}"; //Set the text in the textbox to 0
            _placesSortedAndOrFiltered = _camping.Places; //get all the places to the variable
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;   // For all items in the ListBox use the camping places.
            this._headerTag = "Placenumber";
        }

        //Function (EventHandler) that resets the background of a textbox if the filters are reset
        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.Background.Equals(Brushes.Red))
            {
                textbox.Background = Brushes.White;
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
            if (_arrivalDate >= _departureDate || _arrivalDate.Date < DateTime.Now.Date)
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
            DateTime date;
            if (datePicker.SelectedDate.HasValue)
            {
                date = datePicker.SelectedDate.Value.Date;
            }
            else
            {
                int tagValue = int.Parse(datePicker.Tag.ToString());
                date = DateTime.MaxValue.AddDays(tagValue);
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
            _camping.Places = _camping.CampingRepository.GetPlaces();
            _placesSortedAndOrFiltered = _camping.Places;
            Filter(_arrivalDate, _departureDate, PersonCount, _maxPriceRange, _hasPower);

        }

        // Function to filter the places List based on either or choice on arrival and departure date, amount of people possible on the place,
        // The max price a guest is willing to pay and if it has power or not 
        private void Filter(DateTime arrivalDate, DateTime departureDate, int personCount, double maxPriceRange, bool? hasPower)
        {

            if (!_wrongFilter)
            {
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPrice(maxPriceRange, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPersonCount(personCount, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnDate(arrivalDate, departureDate, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPower(hasPower, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SetSortDuringFiltering(_isSortedAscending, _headerTag, _placesSortedAndOrFiltered);
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
            _camping.Places = _camping.CampingRepository.GetPlaces();
            _placesSortedAndOrFiltered = _camping.Places;
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
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

        // Function (EventHandler) to sort the list of places based on the clicked column name and corresponding data
        private void SetSorterColumn_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader gridViewColumnHeader = (GridViewColumnHeader)sender;
            _placesSortedAndOrFiltered = SortColumns(gridViewColumnHeader.Tag.ToString());
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
        }

        private IEnumerable<Place> SortColumns(string headerTag)
        {
            if (headerTag.Equals("Placenumber"))
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SortColumnPlaceNumber(_isSortedAscending, _placesSortedAndOrFiltered);
            else if (headerTag.Equals("Price"))
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SortColumnPrice(_isSortedAscending, _placesSortedAndOrFiltered);
            else
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SortColumnPersonCount(_isSortedAscending, _placesSortedAndOrFiltered);
            _isSortedAscending = !_isSortedAscending;
            _headerTag = headerTag;
            return _placesSortedAndOrFiltered;
        }

        private void DeletePlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            MessageBoxResult deleteMessageBox = MessageBox.Show("Weet je zeker dat de volgende plaats " + place.PlaceNumber + " verwijderd wordt?", "Waarschuwing!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deleteMessageBox == MessageBoxResult.Yes)
            {
                PlacesOverviewPageDelete.DeletePlace(_camping, place);
                PlaceOverviewGrid.Visibility = Visibility.Collapsed;
                PlacesListView.SelectedItems.Clear();
                
            }
        }


        // Is used everytime a different place is selected in the place list
        private void PlacesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PlacesListView.SelectedItems.Count > 0)
            {
                Place place = (Place) PlacesListView.SelectedItem;
                nrLabel.Content = place;
                areaLabel.Content = "Oppervlakte: " + place.SurfaceArea;
                nrPeopleLabel.Content = "Aantal personnen:" + place.PersonCount;
                electricityLabel.Content = "Toegang tot stroom: ";

                if (place.HasPower) electricityLabel.Content += "Ja";
                else electricityLabel.Content += "Nee";
                priceLabel.Content = "Prijs: " + String.Format("{0:0.00}", place.PricePerNight) + "$";
                descriptionLabel.Content = "Beschrijving: " + place.Description;

                PlaceOverviewGrid.Visibility = Visibility.Visible;
                ReservationCalender.BlackoutDates.Clear();

                ReservationCalender.SelectedDate = null;
                var reservations = _camping.Reservations.Where(r => r.PlaceID == place.PlaceNumber).ToList();
                reservations = reservations.Where(r => r.DepartureDate >= DateTime.Now).ToList();
                ReservationCalender.BlackoutDates.AddDatesInPast();
                foreach ( var reservation in reservations )
                {
                    ReservationCalender.BlackoutDates.Add(new CalendarDateRange(reservation.ArrivalDate, reservation.DepartureDate));
                }
            }
            else
            {
                PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
