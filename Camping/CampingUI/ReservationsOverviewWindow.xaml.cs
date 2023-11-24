using CampingCore;
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

        public ReservationsOverviewWindow(Camping camping)
        {
            InitializeComponent();
            _camping = camping;
            ListBox listbox = new ListBox(); // Creates a ListBox to show in the WPF UI
            Grid.SetRow(listbox, 1);        // Adds a new row to the ListBox.
            listbox.ItemsSource = _camping.Reservations.OrderBy(reservation => reservation.ArrivalDate).ThenBy(reservation => reservation.PlaceID);   // For all items in the ListBox use the camping places.
            ListGrid.Children.Add(listbox);     // Adds each items inside the listBox to the grid UI.
            

            //Checks if reservations exist to load list.
            if (_camping.Reservations.Count() > 0) 
            {
                LoadReservationList();
            }
      
        }

        //Fills list with reservations
        public void LoadReservationList() 
        {
            ReservationsListView.ItemsSource = _camping.Reservations.OrderBy(reservation => reservation.ArrivalDate).ThenBy(reservation => reservation.PlaceID); //Takes reservations
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
                        _camping.Reservations.Remove(reservationToDelete);

                        // Refresh the ListView
                        LoadReservationList();
                    }
                    // If the user clicked No, do nothing
                }
            }

        }
    }
}
