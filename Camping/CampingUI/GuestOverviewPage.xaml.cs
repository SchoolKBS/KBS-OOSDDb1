﻿using CampingCore;
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
    /// <summary>
    /// Interaction logic for GuestOverviewPage.xaml
    /// </summary>
    public partial class GuestOverviewPage : Page
    {
        SqliteRepository sql = new SqliteRepository();
        public GuestOverviewPage()
        {
            InitializeComponent();

            GuestOverviewItemsControl.ItemsSource = sql.GetGuests();
        }
    }
}