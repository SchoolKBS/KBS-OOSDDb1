using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for ReservationsOverviewWindow.xaml
    /// </summary>
    public partial class ReservationsOverviewWindow : Page
    {

        public static Camping Camping { get; set; }



        public ReservationsOverviewWindow()
        {
            InitializeComponent();

            if (Camping == null)
            {
                Camping = new Camping();
            }


            if (Camping.Reservations.Count() > 0) //Checks if reservations exist to load list
            {
                LoadReservationList();
            }
      
        }
        public void LoadReservationList() //Fills list with reservations
        {
            ReservationsListView.ItemsSource = Camping.Reservations.OrderBy(reservation => reservation.StartDate).ThenBy(reservation => reservation.place.PlaceNumber); //Takes reservations
        }



   
        public void DeleteButton_Click(object sender, RoutedEventArgs e) //Function to delete reservations
        {
            Button? button = sender as Button;

            //Button pressed?
            if (button != null)
            {
                Reservation? reservationToDelete = button.CommandParameter as Reservation; //Takes reservation


                if (reservationToDelete != null)
                {
                    // Show a confirmation dialog
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete Reservation " + reservationToDelete.ReservationNumber + "?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    // Check the users choice
                    if (result == MessageBoxResult.Yes)
                    {
                        // User clicked Yes, so delete the reservation
                        Camping.Reservations.Remove(reservationToDelete);

                        // Refresh the ListView
                        LoadReservationList();
                    }
                    // If the user clicked No, do nothing
                }
            }

        }
    }
}
