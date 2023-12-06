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
        Guest guest;
        public GuestOverviewPage()
        {
            InitializeComponent();

            //Get all guests from database
            GuestOverviewItemsControl.ItemsSource = sql.GetGuests();
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
            guest = (Guest)GuestOverviewItemsControl.SelectedItem;
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
                GuestPostalCodeTextBlock.Text = "Postcode:  " + guest.PostalCode;
                GuestEmailTextBlock.Text = "Email:  " + guest.Email;
                GuestPhoneNumberTextBlock.Text = "Telefoonnummer:  " + guest.PhoneNumber;

                ChangeGuestInformationGrid.Visibility = Visibility.Collapsed;
                GuestDetailsGrid.Visibility = Visibility.Visible;
            }
        }
        public void ChangeGuestInformation(object sender, RoutedEventArgs e)
        {
            GuestDetailsGrid.Visibility = Visibility.Collapsed;
            ChangeFirstNameTextBox.Text = guest.FirstName;
            ChangeInfixTextBox.Text = guest.Infix;
            ChangeLastNameTextBox.Text = guest.LastName;
            ChangeCityTextBox.Text = guest.City;
            ChangePostalCodeTextBox.Text = guest.PostalCode;
            ChangeAddressTextBox.Text = guest.Address;
            ChangeEmailTextBox.Text = guest.Email;
            ChangePhoneNumberTextBox.Text = guest.PhoneNumber;
            ChangeGuestInformationGrid.Visibility = Visibility.Visible;
        }

        public void SaveNewGuestInformation(object sender, RoutedEventArgs e)
        {
            guest.FirstName = ChangeFirstNameTextBox.Text;
            guest.Infix = ChangeInfixTextBox.Text;
            guest.LastName = ChangeLastNameTextBox.Text;
            guest.City = ChangeCityTextBox.Text;
            guest.PostalCode = ChangePostalCodeTextBox.Text;
            guest.Address = ChangeAddressTextBox.Text;
            guest.Email = ChangeEmailTextBox.Text;
            guest.PhoneNumber = ChangePhoneNumberTextBox.Text;
           

            sql.UpdateGuest(guest);
            GuestOverviewItemsControl.ItemsSource = sql.GetGuests();
            ChangeGuestInformationGrid.Visibility = Visibility.Collapsed;
            GuestDetailsGrid.Visibility = Visibility.Collapsed;
        }

        public void CancelNewGuestInformation(object sender, RoutedEventArgs e)
        {
            ChangeGuestInformationGrid.Visibility = Visibility.Collapsed;
            GuestDetailsGrid.Visibility = Visibility.Collapsed;
        }
    }
}