﻿using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using Microsoft.IdentityModel.Tokens;
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
        public Camping Camping { get { return _camping; } }
        private IEnumerable<Place> _placesSortedAndOrFiltered;
        private bool? _hasPower;
        public int PersonCount = 0;
        private DateTime _arrivalDate, _departureDate;
        private bool _isSortedAscending = true;
        private double _maxPriceRange = 0;
        private bool _wrongFilter = false;
        private string _headerTag;
        public bool FilterAplied = false;
        private bool _emptyDates = true;

        public PlacesOverviewPage(Camping camping, SqliteRepository campingRepository)
        {
            InitializeComponent();
            this._camping = camping; // Creates a camping.
            if (!_camping.Places.IsNullOrEmpty())
                _maxPriceRange = _camping.Places.Max(i => i.PricePerNightPerPerson);
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}"; //Set the _maxPriceRange as a standard
            PersonCountTextBox.Text = $"{PersonCount}"; //Set the text in the textbox to 0
            _placesSortedAndOrFiltered = _camping.Places; //get all the places to the variable
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered; // For all items in the ListBox use the camping places.
            this._headerTag = "Placenumber";
            /*foreach (ListViewItem item in PlacesListView.Items)
            {
                if (item.TabIndex % 2 == 0)
                    item.Background = new SolidColorBrush(Color.FromRgb(21, 50, 96));
                else
                    item.Background = new SolidColorBrush(Color.FromRgb(29, 67, 129));
            }*/
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
            if (ArrivalDatePicker.Background.Equals(Brushes.Red) && DepartureDatePicker.Background.Equals(Brushes.Red))
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

        // Function to get the AmountOfPeople from the PersonCountTextBox text
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

        // Function to get the max Price range from the MaxPriceRangeTextBox text
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
                _maxPriceRange = _camping.Places.Max(i => i.PricePerNightPerPerson);
                MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";
            }
        }

        //Function to set the _arrivalDate and _departureDate
        private void SetArrivalAndDepartureDates()
        {
            _arrivalDate = GetDatePickerDate(ArrivalDatePicker);
            _departureDate = GetDatePickerDate(DepartureDatePicker);
            if (_arrivalDate.Equals(DateTime.MaxValue.Date) && _departureDate.Equals(DateTime.MinValue.Date))
            {
                _emptyDates = true;
            }
            else
            {
                _emptyDates = false;
                if (_arrivalDate >= _departureDate || _arrivalDate.Date < DateTime.Now.Date)
                {
                    ArrivalDatePicker.Background = Brushes.Red;
                    DepartureDatePicker.Background = Brushes.Red;
                    _wrongFilter = true;
                }
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
                if (tagValue == -1) date = DateTime.MinValue.Date;
                else date = DateTime.MaxValue.Date;
            }
            return date;
            //Kijken in de reserveringen lijst op die specifieke plek, en kijken of de gekozen tijdperiode nog niet bestaat
        }

        // Function (EventHandler) to apply the filters chosen after the "Pas filters toe" button is pressed
        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            PlacesListView.SelectedItems.Clear();
            SetPersonCountFromPersonCountTextBox();
            SetMaxPriceFromMaxPriceRangeTextBox();
            SetArrivalAndDepartureDates();
            _camping.Places = _camping.CampingRepository.GetPlaces();
            if (!_camping.Places.IsNullOrEmpty())
            {
                _placesSortedAndOrFiltered = _camping.Places;
                Filter(_arrivalDate, _departureDate, PersonCount, _maxPriceRange, _hasPower);
            }

        }

        // Function to filter the places List based on either or choice on arrival and departure date, amount of people possible on the _place,
        // The max Price a guest is willing to pay and if it has power or not 
        private void Filter(DateTime arrivalDate, DateTime departureDate, int personCount, double maxPriceRange, bool? hasPower)
        {

            if (!_wrongFilter)
            {
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPrice(maxPriceRange, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPersonCount(personCount, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnDate(_emptyDates, arrivalDate, departureDate, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPower(hasPower, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SetSortDuringFiltering(_isSortedAscending, _headerTag, _placesSortedAndOrFiltered);
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
                FilterAplied = true;
            }

        }

        // Function (EventHandler) to remove all the filters and reset the filters to their default state.
        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            ArrivalDatePicker.SelectedDate = null;
            DepartureDatePicker.SelectedDate = null;
            PowerRadioButton3.IsChecked = true;
            PersonCount = 0;
            _camping.Places = _camping.CampingRepository.GetPlaces();
            if (!_camping.Places.IsNullOrEmpty())
            {
                _maxPriceRange = _camping.Places.Max(i => i.PricePerNightPerPerson);
                _placesSortedAndOrFiltered = _camping.Places;
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
            }
            else
            {
                _maxPriceRange = 0;
            }
            PersonCountTextBox.Text = $"{PersonCount}"; ;
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";

            ResetBackgroundsFilters();
            _wrongFilter = false;
            FilterAplied = false;
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
            if (!_camping.Places.IsNullOrEmpty())
            {
                GridViewColumnHeader gridViewColumnHeader = (GridViewColumnHeader)sender;
                _placesSortedAndOrFiltered = SortColumns(gridViewColumnHeader.Tag.ToString());
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
            }
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


        private void NewReservation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReservationCreationPage(this));
        }

        //Function (EventHandler) that deletes a _place and its reservations from the camping
        private void DeletePlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            MessageBoxResult deleteMessageBox = MessageBox.Show("Weet je zeker dat de volgende plaats " + place.PlaceNumber + " verwijderd wordt?", "Waarschuwing!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deleteMessageBox == MessageBoxResult.Yes)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Where(i => i.PlaceID != place.PlaceID).ToList();
                PlacesOverviewPageDelete.DeletePlace(_camping, place, DateTime.Now.Date);
                ReloadScreenDataPlaces();
            }

        }
        private void ReloadScreenDataPlaces()
        {
            _camping.Places = _camping.CampingRepository.GetPlaces();
            _placesSortedAndOrFiltered = _camping.Places;
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            PlacesListView.SelectedItems.Clear();
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;

            if (!_placesSortedAndOrFiltered.IsNullOrEmpty() && _maxPriceRange > _placesSortedAndOrFiltered.Max(i => i.PricePerNightPerPerson))
                _maxPriceRange = _placesSortedAndOrFiltered.Max(i => i.PricePerNightPerPerson);
            else
                _maxPriceRange = 0;
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";
        }

        private void SetDeleteButtonClickableIfNoReservations()
        {
            Place place = (Place)PlacesListView.SelectedItem;
            var placesReservations = _camping.Reservations.Where(i => i.PlaceID == place.PlaceID)
                                                          .Where(i => i.DepartureDate >= DateTime.Now.Date).ToList();
            if (placesReservations.Count > 0)
            {
                DeletePlaceButton.IsEnabled = false;
            }

            else
            {
                DeletePlaceButton.IsEnabled = true;
            }
        }

        // Is used everytime a different _place is selected in the _place list
        private void PlacesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlacesListView.SelectedItems.Count > 0)
            {
                AddPlaceGrid.Visibility = Visibility.Collapsed;
                Place place = (Place)PlacesListView.SelectedItem;
                nrLabel.Content = place;
                areaLabel.Content = "Oppervlakte: " + place.SurfaceArea;
                nrPeopleLabel.Content = "Aantal personnen:" + place.AmountOfPeople;
                electricityLabel.Content = "Toegang tot stroom: ";

                if (place.Power) electricityLabel.Content += "Ja";
                else electricityLabel.Content += "Nee";
                priceLabel.Content = "Prijs: " + String.Format("{0:0.00}", place.PricePerNightPerPerson) + "$";

                PlaceOverviewGrid.Visibility = Visibility.Visible;
                ReservationCalender.BlackoutDates.Clear();

                ReservationCalender.SelectedDate = null;
                _camping.Reservations = _camping.CampingRepository.GetReservations();
                var reservations = _camping.Reservations.Where(r => r.PlaceID == place.PlaceID).ToList();
                reservations = reservations.Where(r => r.DepartureDate >= DateTime.Now).ToList();
                ReservationCalender.BlackoutDates.AddDatesInPast();
                foreach (var reservation in reservations)
                {
                    ReservationCalender.BlackoutDates.Add(new CalendarDateRange(reservation.ArrivalDate, reservation.DepartureDate));
                }
                SetDeleteButtonClickableIfNoReservations();
            }
            else
            {
                PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void AddPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            AddPlaceGrid.Visibility = Visibility.Visible;

        }

        //Function (EventHandler) To add a place when the add place button is clicked
        public void AddPlaceOnClick(object sender, RoutedEventArgs e)
        {

            string[] TextInput =
            {
                PlaceNumber.Text,
                SurfaceArea.Text,
                PricePerPersonPerNight.Text,
                NumberOfPeople.Text,
                HasPower.SelectionBoxItem.ToString()
            };

            //Checks if the required textboxes are filled
            if (CheckIfInputIsNotNull(TextInput))
            {
                AddPlaceToDatabase();
            }
        }

        //Function to initiate the addPlace function from the database and show/hide the correct screen
        public void AddPlaceToDatabase()
        {
            try
            {
                //Parses the string inputs from textboxes to ints
                int placeNumber = Int32.Parse(PlaceNumber.Text);
                int surfaceArea = Int32.Parse(SurfaceArea.Text);
                int pricePerPersonPerNight = Int32.Parse(PricePerPersonPerNight.Text);
                int amountOfPeople = Int32.Parse(NumberOfPeople.Text);
                string electricity = HasPower.SelectionBoxItem.ToString();
                string placeDescription = PlaceDescription.Text;
                //Checks if the place has electricity 
                bool hasPower;
                if (electricity.Equals("Ja"))
                {
                    hasPower = true;
                }
                else
                {
                    hasPower = false;
                }

                //Make a new place with the input of the textboxes
                // -------------------------------------------------- MOET AANGEPAST WORDEN-----------------------------------------------------
                Place place = new Place(placeNumber, hasElectricity, 1, true, surfaceArea,amountOfPeople, pricePerPersonPerNight, 1, 1);

                //Database db = new Database();
                //db.AddPlaceToDatabase(place);
                _camping.CampingRepository.AddPlace(place);


                //Clears textboxes when the data is inserted in the database
                foreach (var textbox in AddPlaceGrid.Children)
                {
                    if (textbox is TextBox textBox)
                    {
                        textBox.Text = string.Empty;
                    }
                }


                //misschien aanpassen zodat deze niet blijft staan
                AddPlaceMessage.Text = "Nieuwe plaats is toegevoegd";
                AddPlaceMessage.Foreground = Brushes.Green;
                _camping.Places = _camping.CampingRepository.GetPlaces();
                PlacesListView.ItemsSource = _camping.Places;

            }
            catch (Exception ex)
            {
                AddPlaceMessage.Text = "Ongeldigde input";
                AddPlaceMessage.Foreground = Brushes.Red;
            }

        }

        private void EditPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            AddPlaceGrid.Visibility = Visibility.Visible;
            AddPlaceButton.Visibility = Visibility.Collapsed;
            ConfirmEditButton.Visibility = Visibility.Visible;
            AddPlaceDataToTextfields(place);

        }

        private void AddPlaceDataToTextfields(Place place)
        {
            PlaceNumber.Text = place.PlaceNumber.ToString();
            PlaceNumber.IsEnabled = false;
            SurfaceArea.Text = place.SurfaceArea.ToString();
            if (place.HasPower)
                HasPower.SelectedItem = "Ja";
            else
                HasPower.SelectedItem = "Nee";
            NumberOfPeople.Text = place.PersonCount.ToString();
            PricePerPersonPerNight.Text = place.PricePerNight.ToString();
            PlaceDescription.Text = place.Description.ToString();
        }

        private Place GetPlaceFromTextBoxes()
        {
            try
            {
                //Parses the string inputs from textboxes to ints
                int placeNumber = Int32.Parse(PlaceNumber.Text);
                int surfaceArea = Int32.Parse(SurfaceArea.Text);
                double pricePerPersonPerNight = double.Parse(PricePerPersonPerNight.Text);
                int personCount = Int32.Parse(NumberOfPeople.Text);
                string electricity = HasPower.SelectionBoxItem.ToString();
                string placeDescription = PlaceDescription.Text;
                //Checks if the place has electricity 
                bool hasPower;
                if (electricity.Equals("Ja"))
                    hasPower = true;
                else
                    hasPower = false;
                return new Place(placeNumber, hasPower, surfaceArea, personCount, pricePerPersonPerNight, placeDescription);
            }
            catch
            {
                AddPlaceMessage.Text = "Ongeldigde input";
                AddPlaceMessage.Foreground = Brushes.Red;
                return null;
            }
        }

        private void EditPlaceConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = null;
            while(place == null)
            {
                place = GetPlaceFromTextBoxes();
            }
            _camping.CampingRepository.UpdatePlaceData(place);
            AddPlaceButton.Visibility = Visibility.Visible;
            ConfirmEditButton.Visibility = Visibility.Collapsed;
            AddPlaceGrid.Visibility = Visibility.Collapsed;
            ReloadScreenDataPlaces();
            ResetDataFromTextfields();
        }

        private void ResetDataFromTextfields()
        {
            PlaceNumber.Text = null;
            PlaceNumber.IsEnabled = true;
            SurfaceArea.Text = null;
            HasPower.Text = null;
            NumberOfPeople.Text = null;
            PricePerPersonPerNight.Text = null;
            PlaceDescription.Text = null;
        }

        //Function to check if the input is null or not from a text array
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
