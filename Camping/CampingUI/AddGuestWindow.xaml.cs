﻿using CampingCore;
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
using System.Windows.Shapes;
using System.Xml.Schema;

namespace CampingUI
{

    public partial class AddGuestWindow : Page
    {
        public AddGuestWindow()
        {
            InitializeComponent();
        }

        public void AddGuestOnClick(object sender, RoutedEventArgs e)
        {
            if (!Email.Text.Contains("@") && !Email.Text.Contains("."))
            {
                notification.Text = "Ongeldig emailadres";
            }
            else
            {
                string[] TextInput =
                {
                    FirstName.Text,
                    LastName.Text,
                    Address.Text,
                    City.Text,
                    Email.Text,
                    PhoneNumber.Text,
                };

                CheckIfInputIsValid(TextInput);
            }
        }

        public bool CheckIfInputIsValid(String[] TextInput) 
        {
            foreach(string input in TextInput)
            {
                if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                {
                    notification.Text = "Vul alle velden in";
                    return false;
                }
                else
                {
                    notification.Text = "akkoord";
                }
            }
            return true;
        }

     
    }
}
