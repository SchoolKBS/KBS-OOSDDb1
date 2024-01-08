using CampingCore;
using CampingUI.GenerateComponentsMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CampingUI.Map.PlaceMap
{
    public class PlaceOnMap : MapMethods
    {
        public PlaceOnMap(MapPage mapPage, Camping camping) : base(mapPage, camping) { }

        public bool? GetValueFromExtendComboBox(ComboBox combobox)
        {
            bool? extendBool = null;
            if (combobox.SelectedIndex == 0) extendBool = true;
            else if (combobox.SelectedIndex == 1) extendBool = false;
            return extendBool;
        }
        public void SetPlaceDataOnFields(Place place, Camping camping)
        {
            MapPage.PlaceStreetComboBox.Text = camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(place).Name;
            MapPage.PlaceAreaComboBox.Text = camping.CampingRepository.CampingMapRepository.GetAreaByAreaID(place).Name;
            MapPage.PlacePlaceIDTextbox.IsEnabled = false;
            MapPage.PlacePlaceIDTextbox.Text = place.PlaceID.ToString();
            MapPage.PlaceHasPowerCheckbox.IsChecked = place.Power;
            MapPage.PlaceHasDogsCheckbox.IsChecked = place.Dogs;
            MapPage.PlaceSurfaceAreaTextbox.Text = place.SurfaceArea.ToString();
            MapPage.PlacePricePerNightPerPersonTextbox.Text = place.PricePerNightPerPerson.ToString();
            MapPage.PlaceAmountOfPeopleTextbox.Text = place.AmountOfPeople.ToString();
        }
        public void EnableExtendComboBoxes(bool extend)
        {
            foreach (Grid grid in MapPage.PlaceInfoGrid.Children)
            {
                foreach (var component in grid.Children)
                {
                    if (component is ComboBox comboBox)
                    {
                        if (extend == false) comboBox.Opacity = 0.5;
                        else comboBox.Opacity = 1;
                        comboBox.IsEnabled = extend;
                    }
                }
            }
        }
        public void GetAddPlaceID()
        {
            MapPage.PlacePlaceID = GetAddTextBox(MapPage.PlacePlaceIDTextbox, MapPage.PlacePlaceID);
            List<Place> places = MapPage.Camping.CampingRepository.CampingPlaceRepository.GetPlaces().Where(i => i.PlaceID == MapPage.PlacePlaceID).ToList();
            if (places.Count > 0 && !MapPage.EditPlaceBool)
            {
                StaticUIMethods.SetErrorTextboxBorder(MapPage.PlacePlaceIDTextbox);
                MapPage.WrongInput = true;
                MapPage.PlacePlaceID = -1;
            }
        }
        public void GetAddAreaID()
        {
            if (MapPage.PlaceAreaComboBox.SelectedItem != null)
            {
                Area area = MapPage.Camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(MapPage.PlaceAreaComboBox.SelectedItem.ToString());
                MapPage.PlaceAreaID = area.AreaID;
            }
            else
            {
                StaticUIMethods.SetErrorComboBoxBorder(MapPage.PlaceAreaBorder);
                MapPage.WrongInput = true;
            }
        }
        public void GetAddStreetID()
        {
            if (MapPage.PlaceStreetComboBox.SelectedItem != null)
            {
                Street street = MapPage.Camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(MapPage.PlaceStreetComboBox.SelectedItem.ToString());
                MapPage.PlaceStreetID = street.StreetID;
            }
            else
            {
                StaticUIMethods.SetErrorComboBoxBorder(MapPage.PlaceStreetBorder);
                MapPage.WrongInput = true;
            }
        }
        public void GetAddPlaceValues()
        {
            GetAddPlaceID();
            GetAddStreetID();
            GetAddAreaID();
            MapPage.PlaceAmountOfPeople = GetAddAmountOfPeople(MapPage.PlaceAmountOfPeopleTextbox, MapPage.PlaceAmountOfPeople);
            MapPage.PlaceSurfaceArea = GetAddSurfaceArea(MapPage.PlaceSurfaceAreaTextbox, MapPage.PlaceSurfaceArea);
            MapPage.PlacePricePerNightPerPerson = GetAddPricePerNightPerPerson(MapPage.PlacePricePerNightPerPersonTextbox, MapPage.PlacePricePerNightPerPerson);
        }
        public void SetTextboxValues(ComboBox comboBox, TextBox textbox, Street street, Area area)
        {
            int selectedIndex = comboBox.SelectedIndex;
            if (comboBox.Name.Contains("SurfaceArea") && textbox.Name.Contains("SurfaceArea"))
            {
                textbox.Text = selectedIndex == 0 ? street.SurfaceArea.ToString() : selectedIndex == 1 ? area.SurfaceArea.ToString(): textbox.Text;
                textbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("PricePerNight") && textbox.Name.Contains("PricePerNight"))
            {
                textbox.Text = selectedIndex == 0 ? street.PricePerNightPerPerson.ToString() : selectedIndex == 1 ? area.PricePerNightPerPerson.ToString() : textbox.Text;
                textbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("AmountOfPeople") && textbox.Name.Contains("AmountOfPeople"))
            {
                textbox.Text = selectedIndex == 0 ? street.AmountOfPeople.ToString() : selectedIndex == 1 ? area.AmountOfPeople.ToString(): textbox.Text;
                textbox.IsEnabled = selectedIndex == 2;
            }
        }
        public void SetCheckboxValues(ComboBox comboBox, CheckBox checkbox, Street street, Area area)
        {
            int selectedIndex = comboBox.SelectedIndex;
            if (comboBox.Name.Contains("Power") && checkbox.Name.Contains("Power"))
            {
                checkbox.IsChecked = selectedIndex == 0 ? street.Power : selectedIndex == 1 ? area.Power : checkbox.IsChecked;
                checkbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("Dogs") && checkbox.Name.Contains("Dogs"))
            {
                checkbox.IsChecked = selectedIndex == 0 ? street.Dogs : selectedIndex == 1 ? area.Dogs : checkbox.IsChecked;
                checkbox.IsEnabled = selectedIndex == 2;
            }
        }
        public void HandleExtending(ComboBox comboBox)
        {
            Area area = null;
            Street street = null;

            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                if (MapPage.PlaceAreaComboBox.SelectedItem != null)
                    area = MapPage.Camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(MapPage.PlaceAreaComboBox.SelectedItem.ToString());
                if (MapPage.PlaceStreetComboBox.SelectedItem != null)
                    street = MapPage.Camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(MapPage.PlaceStreetComboBox.SelectedItem.ToString());
            }

            if (street != null || area != null)
            {
                foreach (Grid grid in MapPage.PlaceInfoGrid.Children)
                {
                    foreach (var component in grid.Children)
                    {
                        if (component is TextBox textbox)
                            MapPage.PlaceOnMap.SetTextboxValues(comboBox, textbox, street, area);
                        else if (component is CheckBox checkbox)
                            MapPage.PlaceOnMap.SetCheckboxValues(comboBox, checkbox, street, area);
                    }
                }
            }
        }
        public void SetPlaceDataExtending(Place place)
        {
            List<bool?> placeExtend = MapPage.Camping.CampingRepository.CampingPlaceRepository.GetPlaceExtendingByPlaceID(place.PlaceID);
            int counter = 0;
            foreach (Grid grid in MapPage.PlaceInfoGrid.Children)
            {
                foreach (var comp in grid.Children)
                {
                    if (comp is ComboBox comboBox && !comboBox.Name.Equals("PlaceStreetComboBox") && !comboBox.Name.Equals("PlaceAreaComboBox"))
                    {
                        if (placeExtend[counter].HasValue)
                        {
                            if (placeExtend[counter] == true) comboBox.SelectedIndex = 0;
                            else comboBox.SelectedIndex = 1;
                        }
                        else comboBox.SelectedIndex = 2;
                        counter++;
                    }
                }
            }
        }
        public void HandlePlaceClick(Place place, bool AddPlaceBool)
        {
            SetInfoVisible("Place");
            ResetInputFields("Place");
            MapPage.EditPlaceBool = false;
            if (!AddPlaceBool)
            {
                MapPage.EditPlaceBool = true;
                SetPlaceDataOnFields(place, MapPage.Camping);
                SetPlaceDataExtending(place);
            }
        }
        public void SetPlaceEvents(SolidColorBrush brush, Canvas canvasPlace, Place place)
        {
            if (brush == Brushes.Black)
            {
                canvasPlace.MouseEnter += (sender, e) =>
                {
                    if (MapPage.SelectedMapButton.Contains("View")) canvasPlace.Background = Brushes.DarkCyan;
                };

                canvasPlace.MouseLeave += (sender, e) =>
                {
                    if (!((MapPage.SelectedPlace != null && MapPage.SelectedPlace.Name.Equals(canvasPlace.Name))
                       || (MapPage.SelectedArea != null && MapPage.SelectedArea.AreaID == place.AreaID)
                       || (MapPage.SelectedStreet != null && MapPage.SelectedStreet.StreetID == place.StreetID)))
                        canvasPlace.Background = Brushes.Black;
                };
            }

            canvasPlace.MouseLeftButtonDown += (sender, e) =>
            {
                if (MapPage.SelectedMapButton.Contains("View"))
                {
                    DeselectAllFields();
                    GenerateMap(MapPage.field);
                    canvasPlace.Background = Brushes.DarkCyan;
                    MapPage.PreviousSelectedCanvas = canvasPlace;
                    MapPage.SelectedStreet = null;
                    MapPage.SelectedArea = null;
                    MapPage.SelectedPlace = canvasPlace;
                    HandlePlaceClick(place, false);
                    MapPage.HandlePlaceClickMinimap(place);
                }
            };
        }   
        public void RemoveOldPreviewPlace()
        {
            foreach (var component in MapPage.field.Children)
            {
                if (component is Border border && border.Name.Equals("Place_0"))
                {
                    MapPage.field.Children.Remove(border);
                    break;
                }
            }
        }
        public void GenerateNewPlaceWithInfoBoxes()
        {
            Point p = Mouse.GetPosition(MapPage.field);
            MapPage.XPressed = (int)Math.Round(p.X) - 15;
            MapPage.YPressed = (int)Math.Round(p.Y) - 15;
            Place place1 = new Place(0, false, 1, 1, false, 0, 0, 0, MapPage.XPressed, MapPage.YPressed);
            GeneratePreviewPlace(place1, Brushes.DarkGray);
            EnableExtendComboBoxes(false);
            HandlePlaceClick(place1, true);
        }
        public void GeneratePreviewPlace(Place place, SolidColorBrush color)
        {
            Point p = Mouse.GetPosition(MapPage.field);
            MapPage.XPressed = (int)Math.Round(p.X) - 15;
            MapPage.YPressed = (int)Math.Round(p.Y) - 15;
            MapPagePlace.GeneratePlace(MapPage.field, place, color, false);
        }
    }
}
