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
    /// Interaction logic for GuestOverviewPage.xaml
    /// </summary>
    public partial class GuestOverviewPage : Page
    {
        public GuestOverviewPage()
        {
            InitializeComponent();

            var Gastenlijst = new List<Guest> { };

            Guest persoon1 = new Guest(1, "Hannelore", "", "baarssen", "smederij", "dronten", "", "", "");

            Gastenlijst.Add(persoon1);

            GuestOverviewItemsControl.ItemsSource = Gastenlijst;
        }
    }
}