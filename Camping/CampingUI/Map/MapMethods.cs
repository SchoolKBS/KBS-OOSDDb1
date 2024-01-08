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
    public class MapMethods 
    {
        protected MapPage MapPage { get; private set; }
        protected Camping Camping;
        public MapMethods(MapPage mapPage, Camping camping) 
        {
            MapPage = mapPage;
            Camping = camping;

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
                MapPage.WrongInput = true;
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
                MapPage.WrongInput = true;
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
                MapPage.WrongInput = true;
            }
            return name;
        }
        public void HideInfoGrids()
        {
            SetInfoGridsInvisible();
            MapPage.SelectedArea = null;
            MapPage.SelectedPlace = null;
            MapPage.SelectedStreet = null;
            MapPage.StreetPoint1 = new Point(-1, -1);
            GenerateMap(MapPage.field);
        }
        public void ResetInputFields(string componentString)
        {
            Grid grid = MapPage.PlaceInfoGrid;
            if (componentString.Equals("Place"))
            {
                MapPage.PlaceStreetComboBox.Items.Clear();
                MapPage.PlaceAreaComboBox.Items.Clear();
                foreach (Street street in MapPage.Streets)
                    MapPage.PlaceStreetComboBox.Items.Add(street.Name);
                foreach (Area area in MapPage.Areas)
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
                collection = MapPage.PlaceInfoGrid.Children;
                MapPage.PlaceInfoGrid.Visibility = Visibility.Hidden;
            }
            if (mapComponent.Contains("Street"))
            {
                collection = MapPage.StreetInfoGrid.Children;
                MapPage.StreetInfoGrid.Visibility = Visibility.Hidden;
            }
            if (mapComponent.Contains("Area"))
            {
                collection = MapPage.AreaInfoGrid.Children;
                MapPage.AreaInfoGrid.Visibility = Visibility.Hidden;
            }

            if (collection != null && collection.Count > 0)
            {
                foreach (Grid grid in collection)
                {
                    MapPage.PlaceOnMap.ResetBorders(grid);
                }
                GenerateMap(MapPage.field);
            }
        }
        public void HighLightPlaces(Object type, SolidColorBrush color)
        {
            List<Place> places = new List<Place>();

            if (type is Street street)
                places = MapPage.Places.Where(p => p.StreetID == street.StreetID).ToList();
            if (type is Area area)
                places = MapPage.Places.Where(p => p.AreaID == area.AreaID).ToList();

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
        public void GenerateMap(Canvas canvas)
        {
            canvas.Children.Clear();
            MapPage.Areas = Camping.CampingRepository.CampingMapRepository.GetAreas();
            MapPage.Streets = Camping.CampingRepository.CampingMapRepository.GetStreets();
            MapPage.Places = Camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            GenerateComponentsMap(MapPage.Areas, canvas);
            GenerateComponentsMap(MapPage.Streets, canvas);
            GenerateComponentsMap(MapPage.Places, canvas);
        }
        public void GenerateComponentsMap<T>(List<T> list, Canvas canvas)
        {
            if (list != null && list.Count() > 0)
            {
                foreach (var comp in list)
                {
                    if (comp is Area)
                    {
                        Border border = MapPageArea.GenerateArea((Area)(object)(comp));
                        canvas.Children.Add(border);
                        MapPage.AreaOnMap.SetAreaEvents(border, (Area)(object)comp);
                    }
                    else if (comp is Street street)
                    {
                        MapPageStreet.GenerateStreet(street, Brushes.Black);
                        Line line = MapPageStreet.GetLine();
                        canvas.Children.Add(line);
                        canvas.Children.Add(MapPageStreet.GetTextBlock());
                        MapPage.StreetOnMap.SetStreetEvents(line, street);
                    }
                    else if (comp is Place place)
                    {
                        MapPagePlace.GeneratePlace(canvas, (Place)(object)comp, Brushes.Black, true);
                        MapPage.PlaceOnMap.SetPlaceEvents(Brushes.Black, MapPagePlace.Canvas, place);
                    }
                }
            }
        }
    }
}
