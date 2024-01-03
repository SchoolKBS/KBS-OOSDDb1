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
            if (AreaColor.SelectedItem != null)
                SelectedArea.Color = (int)AreaColor.SelectedValue;
            else
            {
                StaticUIMethods.SetErrorComboBoxBorder(AreaColorBorder);
                _wrongInput = true;
            }
        }
        public void GetAddAreaValues()
        {
            _wrongInput = false;
            SelectedArea.Name = GetAddComponentNameTextbox(AreaName, _areaName);
            _areaOnMap.GetColorID();
            SelectedArea.AmountOfPeople = GetAddAmountOfPeople(AreaAmountOfPeople, _areaPersons);
            SelectedArea.PricePerNightPerPerson = GetAddPricePerNightPerPerson(AreaPrice, _areaPricePerNightPerPerson);
            SelectedArea.SurfaceArea = GetAddSurfaceArea(AreaPlaceSurfaceArea, _areaSurfaceArea);
            SelectedArea.Power = (bool)AreaPower.IsChecked;
            SelectedArea.Dogs = (bool)AreaDogs.IsChecked;
        }
        public void HandleAreaClick()
        {
            SetInfoVisible("Area");
            AreaName.Text = SelectedArea.Name;
            SetAreaComboBox();
            AreaPower.IsChecked = SelectedArea.Power;
            AreaDogs.IsChecked = SelectedArea.Dogs;
            if (SelectedArea.SurfaceArea > 0) AreaPlaceSurfaceArea.Text = SelectedArea.SurfaceArea.ToString();
            else AreaPlaceSurfaceArea.Text = "";
            if (SelectedArea.PricePerNightPerPerson > 0) AreaPrice.Text = SelectedArea.PricePerNightPerPerson.ToString();
            else AreaPrice.Text = "";
            if (SelectedArea.AmountOfPeople > 0) AreaAmountOfPeople.Text = SelectedArea.AmountOfPeople.ToString();
            else AreaAmountOfPeople.Text = "";
        }
        public void SetAreaComboBox()
        {
            AreaColor.Items.Clear();
            List<int> activeColors = _areas.Where(a => SelectedArea != null ? a.Color != SelectedArea.Color : true).Select(a => a.Color).ToList();
            List<int> possibelColors = Enumerable.Range(0, StaticUIMethods.ColorCount).Except(activeColors).ToList();
            for (int i = 0; i < possibelColors.Count; i++)
            {
                AreaColor.Items.Add(new { Text = StaticUIMethods.GetColorNameFromInt(possibelColors[i]), Value = possibelColors[i] });
            }
            if (SelectedArea != null) AreaColor.SelectedValue = SelectedArea.Color;
        }
        public void ToggleAreaInput(bool enabled)
        {
            AreaName.IsEnabled = enabled;
            AreaColor.IsEnabled = enabled;
            AreaPower.IsEnabled = enabled;
            AreaDogs.IsEnabled = enabled;
            AreaPlaceSurfaceArea.IsEnabled = enabled;
            AreaPrice.IsEnabled = enabled;
            AreaAmountOfPeople.IsEnabled = enabled;
            AddAreaButton.Visibility = Visibility.Hidden;
            if (enabled) AddAreaButton.Visibility = Visibility.Visible;
        }
        public void CalculateAreaSize(double xCord, double yCord)
        {
            if (xCord < _areaStartPoint.X) (xCord, _areaStartPoint.X) = (_areaStartPoint.X, xCord);
            if (yCord < _areaStartPoint.Y) (yCord, _areaStartPoint.Y) = (_areaStartPoint.Y, yCord);
            if (_areaStartPoint.X < 5) _areaStartPoint.X = 0;
            if (_areaStartPoint.Y < 5) _areaStartPoint.Y = 0;
            SelectedArea.XCord1 = (int)_areaStartPoint.X;
            SelectedArea.YCord1 = (int)_areaStartPoint.Y;
            SelectedArea.Width = (int)xCord - SelectedArea.XCord1;
            SelectedArea.Height = (int)yCord - SelectedArea.YCord1;
            if (_areaStartPoint.X + SelectedArea.Width > (int)field.Width - 5) SelectedArea.Width = (int)field.Width - SelectedArea.XCord1;
            if (_areaStartPoint.Y + SelectedArea.Height > (int)field.Height - 5) SelectedArea.Height = (int)field.Height - SelectedArea.YCord1;
        }
        public void SetAreaEvents(Border border, Area area)
        {
            border.MouseLeftButtonDown += (sender, e) =>
            {
                _areaOnMap.DeselectAllFields();
                if (_selectedMapButton.Contains("View"))
                {
                    SelectedArea = area;
                    SelectedStreet = null;
                    SelectedPlace = null;
                    _areaOnMap.ToggleAreaInput(false);
                    _areaOnMap.HandleAreaClick();
                    border.BorderBrush = Brushes.LightCyan;
                    border.BorderThickness = new Thickness(4);
                    HighLightPlaces(area, Brushes.DarkCyan);
                }
            };
        }
        public void GenerateAreaStartPoint(double xCord, double yCord)
        {
            _areaStartPoint = new Point(xCord, yCord);
            Ellipse ellipse = new Ellipse();
            Canvas.SetLeft(ellipse, xCord - 7.5);
            Canvas.SetTop(ellipse, yCord - 7.5);
            ellipse.Width = 15;
            ellipse.Height = 15;
            ellipse.Fill = Brushes.DarkGray;
            field.Children.Add(ellipse);
        }
        public void FieldMouseDownArea()
        {
            GenerateMap();
            Point point = Mouse.GetPosition(field);
            double xCord = Math.Round(point.X);
            double yCord = Math.Round(point.Y);
            if (_areaStartPoint.X == -1 && _areaStartPoint.Y == -1)
            {
                GenerateAreaStartPoint(xCord, yCord);
                ResetBorders(AreaInfoGrid);
            }
            else
            {
                SelectedArea = new Area();
                HandleAreaClick();
                CalculateAreaSize(xCord, yCord);
                Border border = MapPageArea.GenerateArea(SelectedArea);
                field.Children.Add(border);
                _newArea = border;
                ToggleAreaInput(true);
                SetInfoVisible("Area");
            }
        }
    }
}
