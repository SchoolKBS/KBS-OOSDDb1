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
using System.Windows.Shapes;
using Transform = CampingUI.NewFolder.Transform;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for PlacesOverviewPage.xaml
    /// </summary>
    public partial class PlacesOverviewPage : Page
    {

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
        private bool _hasPowerEdit, _dogsAllowedEdit;
        private int _surfaceAreaEdit;
        private double _pricePerNightPerPersonEdit;
        private int _amountOfPeopleEdit;
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
            _camping.Places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            if (!_camping.Places.IsNullOrEmpty())
                _maxPriceRange = _camping.Places.Max(i => i.PricePerNightPerPerson);
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}"; 
            AmountOfPeopleTextBox.Text = $"{_amountOfPeople}"; 
            _placesSortedAndOrFiltered = _camping.Places;
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered; 
            this._headerTag = "PlaceID";
            new Transform(field2, desiredWidthMini, desiredHeightMini, "plattegrond");
            new Transform(field, desiredWidthMain, desiredHeightMain, "plattegrondMain");
            MapPage mapPage = new MapPage(camping);
            mapPage.GenerateMap(field2);
            mapPage.GenerateMap(field);
            mapPage.PlaceSelectedOnMap += HandlePlaceSelectedOnMap;

        }

        public void HandlePlaceSelectedOnMap(Place place)
        {

            ResetFilters();
            PlacesListView.SelectedItem = place;
            DogCheckBoxFilter.IsChecked = place.Dogs;
            if (CheckBoxChecked(DogCheckBoxFilter, "hond") == true ) { DogCheckBoxFilter.Content = "Wel hond"; }
            if (CheckBoxChecked(DogCheckBoxFilter, "hond") == false) { DogCheckBoxFilter.Content = "Geen hond"; }

            PowerCheckBoxFilter.IsChecked = place.Power;
            if (CheckBoxChecked(PowerCheckBoxFilter, "stroom") == true) { DogCheckBoxFilter.Content = "Wel stroom"; }
            if (CheckBoxChecked(PowerCheckBoxFilter, "stroom") == false) { DogCheckBoxFilter.Content = "Geen stroom"; }


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
            CloseEditPlacesAndPlaceOverview();
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
            SetFilterVariables();
            ResetListViewForFilter();
            return attributeboolean;
        }
        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            ResetFilters();
            ResetBackgroundsFilters();
            CloseEditPlacesAndPlaceOverview();
        }
        private void ResetFilters()
        {
            ArrivalDatePicker.SelectedDate = null;
            DepartureDatePicker.SelectedDate = null;
            DogCheckBoxFilter.Content = "Geen voorkeur (hond)";
            PowerCheckBoxFilter.Content = "Geen voorkeur (stroom)";
            DogCheckBoxFilter.IsChecked = null;
            PowerCheckBoxFilter.IsChecked = null;
            _hasPower = null;
            _dogsAllowed = null;
            _amountOfPeople = 0;
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
            AmountOfPeopleTextBox.Text = $"{_amountOfPeople}"; ;
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";
            _wrongInput = false;
            _filterApplied = false;
        }
        private void ResetListViewForFilter()
        {
            PlacesListView.SelectedItems.Clear();
            CloseEditPlacesAndPlaceOverview();
            _placesSortedAndOrFiltered = _camping.Places;
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
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
                _filterApplied = true;
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
            if (!_camping.Places.IsNullOrEmpty())
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
                ReloadScreenDataPlaces();
            }

        }
        private void OpenPlaceOverview()
        {
            EditPlaceGrid.Visibility = Visibility.Collapsed;
            PlaceOverviewGrid.Visibility = Visibility.Visible;
            BorderOverview.Visibility = Visibility.Visible;
        }
        private void OpenEditPlace()
        {
            EditPlaceGrid.Visibility = Visibility.Visible;
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
        }
        private void CloseEditPlacesAndPlaceOverview()
        {
            EditPlaceGrid.Visibility = Visibility.Collapsed;
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
            _camping.Places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            _placesSortedAndOrFiltered = _camping.Places;
            CloseEditPlacesAndPlaceOverview();
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
            List<Reservation> placesReservations = _camping.Reservations.Where(i => i.PlaceID == place.PlaceID)
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
            _camping.Reservations = _camping.CampingRepository.CampingReservationRepository.GetReservations();
            List<Reservation> reservations = _camping.Reservations.Where(r => r.PlaceID == place.PlaceID).ToList();
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
        private void EditPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            OpenEditPlace();
            setDataFromPlaceOnFieldsEdit(place);
        }
        private void EditExtendButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            Button button = (Button)sender;
            Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(place);
            if (button.Name.Equals(AmountOfPeopleExtendButton.Name))
                AmountOfPeopleEditTextBox.Text = street.AmountOfPeople.ToString();
            else if (button.Name.Equals(PricePerNightPerPersonExtendButton.Name))
                PricePerNightPerPersonEditTextBox.Text = street.PricePerNightPerPerson.ToString();
            else
                SurfaceAreaEditTextBox.Text = street.SurfaceArea.ToString();
        }
        private void setDataFromPlaceOnFieldsEdit(Place place)
        {
            PlaceIDLabelEdit.Content = "Plaats " + place.PlaceID;
            AmountOfPeopleEditTextBox.Text = place.AmountOfPeople.ToString();
            SurfaceAreaEditTextBox.Text = place.SurfaceArea.ToString();
            PricePerNightPerPersonEditTextBox.Text = place.PricePerNightPerPerson.ToString();
            PowerEditCheckBox.IsChecked = null;
            if (place.Power)
                PowerEditCheckBox.IsChecked = true;
            DogsEditCheckBox.IsChecked = null;
            if (place.Dogs)
                DogsEditCheckBox.IsChecked = true;
            GetEditedCheckBox(DogsEditCheckBox, "hond", _dogsAllowedEdit);
            GetEditedCheckBox(PowerEditCheckBox, "stroom", _hasPowerEdit);
        }
        private void EditPlaceConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            GetEditedNumbers();
            _hasPowerEdit = GetEditedCheckBox(PowerEditCheckBox, "stroom", _hasPowerEdit);
            _dogsAllowedEdit = GetEditedCheckBox(DogsEditCheckBox, "hond", _dogsAllowedEdit);
            Place place = (Place)PlacesListView.SelectedItem;
            if (!_wrongInput)
            {
                _camping.UpdatePlace(place.PlaceID, place.StreetID, place.AreaID, _hasPowerEdit, _surfaceAreaEdit, _pricePerNightPerPersonEdit, _amountOfPeopleEdit, _dogsAllowedEdit);
                EditPlaceGrid.Visibility = Visibility.Collapsed;
                PlaceOverviewGrid.Visibility = Visibility.Visible;
                ReloadScreenDataPlaces();
            }

        }
        private void PowerEditCheckBox_Click(object sender, RoutedEventArgs e)
        {
            _hasPowerEdit = GetEditedCheckBox((CheckBox)sender, "stroom", _hasPowerEdit);
        }
        private void DogsEditCheckBox_Click(object sender, RoutedEventArgs e)
        {
            _dogsAllowedEdit = GetEditedCheckBox((CheckBox)sender, "hond", _dogsAllowedEdit);
        }
        private bool GetEditedCheckBox(CheckBox checkBox, string content, bool editBool)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(place);
            if (checkBox.IsChecked == true)
            {
                editBool = true;
                checkBox.Content = "Wel " + content;
            }
            else if (checkBox.IsChecked == false)
            {
                editBool = false;
                checkBox.Content = "Geen " + content;
            }
            else
            {
                if (checkBox.Equals(DogsEditCheckBox))
                    editBool = street.Dogs;
                else
                    editBool = street.Power;
                checkBox.Content = "Overerven van straat (" + content + ")";
            }
            return editBool;
        }
        private void GetEditedNumbers()
        {
            GetEditedAmountOfPeople();
            GetEditedPricePerNightPerPerson();
            GetEditedSurfaceArea();
        }
        private int GetEditedTextBox(TextBox textbox, int editNumber)
        {
            int number;
            if (int.TryParse(textbox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(textbox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                editNumber = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                _wrongInput = true;
            }
            return editNumber;
        }
        private void GetEditedAmountOfPeople()
        {
            _amountOfPeopleEdit = GetEditedTextBox(AmountOfPeopleEditTextBox, _amountOfPeopleEdit);
        }
        private void GetEditedSurfaceArea()
        {
            _surfaceAreaEdit = GetEditedTextBox(SurfaceAreaEditTextBox, _surfaceAreaEdit);
        }
        private void GetEditedPricePerNightPerPerson()
        {
            double number;
            if (double.TryParse(PricePerNightPerPersonEditTextBox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(PricePerNightPerPersonEditTextBox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                _pricePerNightPerPersonEdit = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(PricePerNightPerPersonEditTextBox);
                _wrongInput = true;
            }
        }
        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {

            OpenPlaceOverview();
            _wrongInput = false;
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
