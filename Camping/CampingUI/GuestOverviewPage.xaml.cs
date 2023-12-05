using CampingCore;
using CampingDataAccess;
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
    public partial class GuestOverviewPage : Page
    {
        SqliteRepository sql = new SqliteRepository();
        public GuestOverviewPage()
        {
            InitializeComponent();

            //Get all guests from database
            GuestOverviewItemsControl.ItemsSource = sql.GetGuests();
        }

        public void SelectGuestByID(int GuestID)
        {
            sql.GetGuestFromGuestID(GuestID);
        }

        public void FilterOnGuestName_Click(object sender, RoutedEventArgs e)
        {
            string FirstName = GuestFirstNameTextBox.Text;
            string LastName = GuestLastNameTextBox.Text;

            //Checks which fields are used to search a guest
            if(string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                GuestOverviewItemsControl.ItemsSource = sql.GetGuestsByLastName(LastName);

            } else if (!string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
            {
                GuestOverviewItemsControl.ItemsSource = sql.GetGuestsByFirstName(FirstName);

            } else if(!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                GuestOverviewItemsControl.ItemsSource = sql.GetGuestsByFirstAndLastName(FirstName, LastName);
            }
        }
    }
}