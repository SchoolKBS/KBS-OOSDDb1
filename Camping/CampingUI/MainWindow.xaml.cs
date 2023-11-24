using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampingDataAccess;
using System.Windows;

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
        }

        //Function (EventHandler) to open the reservations page
        private void ReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new ReservationsOverviewWindow(Camping, CampingRepository);
        }

        //Function (EventHandler) to open the places overview page
        private void PlacesButton_Click(object sender, RoutedEventArgs e)
        {

            Main.Content = new PlacesOverviewPage(Camping, CampingRepository);
        }
    }


}

