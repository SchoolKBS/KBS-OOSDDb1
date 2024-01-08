using CampingCore;
using CampingUI.Map.StreetMap;
using CampingUI.NewFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CampingUI.Map.AreaMap
{
    public class AreaOnMap : MapMethods
    {
        public AreaOnMap(MapPage mapPage, Camping camping) : base(mapPage, camping) { }
        public void GetColorID()
        {
            if (MapPage.AreaColor.SelectedItem != null)
                MapPage.SelectedArea.Color = (int)MapPage.AreaColor.SelectedValue;
            else
            {
                StaticUIMethods.SetErrorComboBoxBorder(MapPage.AreaColorBorder);
                MapPage.WrongInput = true;
            }
        }
        public void GetAddAreaValues()
        {
            MapPage.WrongInput = false;
            MapPage.SelectedArea.Name = GetAddComponentNameTextbox(MapPage.AreaNameTextbox, MapPage.AreaName);
            MapPage.AreaOnMap.GetColorID();
            MapPage.SelectedArea.AmountOfPeople = GetAddAmountOfPeople(MapPage.AreaAmountOfPeopleTextbox, MapPage.AreaAmountOfPeople);
            MapPage.SelectedArea.PricePerNightPerPerson = GetAddPricePerNightPerPerson(MapPage.AreaPricePerNightPerPersonTextbox, MapPage.AreaPricePerNightPerPerson);
            MapPage.SelectedArea.SurfaceArea = GetAddSurfaceArea(MapPage.AreaPlaceSurfaceAreaTextbox, MapPage.AreaSurfaceArea);
            MapPage.SelectedArea.Power = (bool)MapPage.AreaPowerCheckbox.IsChecked;
            MapPage.SelectedArea.Dogs = (bool)MapPage.AreaDogsCheckbox.IsChecked;
        }
        public void HandleAreaClick()
        {
            SetInfoVisible("Area");
            MapPage.AreaNameTextbox.Text = MapPage.SelectedArea.Name;
            SetAreaComboBox();
            MapPage.AreaPowerCheckbox.IsChecked = MapPage.SelectedArea.Power;
            MapPage.AreaDogsCheckbox.IsChecked = MapPage.SelectedArea.Dogs;
            if (MapPage.SelectedArea.SurfaceArea > 0) MapPage.AreaPlaceSurfaceAreaTextbox.Text = MapPage.SelectedArea.SurfaceArea.ToString();
            else MapPage.AreaPlaceSurfaceAreaTextbox.Text = "";
            if (MapPage.SelectedArea.PricePerNightPerPerson > 0) MapPage.AreaPricePerNightPerPersonTextbox.Text = MapPage.SelectedArea.PricePerNightPerPerson.ToString();
            else MapPage.AreaPricePerNightPerPersonTextbox.Text = "";
            if (MapPage.SelectedArea.AmountOfPeople > 0) MapPage.AreaAmountOfPeopleTextbox.Text = MapPage.SelectedArea.AmountOfPeople.ToString();
            else MapPage.AreaAmountOfPeopleTextbox.Text = "";
        }
        public void SetAreaComboBox()
        {
            MapPage.AreaColor.Items.Clear();
            List<int> activeColors = MapPage.Areas.Where(a => MapPage.SelectedArea != null ? a.Color != MapPage.SelectedArea.Color : true).Select(a => a.Color).ToList();
            List<int> possibelColors = Enumerable.Range(0, StaticUIMethods.ColorCount).Except(activeColors).ToList();
            for (int i = 0; i < possibelColors.Count; i++)
            {
                MapPage.AreaColor.Items.Add(new { Text = StaticUIMethods.GetColorNameFromInt(possibelColors[i]), Value = possibelColors[i] });
            }
            if (MapPage.SelectedArea != null) MapPage.AreaColor.SelectedValue = MapPage.SelectedArea.Color;
        }
        public void ToggleAreaInput(bool enabled)
        {
            MapPage.AreaNameTextbox.IsEnabled = enabled;
            MapPage.AreaColor.IsEnabled = enabled;
            MapPage.AreaPowerCheckbox.IsEnabled = enabled;
            MapPage.AreaDogsCheckbox.IsEnabled = enabled;
            MapPage.AreaPlaceSurfaceAreaTextbox.IsEnabled = enabled;
            MapPage.AreaPricePerNightPerPersonTextbox.IsEnabled = enabled;
            MapPage.AreaAmountOfPeopleTextbox.IsEnabled = enabled;
            MapPage.AddAreaButton.Visibility = Visibility.Hidden;
            if (enabled) MapPage.AddAreaButton.Visibility = Visibility.Visible;
        }
        public void CalculateAreaSize(double xCord, double yCord)
        {
            if (xCord < MapPage.AreaStartPoint.X) (xCord, MapPage.AreaStartPoint.X) = (MapPage.AreaStartPoint.X, xCord);
            if (yCord < MapPage.AreaStartPoint.Y) (yCord, MapPage.AreaStartPoint.Y) = (MapPage.AreaStartPoint.Y, yCord);
            if (MapPage.AreaStartPoint.X < 5) MapPage.AreaStartPoint.X = 0;
            if (MapPage.AreaStartPoint.Y < 5) MapPage.AreaStartPoint.Y = 0;
            MapPage.SelectedArea.XCord1 = (int)MapPage.AreaStartPoint.X;
            MapPage.SelectedArea.YCord1 = (int)MapPage.AreaStartPoint.Y;
            MapPage.SelectedArea.Width = (int)xCord - MapPage.SelectedArea.XCord1;
            MapPage.SelectedArea.Height = (int)yCord - MapPage.SelectedArea.YCord1;
            if (MapPage.AreaStartPoint.X + MapPage.SelectedArea.Width > (int)MapPage.field.Width - 5) MapPage.SelectedArea.Width = (int)MapPage.field.Width - MapPage.SelectedArea.XCord1;
            if (MapPage.AreaStartPoint.Y + MapPage.SelectedArea.Height > (int)MapPage.field.Height - 5) MapPage.SelectedArea.Height = (int)MapPage.field.Height - MapPage.SelectedArea.YCord1;
        }
        public void SetAreaEvents(Border border, Area area)
        {
            border.MouseLeftButtonDown += (sender, e) =>
            {
                MapPage.AreaOnMap.DeselectAllFields();
                if (MapPage.SelectedMapButton.Contains("View"))
                {
                    MapPage.SelectedArea = area;
                    MapPage.SelectedStreet = null;
                    MapPage.SelectedPlace = null;
                    ToggleAreaInput(false);
                    HandleAreaClick();
                    border.BorderBrush = Brushes.LightCyan;
                    border.BorderThickness = new Thickness(4);
                    HighLightPlaces(area, Brushes.DarkCyan);
                }
            };
        }
        public void GenerateAreaStartPoint(double xCord, double yCord)
        {
            MapPage.AreaStartPoint = new Point(xCord, yCord);
            Ellipse ellipse = new Ellipse();
            Canvas.SetLeft(ellipse, xCord - 7.5);
            Canvas.SetTop(ellipse, yCord - 7.5);
            ellipse.Width = 15;
            ellipse.Height = 15;
            ellipse.Fill = Brushes.DarkGray;
            MapPage.field.Children.Add(ellipse);
        }
        public void FieldMouseDownArea()
        {
            GenerateMap(MapPage.field);
            Point point = Mouse.GetPosition(MapPage.field);
            double xCord = Math.Round(point.X);
            double yCord = Math.Round(point.Y);
            if (MapPage.AreaStartPoint.X == -1 && MapPage.AreaStartPoint.Y == -1)
            {
                GenerateAreaStartPoint(xCord, yCord);
                ResetBorders(MapPage.AreaInfoGrid);
            }
            else
            {
                MapPage.SelectedArea = new Area();
                HandleAreaClick();
                CalculateAreaSize(xCord, yCord);
                Border border = MapPageArea.GenerateArea(MapPage.SelectedArea);
                MapPage.field.Children.Add(border);
                MapPage.NewArea = border;
                ToggleAreaInput(true);
                SetInfoVisible("Area");
            }
        }
    }
}
