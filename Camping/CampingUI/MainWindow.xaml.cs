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
using System.IO;

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

            this.CampingRepository = new CampingRepository();

            this.Camping = new Camping(CampingRepository);

            Main.Navigate(new MapPage(Camping));

        }
        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main screen or set the desired page
            Main.Navigate(new MapPage(Camping)); // Replace MainPage with the appropriate page class for your main screen
        }
        private void ReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new ReservationsOverviewWindow(Camping, CampingRepository);
        }
        private void PlacesButton_Click(object sender, RoutedEventArgs e)
        {

            Main.Content = new PlacesOverviewPage(Camping, CampingRepository);

        }
        private void GuestOverviewButton_Click(Object sender, RoutedEventArgs e)
        {
            Main.Content = new GuestOverviewPage(Camping);
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var animation = new System.Windows.Media.Animation.DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
            animation.Completed += (s, _) => Close();
            BeginAnimation(OpacityProperty, animation);
        }


    }


}

