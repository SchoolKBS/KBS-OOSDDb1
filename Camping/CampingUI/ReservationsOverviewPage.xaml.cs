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

        public ReservationsOverviewWindow(Camping camping, CampingRepository campingRepository)
        {
            InitializeComponent();
            _camping = camping;

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
                Reservation? reservationToDelete = button.CommandParameter as Reservation; //Takes reservation


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
                SetReservationLables(reservation);
                ReservationOverviewGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ReservationOverviewGrid.Visibility = Visibility.Collapsed;
            }
        }
        private void SetReservationLables(Reservation reservation)
        {
            nrLabel.Content = "Reservering: " + reservation.ReservationID.ToString();
            Guest guest = _camping.CampingRepository.CampingReservationRepository.GetGuestFromGuestID(reservation.GuestID);
            GuestLabel.Content = $"Guest: {guest}";
            ArrivalDateLabel.Content = $"Aankomstdatum: {reservation.ArrivalDate.ToString("dd MMMM yyyy")}";
            DepartureDateLabel.Content = $"Vertrekdatum: {reservation.DepartureDate.ToString("dd MMMM yyyy")}";
            PlaceIDLabel.Content = $"Plaatsnummer: {reservation.PlaceID}";
            AmountOfPeopleLabel.Content = $"Aantal personnen: {reservation.AmountOfPeople}";
            string Paid;
            if (reservation.IsPaid) Paid = "Ja";
            else Paid = "Nee";
            IsPaidLabel.Content = $"Is betaald: {Paid}";
            PriceLabel.Content = $"Prijs: {String.Format("{0:0.00}", reservation.Price)}";
        }
    }
}
