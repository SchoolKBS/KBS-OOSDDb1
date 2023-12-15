using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using Google.Protobuf.WellKnownTypes;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Numerics;
using System.Printing;
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
    /// Interaction logic for ReservationsOverviewWindow.xaml
    /// </summary>
    public partial class ReservationsOverviewWindow : Page
    {

        private Camping _camping { get; set; }
        private Reservation _reservation { get; set; }
        private bool _appliedFilters {  get; set; }

        public ReservationsOverviewWindow(Camping camping, CampingRepository campingRepository)
        {
            InitializeComponent();
            _camping = camping;
            _appliedFilters = false;

            //Checks if reservations exist to load list.
            if (_camping.Reservations.Count() > 0)
            {
                LoadReservationList();

            }
        }

        //Fills list with reservations
        public void LoadReservationList()
        {
            ReservationsListView.ItemsSource = _camping.Reservations.Where(reservation => reservation.DepartureDate >= DateTime.Now.Date).OrderBy(reservation => reservation.ArrivalDate).ThenBy(reservation => reservation.PlaceID); //Takes reservations
        }

        private void PriceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool? isPaid = PriceCheckBox.IsChecked;

            if (isPaid == true)
            {
                PriceCheckBox.Content = "Wel Betaald";

            }
            else if (isPaid == false)
            {
                PriceCheckBox.Content = "Geen Voorkeur";

            }
            else if (isPaid == null)
            {
                PriceCheckBox.Content = "Niet Betaald";

            }
        }
          



        // Funcion for the application of all filters
        public void ApplyFilters()
        {
            IEnumerable<Reservation> filteredReservations = _camping.Reservations;
            DateTime? arrivalDate = ArrivalDatePickerr.SelectedDate;
            if (arrivalDate.HasValue)
            {
                if (arrivalDate < DateTime.Now.Date)
                {
                    ArrivalDatePickerr.Text = string.Empty;
                    DepartureDatePickerr.Text = string.Empty;
                    ArrivalDatePickerr.Background = Brushes.Red;
                    DepartureDatePickerr.Background = Brushes.Red;

                }
                else
                {
                    ArrivalDatePickerr.Background = Brushes.White;
                    DepartureDatePickerr.Background = Brushes.White;
                    _appliedFilters = true;
                    filteredReservations = filteredReservations.Where(reservation => reservation.ArrivalDate.Date >= arrivalDate.Value.Date);
                }
            }

            bool? isPaid = PriceCheckBox.IsChecked;
            if (isPaid.HasValue)
            {
                if (isPaid == true)
                {
                    filteredReservations = filteredReservations.Where(reservation => reservation.IsPaid == true);
                    PriceCheckBox.Content = "Wel Betaald";

                }
            }
            else
            {

                filteredReservations = filteredReservations.Where(reservation => reservation.IsPaid == false);
            }
            

            // Filter by Departure Date
            DateTime? departureDate = DepartureDatePickerr.SelectedDate;
            if (departureDate.HasValue)
            {
                if (departureDate > DateTime.Now.Date && departureDate >= arrivalDate || ArrivalDatePickerr.Text == string.Empty && !(DepartureDatePickerr.Text == string.Empty))
                {
                    ArrivalDatePickerr.Background = Brushes.White;
                    DepartureDatePickerr.Background = Brushes.White;
                    _appliedFilters = true;
                    filteredReservations = filteredReservations.Where(reservation => reservation.DepartureDate.Date <= departureDate.Value.Date);
                }
                else
                {
                    ArrivalDatePickerr.Text = string.Empty;
                    DepartureDatePickerr.Text = string.Empty;
                    ArrivalDatePickerr.Background = Brushes.Red;
                    DepartureDatePickerr.Background = Brushes.Red;

                }
            }

            


            // Filter by ReservationID
            if (int.TryParse(ReservationIdBox.Text, out int reservatieID) && reservatieID >= 0)
            {
                _appliedFilters = true;
                ReservationIdBox.Background = Brushes.White;
                filteredReservations = filteredReservations.Where(reservation => reservation.ReservationID == reservatieID);
            }
            else if (ReservationIdBox.Text == string.Empty)
            {
                ReservationIdBox.Background = Brushes.White;

            }
            else
            {
                ReservationIdBox.Text = string.Empty;
                ReservationIdBox.Background = Brushes.Red;
            }


            // Filter by Place Number
            if (int.TryParse(PlaceNumerBox.Text, out int placeNumber) && placeNumber >= 0)
            {
                _appliedFilters = true;
                PlaceNumerBox.Background = Brushes.White;
                filteredReservations = filteredReservations.Where(reservation => reservation.PlaceID == placeNumber);
            }
            else if (PlaceNumerBox.Text == string.Empty)
            {
                PlaceNumerBox.Background = Brushes.White;

            }
            else 
            {
                PlaceNumerBox.Text = string.Empty;
                PlaceNumerBox.Background = Brushes.Red;
            }

                
              
            // Filter by Guest Name
            if (!string.IsNullOrEmpty(GuestNameBox.Text) && !(int.TryParse(GuestNameBox.Text, out int guest)))
            {
                _appliedFilters = true;
                GuestNameBox.Background = Brushes.White;
                filteredReservations = filteredReservations
       .Where(reservation =>
           reservation.GuestName.Contains(GuestNameBox.Text, StringComparison.OrdinalIgnoreCase)
       );
            }
            else if (GuestNameBox.Text == string.Empty)
            {
                GuestNameBox.Background = Brushes.White;

            }
            else 
            {
                GuestNameBox.Text = string.Empty;
                GuestNameBox.Background = Brushes.Red;
            }
           
            // Apply the combined filters and update the ListView
            ReservationsListView.ItemsSource = filteredReservations
                .OrderBy(reservation => reservation.ArrivalDate)
                .ThenBy(reservation => reservation.PlaceID);
        }
       
        private void ApplyFilters_Clickk(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void RemoveFilters_Clickk(object sender, RoutedEventArgs e)
        {
            RemoveFilters();
        }
        private void RemoveFilters()
        {
            // Clear filter criteria
            ArrivalDatePickerr.SelectedDate = null;
            DepartureDatePickerr.SelectedDate = null;
            PlaceNumerBox.Text = string.Empty;
            GuestNameBox.Text = string.Empty;
            ReservationIdBox.Text = string.Empty;
            GuestNameBox.Background = Brushes.White;
            ArrivalDatePickerr.Background = Brushes.White;
            DepartureDatePickerr.Background = Brushes.White;
            PlaceNumerBox.Background = Brushes.White;
            ReservationIdBox.Background = Brushes.White;
            _appliedFilters = false;


            // Reload the original data without filters
            LoadReservationList();
        }


        //Function to delete reservations
        public void DeleteButton_Click(object sender, RoutedEventArgs e) 
        {
            Button? button = sender as Button;

            //Button pressed?
            if (button != null)
            {
                Reservation? reservationToDelete = (Reservation)ReservationsListView.SelectedItems[0]; //Takes reservation


                if (reservationToDelete != null)
                {
                    // Show a confirmation dialog
                    MessageBoxResult result = MessageBox.Show("Weet je zeker dat je reservering " + reservationToDelete.ReservationID + " wil verwijderen?", "Waarschuwing!", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    // Check the users choice
                    if (result == MessageBoxResult.Yes)
                    {
                        // User clicked Yes, so delete the reservation
                        _camping.Reservations.Remove(reservationToDelete);
                        _camping.CampingRepository.CampingReservationRepository.RemoveReservation(reservationToDelete);

                        // Refresh the ListView
                        LoadReservationList();
                    }
                    // If the user clicked No, do nothing
                }
            }

        }

        private void ReservationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReservationsListView.SelectedItems.Count > 0)
            {
                Reservation reservation = (Reservation)ReservationsListView.SelectedItems[0];
                SetReservationLabels(reservation);
                OpenReservationOverview();
            }
            else
            {
                ReservationOverviewGrid.Visibility = Visibility.Collapsed;
            }
        }
        private void SetReservationLabels(Reservation reservation)
        {
            nrLabel.Content = "Reservering: " + reservation.ReservationID.ToString();
            Guest guest = _camping.CampingRepository.CampingReservationRepository.GetGuestFromGuestID(reservation.GuestID);
            GuestLabel.Content = $"Gast: {guest}";
            ArrivalDateLabel.Content = $"Aankomstdatum: {reservation.ArrivalDate.ToString("dd MMMM yyyy")}";
            DepartureDateLabel.Content = $"Vertrekdatum: {reservation.DepartureDate.ToString("dd MMMM yyyy")}";
            PlaceIDLabel.Content = $"Plaatsnummer: {reservation.PlaceID}";
            AmountOfPeopleLabel.Content = $"Aantal personen: {reservation.AmountOfPeople}";
            string Paid;
            if (reservation.IsPaid) Paid = "Ja";
            else Paid = "Nee";
            IsPaidLabel.Content = $"Is betaald: {Paid}";
            PriceLabel.Content = $"Prijs: {String.Format("{0:0.00}", reservation.Price)}€";
        }

        private void EditReservation_Click(object sender, RoutedEventArgs e)
        {
            OpenReservationEdit();
            _reservation = (Reservation)ReservationsListView.SelectedItems[0];
            ReservationIDLabelEdit.Content = $"Reservering {_reservation.ReservationID}";
            SetEditFields();
        }

        private void EditReservationConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                setReservationData();
                _camping.CampingRepository.CampingReservationRepository.UpdateReservation(_reservation);
                ReservationEditGrid.Visibility = Visibility.Collapsed;
                ReservationOverviewGrid.Visibility = Visibility.Visible;
                LoadReservationList();
                ReservationsListView.SelectedItem = ReservationsListView.Items.IndexOf(_reservation);
                _reservation = (Reservation)ReservationsListView.SelectedItem;
                SetReservationLabels(_reservation);
            }

        }
        private void CancelReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationEditGrid.Visibility = Visibility.Collapsed;
            ReservationOverviewGrid.Visibility = Visibility.Visible;
        }
        private void OpenReservationOverview()
        {
            ReservationOverviewGrid.Visibility = Visibility.Visible;
            ReservationEditGrid.Visibility = Visibility.Collapsed;
        }
        private void OpenReservationEdit()
        {
            ReservationOverviewGrid.Visibility = Visibility.Collapsed;
            ReservationEditGrid.Visibility = Visibility.Visible;
        }
        private void SetEditFields()
        {
            ArrivalDatePicker.SelectedDate = _reservation.ArrivalDate;
            DepartureDatePicker.SelectedDate = _reservation.DepartureDate;
            if(CheckIfReservationIsInPast()) SetDatePickers();
            AmountOfPeopleTextBox.Text = _reservation.AmountOfPeople.ToString();
            IsPaidCheckBox.IsChecked = _reservation.IsPaid;
            SetDropDown();
            PriceEditLabel.Content = $"Prijs: {String.Format("{0:0.00}", _reservation.Price)}€";
        }
        private bool CheckIfReservationIsInPast()
        {
            bool enabled = true;
            if (ArrivalDatePicker.SelectedDate < DateTime.Now.Date && DepartureDatePicker.SelectedDate < DateTime.Now.Date) {
                enabled = false;
            }
            ArrivalDatePicker.IsEnabled = enabled;
            DepartureDatePicker.IsEnabled = enabled;
            AmountOfPeopleTextBox.IsEnabled = enabled;
            PlaceDropDown.IsEnabled = enabled;
            return enabled;
        }
        private void SetDatePickers()
        {
            ShowAvailableDatesArrival();
            ShowAvailableDatesDeparture();
        }
        private void SetDropDown()
        {
            PlaceDropDown.Items.Clear();
            List<Place> list = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            list = list.Where(r => CheckDateOfPlace(r)).OrderBy(r => r.PlaceID).ToList();
            Place currentPlace = _camping.CampingRepository.CampingPlaceRepository.GetPlaceFromPlaceID(_reservation.PlaceID);
            int.TryParse(AmountOfPeopleTextBox.Text, out int AmountOfPeople);
            foreach (Place place in list)
            {
                if(place.Dogs == currentPlace.Dogs && place.Power == currentPlace.Power && AmountOfPeople <= place.AmountOfPeople)
                {
                    PlaceDropDown.Items.Add(place);
                }
            }
            int index = PlaceDropDown.Items.IndexOf(currentPlace);
            if (index >= 0) PlaceDropDown.SelectedItem = PlaceDropDown.Items[index];
            if (!ArrivalDatePicker.IsEnabled) PlaceDropDown.IsEnabled = false;
            else PlaceDropDown.IsEnabled = true;
        }
        private bool CheckDateOfPlace(Place place)
        {
            List<Reservation> list = _camping.CampingRepository.CampingReservationRepository.GetReservations();
            list = list.Where(r => r.PlaceID == place.PlaceID).ToList();
            bool result = true;
            foreach (Reservation reservation in list)
            {
                if (((reservation.ArrivalDate >= ArrivalDatePicker.SelectedDate && reservation.ArrivalDate <= DepartureDatePicker.SelectedDate) ||
                    (reservation.DepartureDate <= DepartureDatePicker.SelectedDate && reservation.DepartureDate >= ArrivalDatePicker.SelectedDate) ||
                    (reservation.ArrivalDate <= ArrivalDatePicker.SelectedDate && reservation.DepartureDate >= DepartureDatePicker.SelectedDate)) && reservation.ReservationID != _reservation.ReservationID)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        private void ShowAvailableDatesArrival()
        {
            if (ArrivalDatePicker.SelectedDate >= DateTime.Today.Date)
            {
                ArrivalDatePicker.IsEnabled = true;
                ArrivalDatePicker.BlackoutDates.Clear();
                ArrivalDatePicker.DisplayDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                ArrivalDatePicker.BlackoutDates.AddDatesInPast();
            }
            else
            {
                ArrivalDatePicker.BlackoutDates.Clear();
                ArrivalDatePicker.DisplayDateStart = DateTime.MinValue;
                ArrivalDatePicker.IsEnabled = false;
            }
        }

        // Sets the possible DepartureDates in the DepartureDatePicker.
        private void ShowAvailableDatesDeparture()
        {
            if(ArrivalDatePicker.SelectedDate >= DepartureDatePicker.SelectedDate) { DepartureDatePicker.SelectedDate = null; }
            if (ArrivalDatePicker.SelectedDate != null)
            {
                DepartureDatePicker.DisplayDateEnd = null;
                DepartureDatePicker.BlackoutDates.Clear();
                DateTime arrivalDate = (DateTime)ArrivalDatePicker.SelectedDate;
                DepartureDatePicker.DisplayDateStart = new DateTime(arrivalDate.Year, arrivalDate.Month, 1);
                if(arrivalDate < DateTime.Now.Date) DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(DepartureDatePicker.DisplayDateStart.Value, DateTime.Now.Date));
                else DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(DepartureDatePicker.DisplayDateStart.Value, arrivalDate));
            }
        }
        private bool CheckData()
        {
            bool result = true;
            if(!CheckAmountOfPeople()) result = false;
            if(!CheckPlace()) result = false;
            if(DepartureDatePicker.SelectedDate == null) result = false;
            return result;
        }
        private bool CheckPlace()
        {
            if(PlaceDropDown.SelectedItem != null) return true;
            PlaceDropDown.BorderBrush = Brushes.Red;
            PlaceDropDown.BorderThickness = new Thickness(2);
            return false;
        }
        private bool CheckAmountOfPeople()
        {
            if(int.TryParse(AmountOfPeopleTextBox.Text, out int numb) && numb > 0)
            {
                AmountOfPeopleTextBox.BorderBrush = null;
                return true;
            }
            else
            {
                AmountOfPeopleTextBox.BorderBrush = Brushes.Red;
                AmountOfPeopleTextBox.BorderThickness = new Thickness(2);
                return false;
            }
        }
        private void setReservationData()
        {
            _reservation.ArrivalDate = (DateTime)ArrivalDatePicker.SelectedDate;
            _reservation.DepartureDate = (DateTime)DepartureDatePicker.SelectedDate;
            _reservation.AmountOfPeople = int.Parse(AmountOfPeopleTextBox.Text);
            Place place = (Place)PlaceDropDown.SelectedItem;
            _reservation.PlaceID = place.PlaceID;
            _reservation.Price = place.PricePerNightPerPerson * _reservation.AmountOfPeople * (_reservation.DepartureDate - _reservation.ArrivalDate).Days;
            _reservation.IsPaid = IsPaidCheckBox.IsPressed;
        }

        private void DataChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDatePickers();
            SetDropDown();
            SetPrice();
        }

        private void AmountOfPeople_Changed(object sender, TextChangedEventArgs e)
        {
            CheckAmountOfPeople();
            SetDropDown();
            SetPrice();
        }

        private void PlaceDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlaceDropDown.BorderBrush = null;
            SetPrice();
        }
        private void SetPrice()
        {
            if (CheckData()) {
                Place place = (Place)PlaceDropDown.SelectedItem;
                int peopleCount = int.Parse(AmountOfPeopleTextBox.Text);
                double price = place.PricePerNightPerPerson * peopleCount * ((DateTime)DepartureDatePicker.SelectedDate - (DateTime)ArrivalDatePicker.SelectedDate).Days;
                PriceEditLabel.Content = $"Prijs: {String.Format("{0:0.00}", price)}€";
            }
            else
            {
                PriceEditLabel.Content = $"Prijs: --€";
            }
        }

    }
}
