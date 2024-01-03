using System;
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
    public class MapMethods : MapPage
    {
        protected MapPage MapPage { get; private set; }
        private Camping _camping;
        public MapMethods(MapPage mapPage, Camping camping) : base(camping)
        {
            MapPage = mapPage;
            _camping = camping;

        }
        public void DeselectAllFields()
        {
            foreach (var comp in field.Children)
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
                PlaceInfo.Visibility = Visibility.Visible;
                PlaceInfoGrid.Visibility = Visibility.Visible;
            }
            else if (component.Equals("Street"))
            {
                StreetInfo.Visibility = Visibility.Visible;
                StreetInfoGrid.Visibility = Visibility.Visible;
            }
            else if (component.Equals("Area"))
            {
                AreaInfo.Visibility = Visibility.Visible;
                AreaInfoGrid.Visibility = Visibility.Visible;
            }
        }
        public void SetInfoGridsInvisible()
        {
            PlaceInfo.Visibility = Visibility.Visible;
            StreetInfo.Visibility = Visibility.Hidden;
            AreaInfo.Visibility = Visibility.Hidden;
            PlaceInfoGrid.Visibility = Visibility.Hidden;
            StreetInfoGrid.Visibility = Visibility.Hidden;
            AreaInfoGrid.Visibility = Visibility.Hidden;
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
                    if (!combobox.Name.Equals(PlaceAreaComboBox.Name) && !combobox.Name.Equals(PlaceStreetComboBox.Name))
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
                _wrongInput = true;
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
                _wrongInput = true;
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
                _wrongInput = true;
            }
            return name;
        }
        public void HideInfoGrids()
        {
            SetInfoGridsInvisible();
            SelectedArea = null;
            SelectedPlace = null;
            SelectedStreet = null;
            _streetPoint1 = new Point(-1, -1);
            GenerateMap();
        }
        public void ResetInputFields(string componentString)
        {
            Grid grid = PlaceInfoGrid;
            if (componentString.Equals("Place"))
            {
                PlaceStreetComboBox.Items.Clear();
                PlaceAreaComboBox.Items.Clear();
                foreach (Street street in _streets)
                    PlaceStreetComboBox.Items.Add(street.Name);
                foreach (Area area in _areas)
                    PlaceAreaComboBox.Items.Add(area.Name);
            }
            else if (componentString.Equals("Street")) grid = StreetInfoGrid;
            else if (componentString.Equals("Area")) grid = AreaInfoGrid;

            ResetInputs(grid);
            ResetBorders(grid);
        }
        public void ResetAfterAddingMapComponent(string mapComponent)
        {
            UIElementCollection collection = null;
            if (mapComponent.Contains("Place"))
            {
                PlaceInfoGrid.Visibility = Visibility.Hidden;
                collection = PlaceInfoGrid.Children;
            }
            if (mapComponent.Contains("Street"))
            {
                collection = StreetInfoGrid.Children;
                StreetInfoGrid.Visibility = Visibility.Hidden;
            }

            if (collection != null && collection.Count > 0)
            {
                foreach (Grid grid in collection)
                {
                    _placeOnMap.ResetBorders(grid);
                }
                GenerateMap();
            }
        }
        public void HighLightPlaces(Object type, SolidColorBrush color)
        {
            List<Place> places = new List<Place>();

            if (type is Street street)
                places = _places.Where(p => p.StreetID == street.StreetID).ToList();
            if (type is Area area)
                places = _places.Where(p => p.AreaID == area.AreaID).ToList();

            if (places.Count > 0)
            {
                foreach (var comp in field.Children)
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
            field.Children.Clear();
            _areas = _camping.GetAreas();
            _streets = _camping.GetStreets();
            _places = _camping.GetPlaces();
            GenerateComponentsMap(_areas);
            GenerateComponentsMap(_streets);
            GenerateComponentsMap(_places);
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
                        field.Children.Add(border);
                        _areaOnMap.SetAreaEvents(border, (Area)(object)comp);
                    }
                    else if (comp is Street street)
                    {
                        MapPageStreet.GenerateStreet(street, Brushes.Black);
                        Line line = MapPageStreet.GetLine();
                        field.Children.Add(line);
                        field.Children.Add(MapPageStreet.GetTextBlock());
                        _streetOnMap.SetStreetEvents(line, street);
                    }
                    else if (comp is Place place)
                    {
                        MapPagePlace.GeneratePlace(field, (Place)(object)comp, Brushes.Black, true);
                        _placeOnMap.SetPlaceEvents(Brushes.Black, MapPagePlace.Canvas, place);
                    }
                }
            }
        }
    }
}
