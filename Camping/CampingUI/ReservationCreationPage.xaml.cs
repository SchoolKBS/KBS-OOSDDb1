using CampingCore;
using CampingDataAccess;
using Microsoft.IdentityModel.Tokens;
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
    /// Interaction logic for ReservationCreationPage.xaml
    /// </summary>
    public partial class ReservationCreationPage : Page
    {
        private PlacesOverviewPage _page;
        private Camping _camping;
        private Place _place;
        public double Price {  get; set; }
        public ReservationCreationPage(PlacesOverviewPage placesOverviewPage)
        {
            InitializeComponent();
            _page = placesOverviewPage;
            _camping = _page.GetCamping();
            _place = (Place) _page.PlacesListView.SelectedItem;
            ShowAvailableDatesArrival();
            DepartureDatePicker.IsEnabled = false;
            SetKnownInformation();
        }
        // Function to set known information from filter.
        public void SetKnownInformation()
        {
            _place = (Place)_page.PlacesListView.SelectedItem;
            if (_page.GetFilterApplied())
            {
                if(_page.ArrivalDatePicker.SelectedDate != null) ArrivalDatePicker.SelectedDate = _page.ArrivalDatePicker.SelectedDate.Value;
                if (_page.DepartureDatePicker.SelectedDate != null)
                {
                    DepartureDatePicker.IsEnabled = true;
                    DepartureDatePicker.SelectedDate = _page.DepartureDatePicker.SelectedDate.Value;
                }
                if(_page.GetCampingAmountOfPeople() > 0)PeopleCountText.Text = _page.GetCampingAmountOfPeople().ToString();
            }
        }
        // Event for when arrival date is changed, This enables deparutre date and calls ShowAvailableDatesDeparture()
        private void ArrivalDatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            DepartureDatePicker.SelectedDate = null;
            ArrivalDatePicker.BorderBrush = Brushes.White;
            DepartureDatePicker.BorderBrush = Brushes.White;
            ArrivalDatePicker.BorderThickness = new Thickness(1,1,1,1);
            DepartureDatePicker.BorderThickness = new Thickness(1,1,1,1);


            this.ArrivalDatePicker.Text = ArrivalDatePicker.SelectedDate.ToString();
            ShowAvailableDatesDeparture();

        }
        // Sets Backgroud back to null if date is changed
        private void DepartureDatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            DepartureDatePicker.BorderBrush = Brushes.White;
            DepartureDatePicker.BorderThickness = new Thickness(1, 1, 1, 1);
            this.DepartureDatePicker.Text = DepartureDatePicker.SelectedDate.ToString();
            Price = CalcPrice();
            PriceLabel.Content = Price.ToString() + "€";
        }
        //Calcultes the price for the reservation based on the input fields PeopleCountText and the datepickers. Returns last (correct) price if input is incorrect
        private double CalcPrice()
        {
            int.TryParse(PeopleCountText.Text, out var TextToInt);
            if (TextToInt.GetType() == typeof(int) && TextToInt > 0 && TextToInt <= _place.AmountOfPeople && ArrivalDatePicker.SelectedDate.HasValue && DepartureDatePicker.SelectedDate.HasValue)
            {
                int dayscount = (int) DepartureDatePicker.SelectedDate.Value.Subtract(ArrivalDatePicker.SelectedDate.Value).TotalDays;
                PeopleCountText.BorderBrush = Brushes.White;
                PeopleCountText.BorderThickness = new Thickness(1,1,1,1);
                
                return _place.PricePerNightPerPerson * TextToInt * dayscount;
            }
            else
            {
                PeopleCountText.BorderBrush = Brushes.Red;
                PeopleCountText.BorderThickness = new Thickness(3, 3, 3, 3);
                return 0;
            }
        }
        // A method that calls the different Check methods to see if a reservation can be made
        public bool CheckValues()
        {
            bool result = true;
            if(!CheckDates() && result) result = false;
            if(!CheckPeopleCount() && result) result = false;
            if(!CheckGuestInputFields() && result) result = false;
            return result;
        }
        //Checks if the dates are selected
        public bool CheckDates()
        {
            if (!ArrivalDatePicker.SelectedDate.HasValue)
            {
                ArrivalDatePicker.BorderBrush = Brushes.Red;
                ArrivalDatePicker.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            if (!DepartureDatePicker.SelectedDate.HasValue)
            {
                DepartureDatePicker.BorderBrush = Brushes.Red;
                DepartureDatePicker.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            return ArrivalDatePicker.SelectedDate.HasValue && DepartureDatePicker.SelectedDate.HasValue;
        }
        // Checks if people count is a viable number
        public bool CheckPeopleCount()
        {
            bool result = int.TryParse(PeopleCountText.Text, out int number);
            if (!result)
            {
                PeopleCountText.BorderBrush = Brushes.Red;
                PeopleCountText.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            else if (number > _place.AmountOfPeople)
            {
                result = false;;
                PeopleCountText.BorderBrush = Brushes.Red;
                PeopleCountText.BorderThickness = new Thickness(3, 3, 3,3);
            }
            return result;
        }
        // Checks if the First name, Last name, and phonenumber are filled in
        public bool CheckGuestInputFields()
        {
            if (FirstNameTB.Text.IsNullOrEmpty())
            {
                FirstNameTB.BorderBrush = Brushes.Red;
                FirstNameTB.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            if (LastnameTB.Text.IsNullOrEmpty())
            {
                LastnameTB.BorderBrush = Brushes.Red;
                LastnameTB.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            if (PhoneNumberTB.Text.IsNullOrEmpty())
            {
                PhoneNumberTB.BorderBrush = Brushes.Red;
                PhoneNumberTB.BorderThickness = new Thickness(3, 3, 3, 3);
            }
                return !FirstNameTB.Text.IsNullOrEmpty() && !LastnameTB.Text.IsNullOrEmpty() && !PhoneNumberTB.Text.IsNullOrEmpty();
        }
        // Changes the PriceTV each time a different people count was entered
        private void PeopleCountText_Changed(object sender, TextChangedEventArgs e)
        {
            Price = CalcPrice();
            PriceLabel.Content = Price.ToString() + "$";

        }
        // Sets the possible Arrivaldates in the ArrivalDatePicker
        private void ShowAvailableDatesArrival()
        {
            ArrivalDatePicker.DisplayDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            ArrivalDatePicker.BlackoutDates.AddDatesInPast();
            var reservations = _camping.Reservations.Where(r => r.PlaceID == _place.PlaceID && r.DepartureDate > DateTime.Now).ToList();
            foreach( var reservation in reservations )
            {
                ArrivalDatePicker.BlackoutDates.Add(new CalendarDateRange(reservation.ArrivalDate, reservation.DepartureDate));
            }
        }
        // Sets the possible DepartureDates in the DepartureDatePicker
        private void ShowAvailableDatesDeparture()
        {
            if (ArrivalDatePicker.SelectedDate != null)
            {
                DepartureDatePicker.DisplayDateEnd = null;
                DepartureDatePicker.BlackoutDates.Clear();
                DateTime arrivalDate = (DateTime) ArrivalDatePicker.SelectedDate;
                DepartureDatePicker.IsEnabled = true;
                DepartureDatePicker.DisplayDateStart = new DateTime(arrivalDate.Year, arrivalDate.Month, 1);
                DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(DepartureDatePicker.DisplayDateStart.Value, arrivalDate));
                var reservations = _camping.Reservations.Where(r => r.PlaceID == _place.PlaceID && r.ArrivalDate > DateTime.Today).ToList();
                if (reservations.Count > 0  && reservations.Min(r => r.ArrivalDate) > ArrivalDatePicker.SelectedDate)
                {
                    var SoonestReservationStart = reservations.Min(r => r.ArrivalDate);
                    DepartureDatePicker.DisplayDateEnd = new DateTime(SoonestReservationStart.Year, SoonestReservationStart.Month, DateTime.DaysInMonth(SoonestReservationStart.Year, SoonestReservationStart.Month));
                    DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(SoonestReservationStart, DepartureDatePicker.DisplayDateEnd.Value));
                }

            }
        }
        // Cancels the reservation, goes back to last page
        private void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        // Accepts the reservation, checks if values are correct, creates a reservation and guest, and goes to a new place overview page
        private void AcceptReservation_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValues())
            {
                Guest guest = new Guest(FirstNameTB.Text, InfixTB.Text, LastnameTB.Text, AddressTB.Text, CityTB.Text, EmailTB.Text, PhoneNumberTB.Text, PostalCodeTB.Text);
                _camping.CampingRepository.CampingGuestRepository.AddGuest(guest);
                //Database db = new Database();
                //db.AddGuestToDatabase(guest);
                _camping.CampingRepository.CampingReservationRepository.AddReservation(new Reservation(0, (DateTime)ArrivalDatePicker.SelectedDate, (DateTime)DepartureDatePicker.SelectedDate, _place.PlaceID, _camping.CampingRepository.CampingGuestRepository.GetLastGuestID(), int.Parse(PeopleCountText.Text), IsPaidCB.IsChecked.Value, Price));
                NavigationService.Navigate(new PlacesOverviewPage(_camping, (CampingRepository)_camping.CampingRepository));
            }
        }
        // Changes the background of textbox back to normal if its value was changed
        private void Input_Changed(object sender, TextChangedEventArgs e)
        {
            if(sender.GetType() == typeof(TextBox)) {
                TextBox textbox = (TextBox)sender;
                if (textbox.BorderBrush.Equals(Brushes.Red))
                {
                    textbox.BorderThickness = new Thickness(1, 1, 1, 1);
                    textbox.BorderBrush = Brushes.White;
                }

            }
        }
    }
}
