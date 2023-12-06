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

        public void GuestSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Guest guest = (Guest)GuestOverviewItemsControl.SelectedItem;
            //Makes sure a guest is selected
            if(guest != null)
            {
                //Cheks if guest has an infix
                if (guest.Infix.Length < 1)
                {
                    GuestFullNameTextBlock.Text = "Naam:  " + guest.FirstName + " " + guest.LastName;

                }
                else
                {
                    GuestFullNameTextBlock.Text = "Naam:  " + guest.FirstName + " " + guest.Infix + " " + guest.LastName;
                }

                //Texboxes display guest data
                GuestIdTextblock.Text = "Gast  " + guest.GuestID;
                GuestCityTextBlock.Text = "Woonplaats:  " + guest.City;
                GuestAddressTextBlock.Text = "Adres:  " + guest.Address;
                GuestEmailTextBlock.Text = "Email:  " + guest.Email;
                GuestPhoneNumberTextBlock.Text = "Telefoonnummer:  " + guest.PhoneNumber;
                GuestDetailsGrid.Visibility = Visibility.Visible;
            }
        }
    }
}