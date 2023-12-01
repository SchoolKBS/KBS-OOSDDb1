using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampingDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public CampingRepository CampingRepository { get; private set; }
        public Camping Camping { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Main.Navigate(new MainPage());
            this.CampingRepository = new CampingRepository();
            this.Camping = new Camping(CampingRepository);

        }


        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main screen or set the desired page
            Main.Navigate(new MainPage()); // Replace MainPage with the appropriate page class for your main screen
        }

        //Function (EventHandler) to open the reservations page
        private void ReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            Camping.Reservations = Camping.CampingRepository.GetReservations();
            Main.Content = new ReservationsOverviewWindow(Camping, CampingRepository);
        }

        //Function (EventHandler) to open the places overview page
        private void PlacesButton_Click(object sender, RoutedEventArgs e)
        {

            Main.Content = new PlacesOverviewPage(Camping, CampingRepository);
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Custom animation for closing the window
            var animation = new System.Windows.Media.Animation.DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
            animation.Completed += (s, _) => Close();
            BeginAnimation(OpacityProperty, animation);
        }

    }


}

