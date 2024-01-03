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
                MapPage._wrongInput = true;
            }
        }
        public void GetAddAreaValues()
        {
            MapPage._wrongInput = false;
            MapPage.SelectedArea.Name = GetAddComponentNameTextbox(MapPage.AreaName, MapPage._areaName);
            MapPage._areaOnMap.GetColorID();
            MapPage.SelectedArea.AmountOfPeople = GetAddAmountOfPeople(MapPage.AreaAmountOfPeople, MapPage._areaPersons);
            MapPage.SelectedArea.PricePerNightPerPerson = GetAddPricePerNightPerPerson(MapPage.AreaPrice, MapPage._areaPricePerNightPerPerson);
            MapPage.SelectedArea.SurfaceArea = GetAddSurfaceArea(MapPage.AreaPlaceSurfaceArea, MapPage._areaSurfaceArea);
            MapPage.SelectedArea.Power = (bool)MapPage.AreaPower.IsChecked;
            MapPage.SelectedArea.Dogs = (bool)MapPage.AreaDogs.IsChecked;
        }
        public void HandleAreaClick()
        {
            SetInfoVisible("Area");
            MapPage.AreaName.Text = MapPage.SelectedArea.Name;
            SetAreaComboBox();
            MapPage.AreaPower.IsChecked = MapPage.SelectedArea.Power;
            MapPage.AreaDogs.IsChecked = MapPage.SelectedArea.Dogs;
            if (MapPage.SelectedArea.SurfaceArea > 0) MapPage.AreaPlaceSurfaceArea.Text = MapPage.SelectedArea.SurfaceArea.ToString();
            else MapPage.AreaPlaceSurfaceArea.Text = "";
            if (MapPage.SelectedArea.PricePerNightPerPerson > 0) MapPage.AreaPrice.Text = MapPage.SelectedArea.PricePerNightPerPerson.ToString();
            else MapPage.AreaPrice.Text = "";
            if (MapPage.SelectedArea.AmountOfPeople > 0) MapPage.AreaAmountOfPeople.Text = MapPage.SelectedArea.AmountOfPeople.ToString();
            else MapPage.AreaAmountOfPeople.Text = "";
        }
        public void SetAreaComboBox()
        {
            MapPage.AreaColor.Items.Clear();
            List<int> activeColors = MapPage._areas.Where(a => MapPage.SelectedArea != null ? a.Color != MapPage.SelectedArea.Color : true).Select(a => a.Color).ToList();
            List<int> possibelColors = Enumerable.Range(0, StaticUIMethods.ColorCount).Except(activeColors).ToList();
            for (int i = 0; i < possibelColors.Count; i++)
            {
                MapPage.AreaColor.Items.Add(new { Text = StaticUIMethods.GetColorNameFromInt(possibelColors[i]), Value = possibelColors[i] });
            }
            if (MapPage.SelectedArea != null) MapPage.AreaColor.SelectedValue = MapPage.SelectedArea.Color;
        }
        public void ToggleAreaInput(bool enabled)
        {
            MapPage.AreaName.IsEnabled = enabled;
            MapPage.AreaColor.IsEnabled = enabled;
            MapPage.AreaPower.IsEnabled = enabled;
            MapPage.AreaDogs.IsEnabled = enabled;
            MapPage.AreaPlaceSurfaceArea.IsEnabled = enabled;
            MapPage.AreaPrice.IsEnabled = enabled;
            MapPage.AreaAmountOfPeople.IsEnabled = enabled;
            MapPage.AddAreaButton.Visibility = Visibility.Hidden;
            if (enabled) MapPage.AddAreaButton.Visibility = Visibility.Visible;
        }
        public void CalculateAreaSize(double xCord, double yCord)
        {
            if (xCord < MapPage._areaStartPoint.X) (xCord, MapPage._areaStartPoint.X) = (MapPage._areaStartPoint.X, xCord);
            if (yCord < MapPage._areaStartPoint.Y) (yCord, MapPage._areaStartPoint.Y) = (MapPage._areaStartPoint.Y, yCord);
            if (MapPage._areaStartPoint.X < 5) MapPage._areaStartPoint.X = 0;
            if (MapPage._areaStartPoint.Y < 5) MapPage._areaStartPoint.Y = 0;
            MapPage.SelectedArea.XCord1 = (int)MapPage._areaStartPoint.X;
            MapPage.SelectedArea.YCord1 = (int)MapPage._areaStartPoint.Y;
            MapPage.SelectedArea.Width = (int)xCord - MapPage.SelectedArea.XCord1;
            MapPage.SelectedArea.Height = (int)yCord - MapPage.SelectedArea.YCord1;
            if (MapPage._areaStartPoint.X + MapPage.SelectedArea.Width > (int)MapPage.field.Width - 5) MapPage.SelectedArea.Width = (int)MapPage.field.Width - MapPage.SelectedArea.XCord1;
            if (MapPage._areaStartPoint.Y + MapPage.SelectedArea.Height > (int)MapPage.field.Height - 5) MapPage.SelectedArea.Height = (int)MapPage.field.Height - MapPage.SelectedArea.YCord1;
        }
        public void SetAreaEvents(Border border, Area area)
        {
            border.MouseLeftButtonDown += (sender, e) =>
            {
                MapPage._areaOnMap.DeselectAllFields();
                if (MapPage._selectedMapButton.Contains("View"))
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
            MapPage._areaStartPoint = new Point(xCord, yCord);
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
            GenerateMap();
            Point point = Mouse.GetPosition(MapPage.field);
            double xCord = Math.Round(point.X);
            double yCord = Math.Round(point.Y);
            if (MapPage._areaStartPoint.X == -1 && MapPage._areaStartPoint.Y == -1)
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
                MapPage._newArea = border;
                ToggleAreaInput(true);
                SetInfoVisible("Area");
            }
        }
    }
}
