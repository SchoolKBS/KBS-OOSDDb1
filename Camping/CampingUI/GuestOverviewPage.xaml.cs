using CampingCore;
using CampingDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public Camping Camping { get; set; }
        Guest guest;
        public GuestOverviewPage(Camping camping)
        {
            InitializeComponent();

            this.Camping = camping;

            //Get all guests from database
            GuestOverviewItemsControl.ItemsSource = camping.CampingRepository.CampingGuestRepository.GetGuests();
        }

        public void FilterOnGuestName_Click(object sender, RoutedEventArgs e)
        {
            string FirstName = GuestFirstNameTextBox.Text;
            string LastName = GuestLastNameTextBox.Text;

            //Checks which fields are used to search a guest
            if(string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                GuestOverviewItemsControl.ItemsSource = Camping.CampingRepository.CampingGuestRepository.GetGuests().Where(guest => guest.LastName.Contains(LastName, StringComparison.OrdinalIgnoreCase));

            } else if (!string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
            {
                GuestOverviewItemsControl.ItemsSource = Camping.CampingRepository.CampingGuestRepository.GetGuests().Where(guest => guest.FirstName.Contains(FirstName, StringComparison.OrdinalIgnoreCase));

            } else if(!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                GuestOverviewItemsControl.ItemsSource = Camping.CampingRepository.CampingGuestRepository.GetGuests().Where(guest => guest.FirstName.Contains(FirstName, StringComparison.OrdinalIgnoreCase) && guest.LastName.Contains(LastName, StringComparison.OrdinalIgnoreCase));

            } else if(string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
            {
                GuestOverviewItemsControl.ItemsSource = Camping.CampingRepository.CampingGuestRepository.GetGuests();
            }
        }

        public void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            GuestFirstNameTextBox.Text = "";
            GuestLastNameTextBox.Text = "";

            GuestOverviewItemsControl.ItemsSource = Camping.CampingRepository.CampingGuestRepository.GetGuests();
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
       
        //Changes guest information and saves it in database
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


            Camping.CampingRepository.CampingGuestRepository.UpdateGuest(guest);
            GuestOverviewItemsControl.ItemsSource = Camping.CampingRepository.CampingGuestRepository.GetGuests();
            ChangeGuestInformationGrid.Visibility = Visibility.Collapsed;
            GuestDetailsGrid.Visibility = Visibility.Collapsed;

            GuestOverviewItemsControl.ItemsSource = Camping.CampingRepository.CampingGuestRepository.GetGuests();
        }

        public void CancelNewGuestInformation(object sender, RoutedEventArgs e)
        {
            ChangeGuestInformationGrid.Visibility = Visibility.Collapsed;
            GuestDetailsGrid.Visibility = Visibility.Visible;
        }

        //Checks if PostalCode is valid
        private void PostalCodeTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(ChangePostalCodeTextBox.Text, @"^\d{4}\s?[A-Za-z]{2}$"))
            {
                InputNotification.Text = "";
                ConfirmEditButton.IsEnabled = true;
            }
            else
            {
                InputNotification.Text = "Voer een geldige postcode in.";
                ConfirmEditButton.IsEnabled = false;
            }
        }

        //Checks if phonenumber is digits
        private void PhoneNumberTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(ChangePhoneNumberTextBox.Text, @"^\d+$") && !string.IsNullOrEmpty(ChangePhoneNumberTextBox.Text))
            {
                InputNotification.Text = "";
                ConfirmEditButton.IsEnabled = true;
            }
            else
            {
                InputNotification.Text = "Het telefoonnummer is niet geldig";
                ConfirmEditButton.IsEnabled = false;
            }
        }

        private void NameTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(ChangeFirstNameTextBox.Text) || string.IsNullOrEmpty(ChangeLastNameTextBox.Text))
            {
                InputNotification.Text = "Voer een voornaam en achternaam in";
                ConfirmEditButton.IsEnabled = false;

            }
            else
            {
                InputNotification.Text = "";
                ConfirmEditButton.IsEnabled = true;
            }
        }
        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterOnGuestName_Click(sender, e);
            }
        }
    }
}