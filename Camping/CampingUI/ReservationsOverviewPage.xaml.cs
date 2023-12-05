using CampingCore;
using CampingDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for ReservationsOverviewWindow.xaml
    /// </summary>
    public partial class ReservationsOverviewWindow : Page
    {

        private Camping _camping { get; set; }

        public ReservationsOverviewWindow(Camping camping, SqliteRepository campingRepository)
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

        public void ApplyFilters()
        {
            IEnumerable<Reservation> filteredReservations = _camping.Reservations;

            DateTime? arrivalDate = ArrivalDatePickerr.SelectedDate;
            if (arrivalDate.HasValue)
            {
                filteredReservations = filteredReservations.Where(reservation => reservation.ArrivalDate.Date >= arrivalDate.Value.Date);
            }

            // Filter by Departure Date
            DateTime? departureDate = DepartureDatePickerr.SelectedDate;
            if (departureDate.HasValue)
            {
                filteredReservations = filteredReservations.Where(reservation => reservation.DepartureDate.Date <= departureDate.Value.Date);
            }

            // Filter by Number of People
            if (int.TryParse(PersonCountTextBoxx.Text, out int personCount))
            {
                filteredReservations = filteredReservations.Where(reservation => reservation.AmountOfPeople == personCount);
            }

            // Filter by Place Number
            if (int.TryParse(PlaceNumerBox.Text, out int placeNumber))
            {
                filteredReservations = filteredReservations.Where(reservation => reservation.PlaceID == placeNumber);
            }

            // Filter by Guest Name
            if (!string.IsNullOrEmpty(GuestNameBox.Text))
            {
                filteredReservations = filteredReservations
                       .Where(reservation =>
                           reservation.GuestName.Replace(" ", "").Contains(GuestNameBox.Text.Replace(" ", ""))
                       );
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
            PersonCountTextBoxx.Text = string.Empty;
            PlaceNumerBox.Text = string.Empty;
            GuestNameBox.Text = string.Empty;

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
                        _camping.CampingRepository.RemoveReservation(reservationToDelete);

                        // Refresh the ListView
                        LoadReservationList();
                    }
                    // If the user clicked No, do nothing
                }
            }

        }
    }
}
