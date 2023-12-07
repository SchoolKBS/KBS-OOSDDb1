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

            // Filter by Departure Date
            DateTime? departureDate = DepartureDatePickerr.SelectedDate;
            if (departureDate.HasValue)
            {
                if (departureDate > DateTime.Now.Date && departureDate >= arrivalDate)
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



            // Filter by Number of People
            if (int.TryParse(PersonCountTextBoxx.Text, out int personCount) && personCount >= 0)
            {
                PersonCountTextBoxx.Background = Brushes.White;
                filteredReservations = filteredReservations.Where(reservation => reservation.AmountOfPeople == personCount);
            }
            else if (PersonCountTextBoxx.Text == string.Empty)
            {
                PersonCountTextBoxx.Background = Brushes.White;

            }
            else
            {
                PersonCountTextBoxx.Text = string.Empty;
                PersonCountTextBoxx.Background = Brushes.Red;
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
            PersonCountTextBoxx.Text = string.Empty;
            PlaceNumerBox.Text = string.Empty;
            GuestNameBox.Text = string.Empty;
            GuestNameBox.Background = Brushes.White;
            ArrivalDatePickerr.Background = Brushes.White;
            DepartureDatePickerr.Background = Brushes.White;
            PlaceNumerBox.Background = Brushes.White;
            PersonCountTextBoxx.Background = Brushes.White;


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
