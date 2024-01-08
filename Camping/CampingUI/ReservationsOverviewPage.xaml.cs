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
        public Canvas MapOverview {  get; set; }

        public ReservationsOverviewWindow(Camping camping, CampingRepository campingRepository)
        {
            InitializeComponent();
            _camping = camping;
            _camping.SetReservations(campingRepository.CampingReservationRepository.GetReservations());
            _appliedFilters = false;

            //Checks if reservations exist to load list.
            if (_camping.GetReservations().Count() > 0)
            {
                LoadReservationList();

            }
        }
        public void LoadReservationList()
        {
            ReservationsListView.ItemsSource = _camping.GetReservations().Where(reservation => reservation.DepartureDate >= DateTime.Now.Date).OrderBy(reservation => reservation.ArrivalDate).ThenBy(reservation => reservation.PlaceID); //Takes reservations
        }
        public void ApplyFilters()
        {
            IEnumerable<Reservation> filteredReservations = _camping.GetReservations();

            DateTime? arrivalDate = ArrivalDatePickerr.SelectedDate;
            if (arrivalDate.HasValue)
            {
                if (arrivalDate < DateTime.Now.Date)
                {
                    ArrivalDatePickerr.Text = string.Empty;
                    DepartureDatePickerr.Text = string.Empty;
                    StaticUIMethods.SetErrorDatePickerBorder(ArrivalDatePickerr);
                    StaticUIMethods.SetErrorDatePickerBorder(DepartureDatePickerr);

                }
                else
                {
                    StaticUIMethods.ResetDatePickerBorder(ArrivalDatePickerr);
                    StaticUIMethods.ResetDatePickerBorder(DepartureDatePickerr);
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
                    ArrivalDatePickerr.BorderBrush = Brushes.White;
                    ArrivalDatePickerr.BorderThickness = new Thickness(3);

                    DepartureDatePickerr.BorderBrush = Brushes.White;
                    DepartureDatePickerr.BorderThickness = new Thickness(3);

                    _appliedFilters = true;
                    filteredReservations = filteredReservations.Where(reservation => reservation.DepartureDate.Date <= departureDate.Value.Date);
                }
                else
                {
                    ArrivalDatePickerr.Text = string.Empty;
                    DepartureDatePickerr.Text = string.Empty;
                    ArrivalDatePickerr.BorderBrush = Brushes.Red;
                    ArrivalDatePickerr.BorderThickness = new Thickness(3);

                    DepartureDatePickerr.BorderBrush = Brushes.Red;
                    DepartureDatePickerr.BorderThickness = new Thickness(3);


                }
            }

            


            // Filter by ReservationID
            if (int.TryParse(ReservationIdBox.Text, out int reservatieID) && reservatieID >= 0)
            {
                _appliedFilters = true;
                ReservationIdBox.BorderBrush = Brushes.White;
                ReservationIdBox.BorderThickness = new Thickness(3);

                filteredReservations = filteredReservations.Where(reservation => reservation.ReservationID == reservatieID);
            }
            else if (ReservationIdBox.Text == string.Empty)
            {
                ReservationIdBox.BorderBrush = Brushes.White;
                ReservationIdBox.BorderThickness = new Thickness(3);

            }
            else
            {
                ReservationIdBox.Text = string.Empty;
                ReservationIdBox.BorderBrush = Brushes.Red;
                ReservationIdBox.BorderThickness = new Thickness(3);

            }


            // Filter by Place Number
            if (int.TryParse(PlaceNumberBox.Text, out int placeNumber) && placeNumber >= 0)
            {
                _appliedFilters = true;
                PlaceNumberBox.BorderBrush = Brushes.White;
                PlaceNumberBox.BorderThickness = new Thickness(3);

                filteredReservations = filteredReservations.Where(reservation => reservation.PlaceID == placeNumber);
            }
            else if (PlaceNumberBox.Text == string.Empty)
            {
                PlaceNumberBox.BorderBrush = Brushes.White;
                PlaceNumberBox.BorderThickness = new Thickness(3);

            }
            else 
            {
                PlaceNumberBox.Text = string.Empty;
                PlaceNumberBox.BorderBrush = Brushes.Red;
                PlaceNumberBox.BorderThickness = new Thickness(3);

            }



            // Filter by Guest Name
            if (!string.IsNullOrEmpty(GuestNameBox.Text) && !(int.TryParse(GuestNameBox.Text, out int guest)))
            {
                _appliedFilters = true;
                GuestNameBox.BorderBrush = Brushes.White;
                GuestNameBox.BorderThickness = new Thickness(3);

                filteredReservations = filteredReservations
       .Where(reservation =>
           reservation.GuestName.Contains(GuestNameBox.Text, StringComparison.OrdinalIgnoreCase)
       );
            }
            else if (GuestNameBox.Text == string.Empty)
            {
                GuestNameBox.BorderBrush = Brushes.White;
                GuestNameBox.BorderThickness = new Thickness(3);


            }
            else 
            {
                GuestNameBox.Text = string.Empty;
                GuestNameBox.BorderBrush = Brushes.Red;
                GuestNameBox.BorderThickness = new Thickness(3);

            }

            // Apply the combined filters and update the ListView
            ReservationsListView.ItemsSource = filteredReservations
                .OrderBy(reservation => reservation.ArrivalDate)
                .ThenBy(reservation => reservation.PlaceID);
        }
        private void Filter_Filled(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }
        private void Texbox_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyFilters();
            }
        }
        private void PriceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool? isPaid = PriceCheckBox.IsChecked;

            if (isPaid == true)
            {
                ApplyFilters();
                PriceCheckBox.Content = "Wel Betaald";

            }
            else if (isPaid == false)
            {
                ApplyFilters();
                PriceCheckBox.Content = "Geen Voorkeur";

            }
            else if (isPaid == null)
            {
                ApplyFilters();
                PriceCheckBox.Content = "Niet Betaald";

            }
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
            PlaceNumberBox.Text = string.Empty;
            GuestNameBox.Text = string.Empty;
            ReservationIdBox.Text = string.Empty;
            StaticUIMethods.ResetTextboxBorder(GuestNameBox);
            StaticUIMethods.ResetTextboxBorder(PlaceNumberBox);
            StaticUIMethods.ResetTextboxBorder(ReservationIdBox);
            StaticUIMethods.ResetDatePickerBorder(ArrivalDatePickerr);
            StaticUIMethods.ResetDatePickerBorder(DepartureDatePickerr);
            _appliedFilters = false;
            PriceCheckBox.Content = "Geen Voorkeur";
            PriceCheckBox.IsChecked = false;

            // Reload the original data without filters
            LoadReservationList();
        }
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
                        _camping.GetReservations().Remove(reservationToDelete);
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
                CollapseTextBoxForEditReservation();
                Reservation reservation = (Reservation)ReservationsListView.SelectedItems[0];
                SetReservationLabels(reservation);
                OpenReservationOverview();
            }
            else
            {
                ReservationOverviewGrid.Visibility = Visibility.Collapsed;
            }
        }
        private void AppearTextBoxForEditReservation()
        {
            ArrivalDateBox.Visibility = Visibility.Visible;
            DepartureDateBox.Visibility = Visibility.Visible;
            IsPaidBox.Visibility = Visibility.Visible;
            AmountOfPeopleBox.Visibility = Visibility.Visible;
            PlaceDropDown.Visibility = Visibility.Visible;
            CancelEditButton.Visibility = Visibility.Visible;
            EditReservationButton.Visibility = Visibility.Collapsed;
            EditReservationButtonConfirm.Visibility = Visibility.Visible;
              DeleteReservationButton.Visibility = Visibility.Collapsed;

        }
        private void CollapseTextBoxForEditReservation()
        {

            DepartureDateBox.BlackoutDates.Clear();
            ArrivalDateBox.BlackoutDates.Clear();
            ArrivalDateBox.BorderBrush = Brushes.White;
            DepartureDateBox.BorderBrush = Brushes.White;
            AmountOfPeopleBox.BorderBrush = Brushes.White;

            DeleteReservationButton.Visibility = Visibility.Visible;
            ArrivalDateBox.Visibility = Visibility.Collapsed;
            DepartureDateBox.Visibility = Visibility.Collapsed;
            IsPaidBox.Visibility = Visibility.Collapsed;
            AmountOfPeopleBox.Visibility = Visibility.Collapsed;
            PlaceDropDown.Visibility = Visibility.Collapsed;
            CancelEditButton.Visibility = Visibility.Collapsed;
            EditReservationButton.Visibility = Visibility.Visible;
            EditReservationButtonConfirm.Visibility = Visibility.Collapsed;

        }
        private void TextEditReservationButtonEditMode()
        {
            EditReservationButton.Content = "Reservering Opslaan";
        }
        private void TextEditReservationButtonViewingMode()
        {
            EditReservationButton.Content = "Reservering Aanpassen";
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
            TextEditReservationButtonViewingMode();
        }
        private void SetEditReservationLabels(Reservation reservation)
        {
            Guest guest = _camping.CampingRepository.CampingReservationRepository.GetGuestFromGuestID(reservation.GuestID);

            ArrivalDateLabel.Content = $"Aankomstdatum: ";
            ArrivalDateBox.SelectedDate = reservation.ArrivalDate;
            DepartureDateLabel.Content = $"Vertrekdatum: ";
            DepartureDateBox.SelectedDate = reservation.DepartureDate;
            if (CheckIfReservationIsInPast()) SetDatePickers();
            PlaceIDLabel.Content = $"Plaatsnummer: ";
            PlaceDropDown.Text = $"{reservation.PlaceID}";
            AmountOfPeopleLabel.Content = $"Aantal personen: ";
            AmountOfPeopleBox.Text = $"{reservation.AmountOfPeople}";
            IsPaidLabel.Content = $"Is betaald: ";
            string Paid;

            IsPaidBox.IsChecked = reservation.IsPaid;
            SetDropDown();


            TextEditReservationButtonEditMode();

        }
        private void EditReservation_Click(object sender, RoutedEventArgs e)
        {
            OpenReservationEdit();
            _reservation = (Reservation)ReservationsListView.SelectedItems[0];
        }
        private void EditReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidInput() && IsValidInputPlace())
            {
                PlaceIDLabel.Content = string.IsNullOrEmpty(PlaceDropDown.Text) ? "Plaatsnummer: Er moet een plaats geselecteerd worden" : "Plaatsnummer:";

                if (ArrivalDateBox.SelectedDate < DepartureDateBox.SelectedDate)
                {
                    ArrivalDateBox.BorderBrush = Brushes.White;
                    DepartureDateBox.BorderBrush = Brushes.White;

                    setReservationData();
                    _camping.CampingRepository.CampingReservationRepository.UpdateReservation(_reservation);

                    ReservationOverviewGrid.Visibility = Visibility.Visible;
                    LoadReservationList();
                    ReservationsListView.SelectedItem = ReservationsListView.Items.IndexOf(_reservation);

                    _reservation = (Reservation)ReservationsListView.SelectedItem;
                    SetReservationLabels(_reservation);
                    Reservation reservation = (Reservation)ReservationsListView.SelectedItems[0];
                    SetReservationLabels(reservation);
                    CollapseTextBoxForEditReservation();
                }
                else
                {
                    ArrivalDateBox.BorderBrush = Brushes.Red;
                    DepartureDateBox.BorderBrush = Brushes.Red;
                }
            }
        }
        private bool IsValidInput()
        {
            if (int.TryParse(AmountOfPeopleBox.Text, out var amountOfPeople) && amountOfPeople > 0)
            {
                AmountOfPeopleBox.BorderBrush = Brushes.White;
                return true;
            }

            AmountOfPeopleBox.BorderBrush = Brushes.Red;
            return false;
        }
        private bool IsValidInputPlace()
        {
            if (PlaceDropDown.Items.Count > 0)
            {
                PlaceDropDown.BorderBrush = Brushes.White;
                return true;
            }
            PlaceDropDown.BorderBrush = Brushes.Red;

            return false;

        }
        private void CancelReservationButton_Click(object sender, RoutedEventArgs e)
        {
            CollapseTextBoxForEditReservation();
            Reservation reservation = (Reservation)ReservationsListView.SelectedItems[0];
            SetReservationLabels(reservation);
        }
        private void OpenReservationOverview()
        {
            ReservationOverviewGrid.Visibility = Visibility.Visible;
        }
        private void OpenReservationEdit()
        {
            AppearTextBoxForEditReservation();
            if (ReservationsListView.SelectedItems.Count > 0)
            {
                Reservation reservation = (Reservation)ReservationsListView.SelectedItems[0];
                SetEditReservationLabels(reservation);
            }
        }
        private void SetDatePickers()
        {
            ShowAvailableDatesArrival();
            ShowAvailableDatesDeparture();
        }
        private void SetDropDown()
        {
            Reservation reservation = (Reservation)ReservationsListView.SelectedItems[0];
            PlaceDropDown.Items.Clear();
            List<Place> list = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            list = list.Where(r => CheckDateOfPlace(r)).OrderBy(r => r.PlaceID).ToList();
            
                Place currentPlace = _camping.CampingRepository.CampingPlaceRepository.GetPlaceFromPlaceID(reservation.PlaceID);
                int.TryParse(AmountOfPeopleBox.Text, out int AmountOfPeople);
                foreach (Place place in list)
                {
                    if (place.Dogs == currentPlace.Dogs && place.Power == currentPlace.Power && AmountOfPeople <= place.AmountOfPeople)
                    {
                        PlaceDropDown.Items.Add(place);
                    }
                }
                int index = PlaceDropDown.Items.IndexOf(currentPlace);
                if (index >= 0) PlaceDropDown.SelectedItem = PlaceDropDown.Items[index];
                if (!ArrivalDateBox.IsEnabled) PlaceDropDown.IsEnabled = false;
                else PlaceDropDown.IsEnabled = true;
            
        }
        private bool CheckData()
        {
            bool result = true;
            if (!CheckAmountOfPeople()) result = false;
            if (!CheckPlace()) result = false;
            return result;
        }
        private bool CheckPlace()
        {
            if (PlaceDropDown.SelectedItem != null) return true;
            PlaceDropDown.BorderBrush = Brushes.Red;
            PlaceDropDown.BorderThickness = new Thickness(2);
            return false;
        }
        private bool CheckAmountOfPeople()
        {
            if (int.TryParse(AmountOfPeopleBox.Text, out int numb) && numb > 0)
            {
                AmountOfPeopleBox.BorderBrush = null;
                return true;
            }
            else
            {
                AmountOfPeopleBox.BorderBrush = Brushes.Red;
                AmountOfPeopleBox.BorderThickness = new Thickness(2);
                return false;
            }
        }
        private bool CheckIfReservationIsInPast()
        {
            bool enabled = true;
            if (ArrivalDateBox.SelectedDate < DateTime.Now.Date && DepartureDateBox.SelectedDate < DateTime.Now.Date)
            {
                enabled = false;
            }
            ArrivalDateBox.IsEnabled = enabled;
            DepartureDateBox.IsEnabled = enabled;
            AmountOfPeopleBox.IsEnabled = enabled;
            PlaceDropDown.IsEnabled = enabled;
            return enabled;
        }
        private bool CheckDateOfPlace(Place place)
        {
            string nrLabelText = nrLabel.Content.ToString();
            nrLabelText = nrLabelText.Replace("Reservering: ", "");

            List<Reservation> list = _camping.CampingRepository.CampingReservationRepository.GetReservations();
            list = list.Where(r => r.PlaceID == place.PlaceID).ToList();
            bool result = true;
            foreach (Reservation reservation in list)
            {
                if (((reservation.ArrivalDate >= ArrivalDateBox.SelectedDate && reservation.ArrivalDate <= DepartureDateBox.SelectedDate) ||
                    (reservation.DepartureDate <= DepartureDateBox.SelectedDate && reservation.DepartureDate >= ArrivalDateBox.SelectedDate) ||
                    (reservation.ArrivalDate <=  ArrivalDateBox.SelectedDate && reservation.DepartureDate >= DepartureDateBox.SelectedDate)) && reservation.ReservationID.ToString() != nrLabelText)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        private void ShowAvailableDatesArrival()
        {
            if (ArrivalDateBox.SelectedDate >= DateTime.Today.Date)
            {
                ArrivalDateBox.IsEnabled = true;
                ArrivalDateBox.BlackoutDates.Clear();
                ArrivalDateBox.DisplayDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                ArrivalDateBox.BlackoutDates.AddDatesInPast();
            }
            else
            {
                ArrivalDateBox.BlackoutDates.Clear();
                ArrivalDateBox.DisplayDateStart = DateTime.MinValue;
                ArrivalDateBox.IsEnabled = false;
            }
        }
        private void ShowAvailableDatesDeparture()
        {
            if (ArrivalDateBox.SelectedDate != null)
            {
                DepartureDateBox.DisplayDateEnd = null;
                DepartureDateBox.BlackoutDates.Clear();
                DateTime arrivalDate = (DateTime)ArrivalDateBox.SelectedDate;
                DepartureDateBox.DisplayDateStart = new DateTime(arrivalDate.Year, arrivalDate.Month, 1);

                if (arrivalDate < DateTime.Now.Date)
                {
                    DepartureDateBox.BlackoutDates.Add(new CalendarDateRange(DepartureDateBox.DisplayDateStart.Value, DateTime.Now.Date));
                }
                else
                {
                    if (DepartureDateBox.DisplayDateStart != null && arrivalDate > DepartureDateBox.DisplayDateStart)
                    {
                     
                    }
                    else
                    {
                        DepartureDateBox.BlackoutDates.Add(new CalendarDateRange(DepartureDateBox.DisplayDateStart.Value, arrivalDate));
                    }
                }
            }
        }
        private void setReservationData()
        {
            _reservation.ArrivalDate = (DateTime)ArrivalDateBox.SelectedDate;
            _reservation.DepartureDate = (DateTime)DepartureDateBox.SelectedDate;
            _reservation.AmountOfPeople = int.Parse(AmountOfPeopleBox.Text);
            Place place = (Place)PlaceDropDown.SelectedItem;
            _reservation.PlaceID = place.PlaceID;
            _reservation.Price = place.PricePerNightPerPerson * _reservation.AmountOfPeople * (_reservation.DepartureDate - _reservation.ArrivalDate).Days;
            _reservation.IsPaid = IsPaidBox.IsChecked.GetValueOrDefault();
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
                int peopleCount = int.Parse(AmountOfPeopleBox.Text);
                DateTime arrivalDate = (DateTime)ArrivalDateBox.SelectedDate;
                DateTime departureDate = (DateTime)DepartureDateBox.SelectedDate;
                double price = place.PricePerNightPerPerson * peopleCount * Math.Abs((departureDate - arrivalDate).Days);
                PriceLabel.Content = $"Prijs: {String.Format("{0:0.00}", price)}€";
            }
            else
            {
                PriceLabel.Content = $"Prijs: --€";
            }
        }

    }
}
