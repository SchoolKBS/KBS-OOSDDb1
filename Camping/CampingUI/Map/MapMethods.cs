﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.IO.Packaging;
using CampingUI.Map.PlaceMap;
using CampingCore;
using CampingUI.GenerateComponentsMap;
using CampingUI.Map.AreaMap;
using CampingUI.Map.StreetMap;
using CampingUI.NewFolder;

namespace CampingUI.Map
{
    public class MapMethods //: MapPage
    {
        protected MapPage MapPage { get; private set; }
        private Camping _camping;
        public MapMethods(MapPage mapPage, Camping camping) //: base(camping)
        {
            MapPage = mapPage;
            _camping = camping;

        }
        public void DeselectAllFields()
        {
            foreach (var comp in MapPage.field.Children)
            {
                if (comp is Line line)
                    line.Stroke = Brushes.Black;
                if (comp is Border borderArea && borderArea.Name.Contains("Canvas"))
                {
                    borderArea.BorderBrush = Brushes.Black;
                    borderArea.BorderThickness = new Thickness(1);
                }
                if (comp is Border borderPlace && borderPlace.Name.Contains("Place") && borderPlace.Child is Canvas canvasPlace)
                    canvasPlace.Background = Brushes.Black;
            }
        }
        public void SetInfoVisible(string component)
        {
            SetInfoGridsInvisible();
            if (component.Equals("Place"))
            {
                MapPage.PlaceInfo.Visibility = Visibility.Visible;
                MapPage.PlaceInfoGrid.Visibility = Visibility.Visible;
            }
            else if (component.Equals("Street"))
            {
                MapPage.StreetInfo.Visibility = Visibility.Visible;
                MapPage.StreetInfoGrid.Visibility = Visibility.Visible;
            }
            else if (component.Equals("Area"))
            {
                MapPage.AreaInfo.Visibility = Visibility.Visible;
                MapPage.AreaInfoGrid.Visibility = Visibility.Visible;
            }
        }
        public void SetInfoGridsInvisible()
        {
            MapPage.PlaceInfo.Visibility = Visibility.Visible;
            MapPage.StreetInfo.Visibility = Visibility.Hidden;
            MapPage.AreaInfo.Visibility = Visibility.Hidden;
            MapPage.PlaceInfoGrid.Visibility = Visibility.Hidden;
            MapPage.StreetInfoGrid.Visibility = Visibility.Hidden;
            MapPage.AreaInfoGrid.Visibility = Visibility.Hidden;
        }
        public void ResetBorders(Grid grid)
        {
            foreach (var component in grid.Children)
            {
                if (component is TextBox textbox)
                    StaticUIMethods.ResetTextboxBorder(textbox);
                else if (component is Border border)
                    StaticUIMethods.ResetComboBoxBorder(border);
                else if (component is Grid nestedGrid)
                    ResetBorders(nestedGrid);
            }
        }
        public void ResetInputs(Grid grid)
        {
            foreach (var component in grid.Children)
            {
                if (component is TextBox textbox)
                {
                    textbox.Text = null;
                    textbox.IsEnabled = true;
                }
                else if (component is CheckBox checkbox)
                {
                    checkbox.IsChecked = false;
                    checkbox.IsEnabled = true;
                }
                else if (component is ComboBox combobox)
                {
                    if (!combobox.Name.Equals(MapPage.PlaceAreaComboBox.Name) && !combobox.Name.Equals(MapPage.PlaceStreetComboBox.Name))
                    {
                        combobox.SelectedIndex = 2;
                        combobox.IsEnabled = false;
                    }
                } else if(component is Grid nestedGrid)
                {
                    ResetInputs(nestedGrid);
                }
            }
        }
        public int GetAddTextBox(TextBox textbox, int editNumber)
        {
            int number;
            if (int.TryParse(textbox.Text, out number) && number > 0 && !string.IsNullOrEmpty(textbox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                editNumber = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                MapPage._wrongInput = true;
            }
            return editNumber;
        }
        public int GetAddAmountOfPeople(TextBox textbox, int amountOfPeople)
        {
            return GetAddTextBox(textbox, amountOfPeople);
        }
        public int GetAddSurfaceArea(TextBox textbox, int surfaceArea)
        {
            return GetAddTextBox(textbox, surfaceArea);
        }
        public double GetAddPricePerNightPerPerson(TextBox textbox, double pricePerNightPerPerson)
        {
            double number;
            if (double.TryParse(textbox.Text, out number) && number > 0 && !string.IsNullOrEmpty(textbox.Text))
                pricePerNightPerPerson = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                MapPage._wrongInput = true;
            }
            return pricePerNightPerPerson;
        }
        public string GetAddComponentNameTextbox(TextBox textbox, string name)
        {
            if (!string.IsNullOrEmpty(textbox.Text))
                name = textbox.Text;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                MapPage._wrongInput = true;
            }
            return name;
        }
        public void HideInfoGrids()
        {
            SetInfoGridsInvisible();
            MapPage.SelectedArea = null;
            MapPage.SelectedPlace = null;
            MapPage.SelectedStreet = null;
            MapPage._streetPoint1 = new Point(-1, -1);
            GenerateMap();
        }
        public void ResetInputFields(string componentString)
        {
            Grid grid = MapPage.PlaceInfoGrid;
            if (componentString.Equals("Place"))
            {
                MapPage.PlaceStreetComboBox.Items.Clear();
                MapPage.PlaceAreaComboBox.Items.Clear();
                foreach (Street street in MapPage._streets)
                    MapPage.PlaceStreetComboBox.Items.Add(street.Name);
                foreach (Area area in MapPage._areas)
                    MapPage.PlaceAreaComboBox.Items.Add(area.Name);
            }
            else if (componentString.Equals("Street")) grid = MapPage.StreetInfoGrid;
            else if (componentString.Equals("Area")) grid = MapPage.AreaInfoGrid;

            ResetInputs(grid);
            ResetBorders(grid);
        }
        public void ResetAfterAddingMapComponent(string mapComponent)
        {
            UIElementCollection collection = null;
            if (mapComponent.Contains("Place"))
            {
                MapPage.PlaceInfoGrid.Visibility = Visibility.Hidden;
                collection = MapPage.PlaceInfoGrid.Children;
            }
            if (mapComponent.Contains("Street"))
            {
                collection = MapPage.StreetInfoGrid.Children;
                MapPage.StreetInfoGrid.Visibility = Visibility.Hidden;
            }

            if (collection != null && collection.Count > 0)
            {
                foreach (Grid grid in collection)
                {
                    MapPage._placeOnMap.ResetBorders(grid);
                }
                GenerateMap();
            }
        }
        public void HighLightPlaces(Object type, SolidColorBrush color)
        {
            List<Place> places = new List<Place>();

            if (type is Street street)
                places = MapPage._places.Where(p => p.StreetID == street.StreetID).ToList();
            if (type is Area area)
                places = MapPage._places.Where(p => p.AreaID == area.AreaID).ToList();

            if (places.Count > 0)
            {
                foreach (var comp in MapPage.field.Children)
                {
                    if (comp is Border placeBlock && placeBlock.Child is Canvas canvas && canvas.Name.Contains("Place"))
                    {
                        foreach (Place placeData in places)
                            if (canvas.Name.Equals("Place" + placeData.PlaceID.ToString())) canvas.Background = color;
                    }
                }
            }
        }
        public void GenerateMap()
        {
            MapPage.field.Children.Clear();
            MapPage._areas = _camping.GetAreas();
            MapPage._streets = _camping.GetStreets();
            MapPage._places = _camping.GetPlaces();
            GenerateComponentsMap(MapPage._areas);
            GenerateComponentsMap(MapPage._streets);
            GenerateComponentsMap(MapPage._places);
        }
        public void GenerateComponentsMap<T>(List<T> list)
        {
            if (list != null && list.Count() > 0)
            {
                foreach (var comp in list)
                {
                    if (comp is Area)
                    {
                        Border border = MapPageArea.GenerateArea((Area)(object)(comp));
                        MapPage.field.Children.Add(border);
                        MapPage._areaOnMap.SetAreaEvents(border, (Area)(object)comp);
                    }
                    else if (comp is Street street)
                    {
                        MapPageStreet.GenerateStreet(street, Brushes.Black);
                        Line line = MapPageStreet.GetLine();
                        MapPage.field.Children.Add(line);
                        MapPage.field.Children.Add(MapPageStreet.GetTextBlock());
                        MapPage._streetOnMap.SetStreetEvents(line, street);
                    }
                    else if (comp is Place place)
                    {
                        MapPagePlace.GeneratePlace(MapPage.field, (Place)(object)comp, Brushes.Black, true);
                        MapPage._placeOnMap.SetPlaceEvents(Brushes.Black, MapPagePlace.Canvas, place);
                    }
                }
            }
        }
    }
}
