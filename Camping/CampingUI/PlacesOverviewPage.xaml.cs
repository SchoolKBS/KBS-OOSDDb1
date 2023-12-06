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
        private bool? _dogsAllowed;
        public int AmountOfPeople = 0;
        private DateTime _arrivalDate, _departureDate;
        private bool _isSortedAscending = true;
        private double _maxPriceRange = 0;
        private bool _wrongInput = false;
        private string _headerTag;
        public bool FilterApplied = false;
        private bool _emptyDates = true;
        private bool _hasPowerEdit;
        private bool _dogsAllowedEdit;
        private int _surfaceAreaEdit;
        private double _pricePerNightPerPersonEdit;
        private int _amountOfPeopleEdit;

        public PlacesOverviewPage(Camping camping, CampingRepository campingRepository)
        {
            InitializeComponent();
            this._camping = camping; // Creates a camping.
            if (!_camping.Places.IsNullOrEmpty())
                _maxPriceRange = _camping.Places.Max(i => i.PricePerNightPerPerson);
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}"; //Set the _maxPriceRange as a standard
            AmountOfPeopleTextBox.Text = $"{AmountOfPeople}"; //Set the text in the textbox to 0
            _placesSortedAndOrFiltered = _camping.Places; //get all the places to the variable
            PlacesListView.ItemsSource = _placesSortedAndOrFiltered; // For all items in the ListBox use the camping places.
            this._headerTag = "PlaceID";
        }

        //Function (EventHandler) that resets the background of a textbox if the filters are reset
        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.BorderBrush.Equals(Brushes.Red))
            {
                textbox.BorderThickness = new Thickness(1, 1, 1, 1);
                textbox.BorderBrush = Brushes.White;
                _wrongInput = false;
            }
        }

        //Function (EventHandler) to reset the datepickers to backgroundcolor white incase they were red before
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArrivalDatePicker.BorderBrush.Equals(Brushes.Red) && DepartureDatePicker.BorderBrush.Equals(Brushes.Red))
            {
                ArrivalDatePicker.BorderThickness = new Thickness(1,1,1,1);
                ArrivalDatePicker.BorderBrush = Brushes.White;
                DepartureDatePicker.BorderThickness = new Thickness(1,1,1,1);
                DepartureDatePicker.BorderBrush = Brushes.White;
                _wrongInput = false;
            }
        }

        // Function to get the AmountOfPeople from the AmountOfPeopleTextBox text
        private void SetAmountOfPeopleFromAmountOfPeopleTextBox()
        {
            int number;
            if (!string.IsNullOrEmpty(AmountOfPeopleTextBox.Text))
            {
                if (int.TryParse(AmountOfPeopleTextBox.Text, out number) && number >= 0)// Checks if int can be parsed and if number is bigger or equal to 0
                {
                    AmountOfPeople = number;
                }
                else
                {
                    AmountOfPeopleTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
                    AmountOfPeopleTextBox.BorderBrush = Brushes.Red;
                    _wrongInput = true;
                }

            }
            else
            {
                AmountOfPeople = 0;
                AmountOfPeopleTextBox.Text = $"{AmountOfPeople}";
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
                    MaxPriceRangeTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
                    MaxPriceRangeTextBox.BorderBrush = Brushes.Red;
                    _wrongInput = true;
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
                    ArrivalDatePicker.BorderThickness = new Thickness(3, 3, 3, 3);
                    ArrivalDatePicker.BorderBrush = Brushes.Red;
                    DepartureDatePicker.BorderThickness = new Thickness(3, 3, 3, 3);
                    DepartureDatePicker.BorderBrush = Brushes.Red;
                    _wrongInput = true;
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
            SetAmountOfPeopleFromAmountOfPeopleTextBox();
            SetMaxPriceFromMaxPriceRangeTextBox();
            SetArrivalAndDepartureDates();
            EditPlaceGrid.Visibility = Visibility.Collapsed;
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            _camping.Places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            if (!_camping.Places.IsNullOrEmpty())
            {
                _placesSortedAndOrFiltered = _camping.Places;
                Filter(_arrivalDate, _departureDate, AmountOfPeople, _maxPriceRange, _hasPower);
            }

        }

        // Function to filter the places List based on either or choice on arrival and departure date, amount of people possible on the _place,
        // The max Price a guest is willing to pay and if it has power or not 
        private void Filter(DateTime arrivalDate, DateTime departureDate, int amountOfPeople, double maxPriceRange, bool? hasPower)
        {

            if (!_wrongInput)
            {
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPrice(maxPriceRange, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnDogs(_dogsAllowed, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnAmountOfPeople(amountOfPeople, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnDate(_emptyDates, arrivalDate, departureDate, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageFilter.GetFilteredListOnPower(hasPower, _placesSortedAndOrFiltered, _camping);
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SetSortDuringFiltering(_isSortedAscending, _headerTag, _placesSortedAndOrFiltered);
                PlacesListView.ItemsSource = _placesSortedAndOrFiltered;
                FilterApplied = true;
            }

        }

        // Function (EventHandler) to remove all the filters and reset the filters to their default state.
        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            ArrivalDatePicker.SelectedDate = null;
            DepartureDatePicker.SelectedDate = null;
            DogCheckBoxFilter.IsChecked = false;
            DogCheckBoxFilter.Content = "Geen voorkeur (hond)";
            PowerCheckBoxFilter.Content = "Geen voorkeur (stroom)";
            PowerCheckBoxFilter.IsChecked = false;
            AmountOfPeople = 0;
            _camping.Places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            EditPlaceGrid.Visibility = Visibility.Collapsed;
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
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
            AmountOfPeopleTextBox.Text = $"{AmountOfPeople}"; ;
            MaxPriceRangeTextBox.Text = $"{_maxPriceRange}";

            ResetBackgroundsFilters();
            _wrongInput = false;
            FilterApplied = false;
        }

        //Function to reset all the filters input fields to the standard background color
        private void ResetBackgroundsFilters()
        {
            ArrivalDatePicker.Background = Brushes.White;
            DepartureDatePicker.Background = Brushes.White;
            MaxPriceRangeTextBox.Background = Brushes.White;
            AmountOfPeopleTextBox.Background = Brushes.White;
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
            if (headerTag.Equals("PlaceID"))
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SortColumnPlaceID(_isSortedAscending, _placesSortedAndOrFiltered);
            else if (headerTag.Equals("Price"))
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SortColumnPrice(_isSortedAscending, _placesSortedAndOrFiltered);
            else
                _placesSortedAndOrFiltered = PlacesOverviewPageSorting.SortColumnAmountOfPeople(_isSortedAscending, _placesSortedAndOrFiltered);
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
            MessageBoxResult deleteMessageBox = MessageBox.Show("Weet je zeker dat de volgende plaats " + place.PlaceID + " verwijderd wordt?", "Waarschuwing!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deleteMessageBox == MessageBoxResult.Yes)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Where(i => i.PlaceID != place.PlaceID).ToList();
                PlacesOverviewPageDelete.DeletePlace(_camping, place, DateTime.Now.Date);
                ReloadScreenDataPlaces();
            }

        }
        private void ReloadScreenDataPlaces()
        {
            _camping.Places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
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
            List<Reservation> placesReservations = _camping.Reservations.Where(i => i.PlaceID == place.PlaceID)
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
                EditPlaceGrid.Visibility = Visibility.Collapsed;
                Place place = (Place)PlacesListView.SelectedItem;
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
                

                PlaceOverviewGrid.Visibility = Visibility.Visible;
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
            EditPlaceGrid.Visibility = Visibility.Visible;

        }
        //Function (EventHandler) To add a place when the add place button is clicked
        /* public void AddPlaceOnClick(object sender, RoutedEventArgs e)
        {

            string[] TextInput =
            {
                PlaceID.Text,
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
        }*/

        //Function to initiate the addPlace function from the database and show/hide the correct screen
        /*public void AddPlaceToDatabase()
        {
            try
            {
                //Parses the string inputs from textboxes to ints
                int placeID = Int32.Parse(PlaceID.Text);
                int surfaceArea = Int32.Parse(SurfaceArea.Text);
                int pricePerPersonPerNight = Int32.Parse(PricePerPersonPerNight.Text);
                int amountOfPeople = Int32.Parse(NumberOfPeople.Text);
                string electricity = HasPower.SelectionBoxItem.ToString();
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
                Place place = new Place(placeID, hasPower, 1, true, surfaceArea,amountOfPeople, pricePerPersonPerNight, 1, 1);

                _camping.CampingRepository.AddPlace(place);


                //Clears textboxes when the data is inserted in the database
                foreach (var textbox in AddPlaceGrid.Children)
                {
                    if (textbox is TextBox textBox)
                    {
                        textBox.Text = string.Empty;
                    }
                }

                //AddPlaceMessage.Text = "Nieuwe plaats is toegevoegd";
                //AddPlaceMessage.Foreground = Brushes.Green;
                _camping.Places = _camping.CampingRepository.GetPlaces();
                PlacesListView.ItemsSource = _camping.Places;

            }
            catch (Exception e)
            {
                AddPlaceMessage.Text = "Ongeldigde input";
                AddPlaceMessage.Foreground = Brushes.Red;
            }

        }*/

        private void EditPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            PlaceOverviewGrid.Visibility = Visibility.Collapsed;
            EditPlaceGrid.Visibility = Visibility.Visible;
            setDataFromPlaceOnFieldsEdit(place);

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
            ChangeDogsLabelOnCheckBoxClick();
            ChangePowerLabelOnCheckBoxClick();

        }
        private void EditPlaceConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            GetEditedNumbers();
            _hasPowerEdit = GetEditedCheckBox(PowerEditCheckBox, _hasPowerEdit);

            _dogsAllowedEdit = GetEditedCheckBox(DogsEditCheckBox, _dogsAllowedEdit);
            Place place = (Place)PlacesListView.SelectedItem;
            if (!_wrongInput)
            {
                //Test
                Camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(place, _hasPowerEdit, _surfaceAreaEdit, _pricePerNightPerPersonEdit, _amountOfPeopleEdit, _dogsAllowedEdit);
                //Test
                EditPlaceGrid.Visibility = Visibility.Collapsed;
                PlaceOverviewGrid.Visibility = Visibility.Visible;
                ReloadScreenDataPlaces();
            }

        }

        private bool GetEditedCheckBox(CheckBox checkBox, bool editBool)
        {
            Place place = (Place)PlacesListView.SelectedItem;
            Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(place);
            if (checkBox.IsChecked == true) editBool = true;
            else if (checkBox.IsChecked == null) editBool = false;
            else
            {
                if (checkBox.Equals(DogsEditCheckBox))
                    editBool = street.Dogs;
                else
                    editBool = street.Power;
            }
            return editBool;
        }

        private void ChangeLabelOnCheckBoxClick(CheckBox checkbox, Label label, string truestring, string falsestring)
        {
            if (checkbox.IsChecked == true) label.Content = truestring;
            else if (checkbox.IsChecked == false) label.Content = "Overnemen van straat eigenschappen";
            else label.Content = falsestring;
        }
        private void ChangeDogsLabelOnCheckBoxClick()
        {
            ChangeLabelOnCheckBoxClick(DogsEditCheckBox, DogsAllowedEditLabel, "Honden toegestaan", "Geen honden toegestaan");
        }
        private void ChangePowerLabelOnCheckBoxClick()
        {
            ChangeLabelOnCheckBoxClick(PowerEditCheckBox, PowerEditLabel, "Wel stroom", "Geen stroom");
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            
            EditPlaceGrid.Visibility = Visibility.Collapsed;
            PlaceOverviewGrid.Visibility = Visibility.Visible;
            _wrongInput = false;
        }

        private void DogsEditCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ChangeDogsLabelOnCheckBoxClick();
        }
        private void PowerEditCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ChangePowerLabelOnCheckBoxClick();
        }

        private void GetEditedNumbers()
        {
            GetEditedAmountOfPeople();
            GetEditedPricePerNightPerPerson();
            GetEditedSurfaceArea();
        }
        private void GetEditedAmountOfPeople()
        {
            int number;
            if (int.TryParse(AmountOfPeopleEditTextBox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(AmountOfPeopleEditTextBox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                _amountOfPeopleEdit = number;
            else
            {
                AmountOfPeopleEditTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
                AmountOfPeopleEditTextBox.BorderBrush = Brushes.Red;
                _wrongInput = true;
            }  
        }
        private void GetEditedPricePerNightPerPerson()
        {
            double number;
            if (double.TryParse(PricePerNightPerPersonEditTextBox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(PricePerNightPerPersonEditTextBox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                _pricePerNightPerPersonEdit = number;
            else
            {
                PricePerNightPerPersonEditTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
                PricePerNightPerPersonEditTextBox.BorderBrush = Brushes.Red;
                _wrongInput = true;
            }
        }
        private void GetEditedSurfaceArea()
        {
            int number;
            if (int.TryParse(SurfaceAreaEditTextBox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(SurfaceAreaEditTextBox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                _surfaceAreaEdit = number;
            else
            {
                SurfaceAreaEditTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
                SurfaceAreaEditTextBox.BorderBrush = Brushes.Red;
                _wrongInput = true;
            }
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

        private void PowerCheckButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.IsChecked == true)
            {
                checkbox.Content = "Wel stroom";
                _hasPower = true;
            }
            else if (checkbox.IsChecked == false)
            {
                checkbox.Content = "Geen voorkeur (stroom)";
                _hasPower = null;
            }
            else
            {
                checkbox.Content = "Geen stroom";
                _hasPower = false;
            }
        }

        private void DogsCheckButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.IsChecked == true)
            {
                checkbox.Content = "Wel honden";
                _dogsAllowed = true;
            }
            else if (checkbox.IsChecked == false)
            {
                checkbox.Content = "Geen voorkeur (hond)";
                _dogsAllowed = null;
            }
            else
            {
                checkbox.Content = "Geen honden";
                _dogsAllowed = false;
            }
        }



        //Function to check if the input is null or not from a text array
        /* public bool CheckIfInputIsNotNull(string[] TextInput)
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
        }*/
    }
}
