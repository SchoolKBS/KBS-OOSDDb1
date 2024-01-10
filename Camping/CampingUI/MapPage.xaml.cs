using CampingCore;
using CampingDataAccess;
using CampingUI.GenerateComponentsMap;
using CampingUI.Map;
using CampingUI.Map.AreaMap;
using CampingUI.Map.PlaceMap;
using CampingUI.Map.StreetMap;
using CampingUI.NewFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Transform = CampingUI.NewFolder.Transform;

namespace CampingUI
{
    public partial class MapPage : Page
    {
        public event Action<Place> PlaceSelectedOnMap;
        public Camping Camping;
        public List<Area> Areas;
        public List<Place> Places;
        public List<Street> Streets;
        public int PlaceSurfaceArea, PlaceAmountOfPeople, PlacePlaceID, PlaceStreetID, PlaceAreaID, YPressed, XPressed;
        public Canvas SelectedPlace;
        public int StreetSurfaceArea, StreetAmountOfPeople;
        public int AreaSurfaceArea, AreaAmountOfPeople;
        public double PlacePricePerNightPerPerson, StreetPricePerNightPerPerson, AreaPricePerNightPerPerson, XCord1, YCord1, XCord2, YCord2;
        public string StreetName, AreaName;
        public Canvas PreviousSelectedCanvas;
        public double DesiredWidth = 1000;
        public double DesiredHeight = 750;
        public bool EditPlaceBool, EditStreetBool, WrongInput;
        public string SelectedMapButton = "View";
        public Point StreetPoint1 = new Point(-1, -1);
        public Point AreaStartPoint = new Point(-1, -1);
        public Border NewArea;
        public Area SelectedArea;
        public Street SelectedStreet;
        public PlaceOnMap PlaceOnMap;
        public StreetOnMap StreetOnMap;
        public AreaOnMap AreaOnMap;
        public MapMethods MapMethods;

        public MapPage(Camping camping)
        {
            InitializeComponent();
            Camping = camping;
            PlaceOnMap = new PlaceOnMap(this, camping);
            StreetOnMap = new StreetOnMap(this, camping);
            AreaOnMap = new AreaOnMap(this, camping);
            MapMethods = new MapMethods(this, camping);
            new Transform(field, DesiredWidth, DesiredHeight, "plattegrond");
            MapMethods.GenerateMap(field);

            Loaded += (sender, e) =>
            {
                Focusable = true;
                Keyboard.Focus(this);
            };
            KeyDown += Handle_KeyDown;



        }
        public void HandlePlaceClickMinimap(Place place)
        {
            PlaceSelectedOnMap?.Invoke(place);
        }
        private void field_MouseEnter(object sender, MouseEventArgs e)
        {
            if (SelectedMapButton.Contains("Place"))
            {
                Place place = new Place(1000000000, false, 1, 1, false, 0, 0, 0, XPressed, YPressed);
                PlaceOnMap.GeneratePreviewPlace(place, Brushes.Aquamarine);
            }
            else if (SelectedMapButton.Contains("Street"))
            {
                Line line1 = null;
                bool canMakeLine = true;
                foreach (var component in field.Children)
                {
                    if (component is Line line)
                    {
                        if (line.Name.Equals("firstPoint")) line1 = line;
                        if (line.Name.Equals("LineSet")) canMakeLine = false;
                    }
                }
                if (line1 == null && canMakeLine)
                    StreetOnMap.GeneratePreviewLine("MoveablePoint", Brushes.Aquamarine);
            }
        }
        private void field_MouseMove(object sender, MouseEventArgs e)
        {
            if (SelectedMapButton.Contains("Place"))
            {
                foreach (var component in field.Children)
                {
                    if (component is Border border && border.Name.Equals("Place_1000000000"))
                    {
                        Point p = Mouse.GetPosition(field);
                        if (border != null)
                        {
                            Canvas.SetLeft(border, p.X - 15);
                            Canvas.SetTop(border, p.Y - 15);
                        }
                        break;
                    }
                }
            }
            else if (SelectedMapButton.Contains("Street"))
            {
                Point p = Mouse.GetPosition(field);
                foreach (var component in field.Children)
                {
                    if (component is Line line && line.Name.Equals("MoveablePoint") && !line.Name.Equals("firstPoint"))
                    {
                        double centerX = line.X1 + (line.X2 - line.X1) / 2;
                        double centerY = line.Y1 + (line.Y2 - line.Y1) / 2;

                        double offsetX = p.X - centerX;
                        double offsetY = p.Y - centerY;

                        line.X1 += offsetX;
                        line.Y1 += offsetY;
                        line.X2 += offsetX;
                        line.Y2 += offsetY;
                    }
                    else if (component is Line firstPoint && firstPoint.Name.Equals("firstPoint"))
                    {
                        firstPoint.X2 = p.X + 7.5;
                        firstPoint.Y2 = p.Y - 3.75;
                    }
                }
            }
        }
        private void field_MouseLeave(object sender, MouseEventArgs e)
        {
            if (SelectedMapButton.Contains("Place"))
            {
                foreach (var component in field.Children)
                    if (component is Border border && border.Name.Equals("Place_1000000000"))
                    {
                        field.Children.Remove(border);
                        break;
                    }
            }
        }
        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedMapButton.Contains("Place"))
            {
                PlaceOnMap.RemoveOldPreviewPlace();
                PlaceOnMap.GenerateNewPlaceWithInfoBoxes();
            }
            else if (SelectedMapButton.Contains("Street"))
            {
                StreetOnMap.FieldMouseDownStreet();
            }
            else if (SelectedMapButton.Contains("Area"))
            {
                AreaOnMap.FieldMouseDownArea();
            }
        }
        private void Handle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (PlaceInfoGrid.Visibility.Equals(Visibility.Hidden) && StreetInfoGrid.Visibility.Equals(Visibility.Hidden) && AreaInfoGrid.Visibility.Equals(Visibility.Hidden))
                {
                    SelectedMapButton = "View";
                    foreach (Button gridButton in MapGridButtons.Children)
                    {
                        Style selectionStyle = (Style)gridButton.FindResource("ButtonStyle1Selection");
                        gridButton.Style = selectionStyle;
                    }
                }
                else MapMethods.HideInfoGrids();
                StreetPoint1 = new Point(-1, -1);
                AreaStartPoint = new Point(-1, -1);
                MapMethods.GenerateMap(field);
                if (SelectedMapButton.Contains("Place"))
                {
                    Point p = Mouse.GetPosition(field);
                    XPressed = (int)p.X;
                    YPressed = (int)p.Y;

                    Place place = new Place(1000000000, false, 1, 1, false, 0, 0, 0, XPressed, YPressed);
                    PlaceOnMap.GeneratePreviewPlace(place, Brushes.Aquamarine);
                }
                if (SelectedMapButton.Contains("Street"))
                    StreetOnMap.GeneratePreviewLine("MoveablePoint", Brushes.Aquamarine);
            }
        }
        private void MakeMapComponentButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOnMap.HideInfoGrids();
            Button button = (Button)sender;
            Style applyStyle = (Style)button.FindResource("ButtonStyle1Apply");
            Style selectionStyle = (Style)button.FindResource("ButtonStyle1Selection");
            if (button.Style.Equals(selectionStyle))
            {
                foreach (Button gridButton in MapGridButtons.Children)
                    gridButton.Style = selectionStyle;

                button.Style = applyStyle;
                SelectedMapButton = button.Name;
            }
            else
            {
                button.Style = selectionStyle;
                MapMethods.GenerateMap(field);
                SelectedMapButton = "View";
            }
        }
        public void HandleAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            PlaceOnMap.GetAddPlaceValues();
            if (!WrongInput)
            {
                bool hasPower = false;
                if (PlaceHasPowerCheckbox.IsChecked == true)
                    hasPower = true;
                bool hasDogs = false;
                if (PlaceHasDogsCheckbox.IsChecked == true)
                    hasDogs = true;
                Street street = Camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                Area area = Camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                Place place = new Place(PlacePlaceID, hasPower, street.StreetID, area.AreaID, hasDogs, PlaceSurfaceArea, PlaceAmountOfPeople, PlacePricePerNightPerPerson, XPressed, YPressed);
                if (EditPlaceBool)
                {
                    Camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(Int32.Parse(PlacePlaceIDTextbox.Text), street.StreetID, area.AreaID, hasPower, PlaceSurfaceArea, PlacePricePerNightPerPerson, PlaceAmountOfPeople, hasDogs);
                    Camping.CampingRepository.CampingPlaceRepository.UpdatePlaceDataExtending(Int32.Parse(PlacePlaceIDTextbox.Text),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlacePowerComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlaceDogsComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlaceSurfaceAreaComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlacePricePerNightPerPersonComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlaceAmountOfPeopleComboBox));
                    PlaceOnMap.ResetAfterAddingMapComponent("Place");
                    return;
                }
                else 
                {
                    Camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                    Camping.CampingRepository.CampingMapRepository.AddExtend(place.PlaceID,
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlacePowerComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlaceDogsComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlaceSurfaceAreaComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlacePricePerNightPerPersonComboBox),
                                                          PlaceOnMap.GetValueFromExtendComboBox(PlaceAmountOfPeopleComboBox));
                    Camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                }
                PlaceOnMap.ResetInputFields("Place");
                PlaceOnMap.ResetAfterAddingMapComponent("Place");
            }
        }
        private void HandleAddStreet_Click(Object sender, RoutedEventArgs e)
        {
            StreetOnMap.GetAddStreetValues();
            if (!WrongInput)
            {
                bool hasPower = (bool)StreetHasPowerCheckbox.IsChecked;
                bool hasDogs = (bool)StreetHasDogsCheckbox.IsChecked;
                if (EditStreetBool)
                {
                    Camping.CampingRepository.CampingMapRepository.UpdateStreetByStreetID(StreetName, hasPower, hasDogs, StreetSurfaceArea, StreetPricePerNightPerPerson, StreetAmountOfPeople, SelectedStreet.StreetID);
                    Street street = Camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(SelectedStreet.StreetID);
                    List<Place> streetPlaces = Places.Where(p => p.StreetID == street.StreetID).ToList();
                    foreach (Place place in streetPlaces)
                    {
                        bool placeExtendPower = place.Power;
                        bool placeExtendDogs = place.Dogs;
                        int placeExtendSurfaceArea = place.SurfaceArea;
                        double placeExtendPricePerNightPerPerson = place.PricePerNightPerPerson;
                        int placeExtendAmountOfPeople = place.AmountOfPeople;

                        List<bool?> placeExtend = Camping.CampingRepository.CampingPlaceRepository.GetPlaceExtendingByPlaceID(place.PlaceID);
                        if (placeExtend[0] == true) placeExtendPower = true;
                        if (placeExtend[1] == true) placeExtendDogs = true;
                        if (placeExtend[2] == true) placeExtendSurfaceArea = street.SurfaceArea;
                        if (placeExtend[3] == true) placeExtendPricePerNightPerPerson = street.PricePerNightPerPerson;
                        if (placeExtend[4] == true) placeExtendAmountOfPeople = street.AmountOfPeople;
                        Camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(place.PlaceID,
                                                                                          place.StreetID,
                                                                                          place.AreaID,
                                                                                          placeExtendPower,
                                                                                          placeExtendSurfaceArea,
                                                                                          placeExtendPricePerNightPerPerson,
                                                                                          placeExtendAmountOfPeople,
                                                                                          placeExtendDogs);
                    }
                }
                else
                {
                    Camping.CampingRepository.CampingMapRepository.AddNewStreet(StreetName, hasPower, hasDogs, StreetSurfaceArea, StreetPricePerNightPerPerson, StreetAmountOfPeople, (int)XCord1, (int)YCord1, (int)XCord2, (int)YCord2);
                    StreetPoint1 = new Point(-1, -1);
                }
                StreetOnMap.ResetAfterAddingMapComponent("Street");
                MapMethods.GenerateMap(field);
            }
        }
        private void SelectedArea_ColorChange(object sender, SelectionChangedEventArgs e)
        {
            if (NewArea != null)
            {
                Canvas usedCanvas = (Canvas)NewArea.Child;
                if (AreaColor.SelectedItem != null)
                {
                    int color = int.Parse(AreaColor.SelectedValue.ToString());
                    usedCanvas.Background = new SolidColorBrush(StaticUIMethods.GetColorFromInt(color));
                }
            }
        }
        public void HandleCancelAddComponent_Click(Object sender, RoutedEventArgs e)
        {
            PlaceOnMap.ResetAfterAddingMapComponent("Place");
            StreetOnMap.ResetAfterAddingMapComponent("Street");
            AreaOnMap.ResetAfterAddingMapComponent("Area");
            MapMethods.HideInfoGrids();
        }
        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetTextboxBorder(textbox);
                WrongInput = false;
            }
        }
        private void HandleCancelAddArea_Click(object sender, RoutedEventArgs e)
        {
            AreaStartPoint.X = -1;
            AreaStartPoint.Y = -1;
            SelectedArea = null;
            NewArea = null;
            MapMethods.GenerateMap(field);
            AreaInfoGrid.Visibility = Visibility.Hidden;
        }
        private void HandleAddArea_Click(object sender, RoutedEventArgs e)
        {
            AreaOnMap.ResetBorders(AreaInfoGrid);
            AreaOnMap.GetAddAreaValues();
            if(!WrongInput)
            {
                Camping.CampingRepository.CampingMapRepository.AddNewArea(SelectedArea);
                AreaInfoGrid.Visibility = Visibility.Hidden;
                AreaStartPoint.X = -1;
                AreaStartPoint.Y = -1;
                MapMethods.GenerateMap(field);
            }
        }
        private void PlaceStreetAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            Border border = PlaceAreaBorder;
            if (combobox.Name.Equals(PlaceStreetComboBox.Name)) border = PlaceStreetBorder;
            if (border.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetComboBoxBorder(border);
                WrongInput = false;
            }
            PlaceOnMap.EnableExtendComboBoxes(false);
            if (PlaceStreetComboBox.SelectedItem != null && PlaceAreaComboBox.SelectedItem != null)
            {
                PlaceOnMap.EnableExtendComboBoxes(true);

                foreach (Grid grid in PlaceInfoGrid.Children)
                {
                    foreach (var component in grid.Children)
                        if (component is ComboBox com)
                            if (com.Name != PlaceStreetComboBox.Name && com.Name != PlaceAreaComboBox.Name)
                                PlaceOnMap.HandleExtending(com);
                }
            }
        }
        private void HandleExtendChange_Click(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (PlaceOnMap != null)
            {
                PlaceOnMap.HandleExtending(comboBox);
            }
        }
    }
}
