using CampingCore;
using System.Linq;
using System.Windows.Controls;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for ReservationsOverviewWindow.xaml
    /// </summary>
    public partial class ReservationsOverviewWindow : Page
    {
        public Camping Camping { get; set; }

        public ReservationsOverviewWindow(Camping camping)
        {
            InitializeComponent();
            this.Camping = camping; // Creates a camping.

            ListBox listbox = new ListBox(); // Creates a ListBox to show in the WPF UI
            Grid.SetRow(listbox, 1);        // Adds a new row to the ListBox.
            listbox.ItemsSource = Camping.Reservations.OrderBy(reservation => reservation.ArrivalDate).ThenBy(reservation => reservation.PlaceID);   // For all items in the ListBox use the camping places.
            ListGrid.Children.Add(listbox);     // Adds each items inside the listBox to the grid UI.

        }
    }
}
