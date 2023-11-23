using System.Windows;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Function (EventHandler) to open the reservations page
        private void ReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new ReservationsOverviewWindow();
        }

        //Function (EventHandler) to open the places overview page
        private void PlacesButton_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new PlacesOverviewPage();
        }
    }


}
