using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transform = CampingUI.NewFolder.Transform;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for PlacesOverviewPage.xaml
    /// </summary>
    public partial class PlacesOverviewPage : Page
    {

        private MapPage _mapPage;
        private Camping _camping;
        private IEnumerable<Place> _placesSortedAndOrFiltered;
        private bool? _hasPower = null, _dogsAllowed = null;
        private int _amountOfPeople = 0;
        private DateTime _arrivalDate, _departureDate;
        private bool _isSortedAscending = true;
        private double _maxPriceRange = 0;
        private bool _wrongInput = false;
        private string _headerTag;
        private bool _filterApplied = false;
        private bool _emptyDates = true;
        private PlacesOverviewPageFilter _placesOverviewPageFilter;
        private double desiredWidthMini = 2500;
        private double desiredHeightMini = 937.5;
        private double desiredWidthMain = 1000;
        private double desiredHeightMain = 750;

        public PlacesOverviewPage(Camping camping, CampingRepository campingRepository)
        {
            InitializeComponent();
            _placesOverviewPageFilter = new PlacesOverviewPageFilter();
            this._camping = camping;
            _camping.SetPlaces(_camping.CampingRepository.CampingPlaceRepository.GetPlaces());
            if (!_camping.GetPlaces().IsNullOrEmpty())
                _maxPriceRange = _camping.GetPlaces().Max(i => i.PricePerNightPerPerson);
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}"; 
            AmountOfPeopleTextBox.Text = $"{_amountOfPeople}";
            _placesSortedAndOrFiltered = _camping.GetPlaces();
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered; 
            this._headerTag = "PlaceID";
            new Transform(field2, desiredWidthMini, desiredHeightMini, "plattegrond");
            new Transform(field, desiredWidthMain, desiredHeightMain, "plattegrondMain");
            _mapPage = new MapPage(camping);
            _mapPage.MapMethods.GenerateMap(field2);
            _mapPage.MapMethods.GenerateMap(field);
            _mapPage.PlaceSelectedOnMap += HandlePlaceSelectedOnMap;
        }
        
        public void SetupMap()
        {
            _mapPage.MapMethods.GenerateMap(field2);
            _mapPage.MapMethods.GenerateMap(field);
          
        }
        public void HandlePlaceSelectedOnMap(Place place)
        {

            ResetFilters();
            PlacesListView.SelectedItem = place;
            DogCheckBoxFilter.IsChecked = place.Dogs;
            if (CheckBoxChecked(DogCheckBoxFilter, "hond") == true ) { DogCheckBoxFilter.Content = "Wel hond"; }
            if (CheckBoxChecked(DogCheckBoxFilter, "hond") == false) { DogCheckBoxFilter.Content = "Geen hond"; }

            PowerCheckBoxFilter.IsChecked = place.Power;
            if (CheckBoxChecked(PowerCheckBoxFilter, "stroom") == true) { PowerCheckBoxFilter.Content = "Wel stroom"; }
            if (CheckBoxChecked(PowerCheckBoxFilter, "stroom") == false) { PowerCheckBoxFilter.Content = "Geen stroom"; }
            

            AmountOfPeopleTextBox.Text = $"{place.AmountOfPeople}";
            MaxPriceRangeTextBox.Text = $"{place.PricePerNightPerPerson}";

        }
        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetTextboxBorder(textbox);
                _wrongInput = false;
            }
        }
        private void TextBox_PressedEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ResetListViewForFilter();
                SetFilterVariables();
                Filter(_arrivalDate, _departureDate, _amountOfPeople, _maxPriceRange, _hasPower, _dogsAllowed);
            }
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArrivalDatePicker.BorderBrush.Equals(Brushes.Red) && DepartureDatePicker.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetDatePickerBorder(ArrivalDatePicker);
                StaticUIMethods.ResetDatePickerBorder(DepartureDatePicker);
                _wrongInput = false;
            }
            ResetListViewForFilter();
            SetFilterVariables();
            ClosePlaceOverview();
            Filter(_arrivalDate, _departureDate, _amountOfPeople, _maxPriceRange, _hasPower, _dogsAllowed);
        }
        private void PowerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _hasPower = FilterAfterCheckboxChanged(PowerCheckBoxFilter, "stroom");
            Filter(_arrivalDate, _departureDate, _amountOfPeople, _maxPriceRange, _hasPower, _dogsAllowed);
        }
        private void DogsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _dogsAllowed = FilterAfterCheckboxChanged(DogCheckBoxFilter, "hond");
            Filter(_arrivalDate, _departureDate, _amountOfPeople, _maxPriceRange, _hasPower, _dogsAllowed);
        }
        private bool? FilterAfterCheckboxChanged(CheckBox checkbox, string inputstring)
        {
            bool? attributeboolean = CheckBoxChecked(checkbox, inputstring);
            ResetListViewForFilter();
            SetFilterVariables();
            return attributeboolean;
        }
        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            ResetFilters();
            ResetBackgroundsFilters();
            ClosePlaceOverview();
        }
        private void ResetFilters()
        {
            PlacesListView.SelectedItems.Clear();
            SetupMap();
            ArrivalDatePicker.SelectedDate = null;
            DepartureDatePicker.SelectedDate = null;
            DogCheckBoxFilter.Content = "Geen voorkeur (hond)";
            PowerCheckBoxFilter.Content = "Geen voorkeur (stroom)";
            DogCheckBoxFilter.IsChecked = null;
            PowerCheckBoxFilter.IsChecked = null;
            _hasPower = null;
            _dogsAllowed = null;
            _amountOfPeople = 0;
            if (!_camping.GetPlaces().IsNullOrEmpty())
            {
                _maxPriceRange = _camping.GetPlaces().Max(i => i.PricePerNightPerPerson);
                _placesSortedAndOrFiltered = _camping.GetPlaces();
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
            }
            else
            {
                _maxPriceRange = 0;
            }
            AmountOfPeopleTextBox.Text = $"{_amountOfPeople}"; ;
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";
            _wrongInput = false;
            _filterApplied = false;
        }
        private void ResetListViewForFilter()
        {
            PlacesListView.SelectedItems.Clear();
            ClosePlaceOverview();
            _placesSortedAndOrFiltered = _camping.GetPlaces();
        }
        private void Filter(DateTime arrivalDate, DateTime departureDate, int amountOfPeople, double maxPriceRange, bool? hasPower, bool? dogsAllowed)
        {

            if (!_wrongInput)
            {
                _placesSortedAndOrFiltered = PlacesOverviewFilter.GetFilteredListOnPrice(maxPriceRange, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewFilter.GetFilteredListOnDogs(dogsAllowed, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewFilter.GetFilteredListOnAmountOfPeople(amountOfPeople, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewFilter.GetFilteredListOnDate(_emptyDates, arrivalDate, departureDate, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewFilter.GetFilteredListOnPower(hasPower, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewSorting.SetSortDuringFiltering(_isSortedAscending, _headerTag, _placesSortedAndOrFiltered);
                HighLightFilteredMiniMap(_placesSortedAndOrFiltered);
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
                _filterApplied = true;
            }

        }
        private void HighLightFilteredMiniMap(IEnumerable<Place> filteredPlaces)
        {
            foreach (var comp in field.Children)
            {
                if (comp is Border placeBlock && placeBlock.Child is Canvas canvas && canvas.Name.Contains("Place"))
                {
                    if (filteredPlaces.Count() > 0) {
                        foreach (Place placeData in filteredPlaces)
                        {
                            if (canvas.Name.Equals("Place" + placeData.PlaceID.ToString()))
                            {

                                canvas.Opacity = 1.0;
                                break;
                            }

                            else
                            {
                                canvas.Opacity = 0.3;
                            }
                        }
                    }
                    else
                    {
                        canvas.Opacity = 0.3;
                    }
                }
            }
        }
        private bool? CheckBoxChecked(CheckBox checkbox, string content)
        {
            bool? editbool = null;
            if (checkbox.IsChecked == true)
            {
                checkbox.Content = "Wel " + content;
                editbool = true;
            }
            else if (checkbox.IsChecked == false)
            {

                checkbox.Content = "Geen " + content;
                editbool = false;
            }
            else
            {
                checkbox.Content = "Geen voorkeur (" + content + ")";
                editbool = null;

            }
            return editbool;
        }
        private void SetFilterVariables()
        {
            SetFilterVariableAmountOfPeople();
            SetFilterVariableDates();
            SetFilterVariableMaxPriceRange();
        }
        private void SetFilterVariableMaxPriceRange()
        {
            _placesOverviewPageFilter.SetMaxPriceFromMaxPriceRangeTextBox(MaxPriceRangeTextBox, _maxPriceRange, _wrongInput, _camping);
            _wrongInput = _placesOverviewPageFilter.WrongInput;
            _maxPriceRange = _placesOverviewPageFilter.MaxPriceRange;
        }
        private void SetFilterVariableDates()
        {
            _placesOverviewPageFilter.SetArrivalAndDepartureDates(ArrivalDatePicker, DepartureDatePicker, _wrongInput, _emptyDates);
            _wrongInput = _placesOverviewPageFilter.WrongInput;
            _arrivalDate = _placesOverviewPageFilter.ArrivalDate;
            _departureDate = _placesOverviewPageFilter.DepartureDate;
            _emptyDates = _placesOverviewPageFilter.EmptyDates;
        }
        private void SetFilterVariableAmountOfPeople()
        {
            _placesOverviewPageFilter.SetAmountOfPeopleFromAmountOfPeopleTextBox(AmountOfPeopleTextBox, _amountOfPeople, _wrongInput);
            _wrongInput = _placesOverviewPageFilter.WrongInput;
            _amountOfPeople = _placesOverviewPageFilter.AmountOfPeople;
        }
        private void ResetBackgroundsFilters()
        {
            StaticUIMethods.ResetDatePickerBorder(ArrivalDatePicker);
            StaticUIMethods.ResetDatePickerBorder(DepartureDatePicker);
            StaticUIMethods.ResetTextboxBorder(MaxPriceRangeTextBox);
            StaticUIMethods.ResetTextboxBorder(AmountOfPeopleTextBox);
        }
        private void SetSorterColumn_Click(object sender, RoutedEventArgs e)
        {
            if (!_camping.GetPlaces().IsNullOrEmpty())
            {
                GridViewColumnHeader gridViewColumnHeader = (GridViewColumnHeader)sender;
                _placesSortedAndOrFiltered = SortColumns(gridViewColumnHeader.Tag.ToString());
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
            }
        }
        private void NewReservation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReservationCreationPage(this));
        }
        private void DeletePlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            MessageBoxResult deleteMessageBox = MessageBox.Show("Weet je zeker dat de volgende plaats " + place.PlaceID + " verwijderd wordt?", "Waarschuwing!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deleteMessageBox == MessageBoxResult.Yes)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Where(i => i.PlaceID != place.PlaceID).ToList();
                PlacesOverviewDelete.DeletePlace(_camping, place, DateTime.Now.Date);
                _camping.SetPlaces(_camping.CampingRepository.CampingPlaceRepository.GetPlaces());
                ReloadScreenDataPlaces();
                ReloadMaps();
                ResetFilters();


            }
        }
        private void ReloadMaps()
        {
            _mapPage.MapMethods.GenerateMap(field);
            _mapPage.MapMethods.GenerateMap(field2);
        }
        private void OpenPlaceOverview()
        {
            PlaceOverviewGrid.Visibility = Visibility.Visible;
            BorderOverview.Visibility = Visibility.Visible;
        }
        private void ClosePlaceOverview()
        {
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            BorderOverview.Visibility = Visibility.Collapsed;

        }
        private IEnumerable<Place> SortColumns(string headerTag)
        {
            if (headerTag.Equals("PlaceID"))
                _placesSortedAndOrFiltered = PlacesOverviewSorting.SortColumnPlaceID(_isSortedAscending, _placesSortedAndOrFiltered);
            else if (headerTag.Equals("Price"))
                _placesSortedAndOrFiltered = PlacesOverviewSorting.SortColumnPrice(_isSortedAscending, _placesSortedAndOrFiltered);
            else
                _placesSortedAndOrFiltered = PlacesOverviewSorting.SortColumnAmountOfPeople(_isSortedAscending, _placesSortedAndOrFiltered);
            _isSortedAscending = !_isSortedAscending;
            _headerTag = headerTag;
            return _placesSortedAndOrFiltered;
        }
        private void ReloadScreenDataPlaces()
        {
            _placesSortedAndOrFiltered = _camping.GetPlaces();
            ClosePlaceOverview();
            PlacesListView.SelectedItems.Clear();
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
            Filter(_arrivalDate, _departureDate, _amountOfPeople, _maxPriceRange, _hasPower, _dogsAllowed);
        }
        private void SetDeleteButtonClickableIfNoReservations()
        {
            Place place = (Place)PlacesListView.SelectedItem;
            List<Reservation> placesReservations = _camping.GetReservations().Where(i => i.PlaceID == place.PlaceID)
                                                          .Where(i => i.DepartureDate >= DateTime.Now.Date).ToList();
            DeletePlaceButton.IsEnabled = true;
            if (placesReservations.Count > 0)
                DeletePlaceButton.IsEnabled = false;
        }
        private void PlacesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (PlacesListView.SelectedItems.Count > 0)
            {

                Place place = (Place)PlacesListView.SelectedItem;
                SetLabelsPlaceOverview(place);
                OpenPlaceOverview();
                SetReservationsInCalendar(place);
                SetDeleteButtonClickableIfNoReservations();
                ReloadMaps();
                foreach (var comp in field2.Children )
                {
                    if (comp is Border placeBlock && placeBlock.Child is Canvas canvas && canvas.Name.Contains("Place"))
                    {

                        if (canvas.Name.Equals("Place" + place.PlaceID.ToString()))
                        {
                            canvas.Background = Brushes.DarkCyan;
                        }
                    }
                }
                

            }
            else
            {
                PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            }
        }   
        private void SetReservationsInCalendar(Place place)
        {
            ReservationCalender.BlackoutDates.Clear();
            ReservationCalender.SelectedDate = null;
            List<Reservation> reservations = _camping.GetReservations().Where(r => r.PlaceID == place.PlaceID).ToList();
            reservations = reservations.Where(r => r.DepartureDate >= DateTime.Now).ToList();
            ReservationCalender.BlackoutDates.AddDatesInPast();
            foreach (Reservation reservation in reservations)
            {
                ReservationCalender.BlackoutDates.Add(new CalendarDateRange(reservation.ArrivalDate, reservation.DepartureDate));
            }
        }
        private void SetLabelsPlaceOverview(Place place)
        {
            nrLabel.Content = place;
            areaLabel.Content = "Oppervlakte: " + place.SurfaceArea;
            amountOfPeopleLabel.Content = "Aantal personen: " + place.AmountOfPeople;
            powerLabel.Content = "Toegang tot stroom: ";

            if (place.Power) powerLabel.Content += "Ja";
            else powerLabel.Content += "Nee";
            dogsLabel.Content = "Honden toegestaan: ";
            if (place.Dogs) dogsLabel.Content += "Ja";
            else dogsLabel.Content += "Nee";
            priceLabel.Content = "Prijs: " + String.Format("{0:0.00}", place.PricePerNightPerPerson) + "€";
        }
        public Camping GetCamping()
        {
            return _camping;
        }
        public int GetCampingAmountOfPeople()
        {
            return _amountOfPeople;
        }
        public bool GetFilterApplied()
        {
            return _filterApplied;
        }
    }
}
