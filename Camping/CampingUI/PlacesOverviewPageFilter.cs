using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CampingUI
{
    public class PlacesOverviewPageFilter
    {
        public TextBox AmountOfPeopleTextBox, MaxPriceRangeTextBox;
        public DatePicker ArrivalDatePicker, DepartureDatePicker;
        public DateTime ArrivalDate, DepartureDate;
        public bool WrongInput, EmptyDates;
        public int AmountOfPeople;
        public double MaxPriceRange;
        public void SetAmountOfPeopleFromAmountOfPeopleTextBox(TextBox textbox, int number, bool wronginput)
        {
            int number1;
            bool filterBool = wronginput;
            if (!string.IsNullOrEmpty(textbox.Text))
            {
                if (int.TryParse(textbox.Text, out number1) && number1 >= 0)// Checks if int can be parsed and if number is bigger or equal to 0
                {
                    number = number1;
                }
                else
                {
                    StaticUIMethods.SetErrorTextboxBorder(textbox);
                    filterBool = true;
                }

            }
            else
            {
                number = 0;
                textbox.Text = $"{number}";
            }
            AmountOfPeople = number;
            WrongInput = filterBool;
        }

        public void SetMaxPriceFromMaxPriceRangeTextBox(TextBox textbox, double number, bool filterBool, Camping camping)
        {
            double number1;
            if (!string.IsNullOrEmpty(textbox.Text))
            {
                string MaxPriceRangeText = textbox.Text.Replace(".", ",");
                if (double.TryParse(MaxPriceRangeText, out number1) && number1 > 0)       // Checks if int can be parsed and if number is bigger or equal to 0
                {
                    number = number1;
                }
                else
                {
                    StaticUIMethods.SetErrorTextboxBorder(textbox);
                    filterBool = true;
                }

            }
            else
            {
                number = camping.GetPlaces().Max(i => i.PricePerNightPerPerson);
                textbox.Text = $"{number}";
            }
            MaxPriceRange = number;
            WrongInput = filterBool;
        }

        //Function to set the _arrivalDate and _departureDate
        public void SetArrivalAndDepartureDates(DatePicker arrivalDatePicker, DatePicker departureDatePicker, bool filterBool, bool emptyDates)
        {
            DateTime arrivalDate = GetDatePickerDate(arrivalDatePicker);
            DateTime departureDate = GetDatePickerDate(departureDatePicker);
            if (arrivalDate.Equals(DateTime.MaxValue.Date) && departureDate.Equals(DateTime.MinValue.Date))
            {
                emptyDates = true;
            }
            else
            {
                emptyDates = false;
                if (arrivalDate >= departureDate || arrivalDate.Date < DateTime.Now.Date)
                {
                    StaticUIMethods.SetErrorDatePickerBorder(arrivalDatePicker);
                    StaticUIMethods.SetErrorDatePickerBorder(departureDatePicker);
                    filterBool = true;
                }
            }
            WrongInput = filterBool;
            EmptyDates = emptyDates;
            ArrivalDate = arrivalDate;
            DepartureDate = departureDate;


        }
        // Function to get the Date entered in a DatePicker (ArrivalDatePicker or DepartureDatePicker)
        // Returns a date
        public DateTime GetDatePickerDate(DatePicker datePicker)
        {
            DateTime date;
            if (datePicker.SelectedDate.HasValue)
            {
                date = datePicker.SelectedDate.Value.Date;
            }
            else
            {
                int tagValue = int.Parse(datePicker.Tag.ToString());
                if (tagValue == -1) date = DateTime.MinValue.Date;
                else date = DateTime.MaxValue.Date;
            }
            return date;
            //Kijken in de reserveringen lijst op die specifieke plek, en kijken of de gekozen tijdperiode nog niet bestaat
        }
    }
}
