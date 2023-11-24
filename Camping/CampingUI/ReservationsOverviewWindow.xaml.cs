using CampingCore;
using CampingDataAccess;
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

        private Camping camping { get; set; }

        public ReservationsOverviewWindow(Camping camping, CampingRepository campingRepository)
        {
            InitializeComponent();
            this.camping = new Camping();

            //Checks if reservations exist to load list.
            if (camping.Reservations.Count() > 0)
            {
                LoadReservationList();
            }

        }

        //Fills list with reservations
        public void LoadReservationList()
        {
            ReservationsListView.ItemsSource = camping.Reservations.OrderBy(reservation => reservation.ArrivalDate).ThenBy(reservation => reservation.Place.PlaceNumber); //Takes reservations
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
                    MessageBoxResult result = MessageBox.Show("Weet je zeker dat je reservering " + reservationToDelete.ReservationNumber + "wil verwijderen?", "Waarschuwing!", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    // Check the users choice
                    if (result == MessageBoxResult.Yes)
                    {
                        // User clicked Yes, so delete the reservation
                        camping.Reservations.Remove(reservationToDelete);


                        // Refresh the ListView
                        LoadReservationList();
                    }
                    // If the user clicked No, do nothing
                }
            }

        }
    }
}
