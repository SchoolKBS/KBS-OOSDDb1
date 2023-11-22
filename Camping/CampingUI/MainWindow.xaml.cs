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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Camping camping { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.camping = new Camping();
        }

        private void BtnClickReservations(object sender, RoutedEventArgs e)
        {
            Main.Content = new ReservationsOverviewWindow(); 
        }

        private void BtnClickPlaces(object sender, RoutedEventArgs e)
        {
            Main.Content = new PlacesOverviewPage();     
        }
    }


}
