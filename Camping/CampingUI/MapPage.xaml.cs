using CampingCore;
using CampingDataAccess;
using CampingUI.GenerateComponentsMap;
using CampingUI.NewFolder;
using Org.BouncyCastle.Asn1.BC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
using Transform = CampingUI.NewFolder.Transform;

namespace CampingUI
{
    /// <summary>
    /// </summary>
    public partial class MapPage : Page
    {
        private Camping _camping;
        private List<Area> _areas;
        private List<Place> _places;
        private List<Street> _streets;
        private int _placeSurfaceArea, _placePersons, _placePlaceID, _placeStreetID, _placeAreaID, _yPressed, _xPressed;
        private Canvas SelectedPlace;
        private int _streetSurfaceArea, _streetPersons;
        private int _areaSurfaceArea, _areaPersons;
        private double _placePricePerNightPerPerson, _streetPricePerNightPerPerson, _areaPricePerNightPerPerson, _xCord1, _yCord1, _xCord2, _yCord2;
        private string _streetName, _areaName;
        private Canvas _previousSelectedCanvas;
        private bool _editPlaceBool, _wrongInput;
        private string _selectedMapButton = "View";
        private Point _streetPoint1 = new Point(-1, -1), _streetPoint2;
        private Point _areaStartPoint = new Point(-1, -1);
        private Border _newArea;
        public Area SelectedArea { get; private set; }
        public Street SelectedStreet;
        public MapPage(Camping camping)
        {
            InitializeComponent();
            _camping = camping;

            new Transform(field); // Transform scale of the map.
            GenerateMap();

            // For the keyboard handler
            Loaded += (sender, e) =>
            {
                Focusable = true;
                Keyboard.Focus(this);
            };
            KeyDown += Handle_KeyDown;      // Handle keyboard buttons.
        }

        public void GenerateMap()
        {
            field.Children.Clear();
            _areas = _camping.CampingRepository.CampingMapRepository.GetAreas().ToList();
            _streets = _camping.CampingRepository.CampingMapRepository.GetStreets().ToList();
            _places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces().ToList();
            GenerateComponentsMap(_areas);
            GenerateComponentsMap(_streets);
            GenerateComponentsMap(_places);
        }
        public void GenerateComponentsMap<T>(List<T> list)
        {
            if(list != null && list.Count() > 0)
            {
                foreach (var comp in list)
                {
                   if(comp is Area)
                    {
                        Border border = MapPageArea.GenerateArea((Area)(object)(comp));                      
                        field.Children.Add(border);
                        SetAreaEvents(border, (Area)(object)comp);
                    }
                    if (comp is Street)
                    {
                        MapPageStreet.GenerateStreet((Street)(object)comp, Brushes.Black);
                        Line line = MapPageStreet.GetLine();
                        field.Children.Add(line);
                        field.Children.Add(MapPageStreet.GetTextBlock());
                        SetStreetEvents(line, (Street)(object)comp);
                    }                      
                    if (comp is Place)
                        GeneratePlace((Place)(object)comp, Brushes.Black, true);
                }
            }
        }

        private void SetAreaEvents(Border border, Area area)
        {
            border.MouseLeftButtonDown += (sender, e) =>
            {
                if (_selectedMapButton.Contains("View"))
                {
                    if (SelectedArea != null)
                    {
                        foreach (var comp in field.Children)
                        {
                            if (comp is Border borderArea && borderArea.Name.Equals("Canvas_" + SelectedArea.AreaID.ToString()))
                            {
                                borderArea.BorderBrush = Brushes.Black;
                                borderArea.BorderThickness = new Thickness(1);
                                HighLightPlaces(SelectedArea, Brushes.Black);
                            }
                        }
                    }
                    SelectedArea = area;
                    SelectedStreet = null;
                    SelectedPlace = null;
                    ToggleAreaInput(false);
                    HandleAreaClick();
                    border.BorderBrush = Brushes.LightCyan;
                    border.BorderThickness = new Thickness(4);
                    HighLightPlaces(area, Brushes.DarkCyan);
                }
            };
        }
        private void SetStreetEvents(Line line, Street comp )
        {
            line.MouseLeftButtonDown += (sender, e) => HandleStreetClick((Street)(object)comp);
            line.MouseLeftButtonDown += (sender, e) => line.Stroke = Brushes.DarkCyan;
            line.MouseEnter += (sender, e) => line.Stroke = Brushes.DarkCyan;

            line.MouseLeave += (sender, e) =>
            {
                if (SelectedStreet == null) line.Stroke = Brushes.Black;
            };
        }
       
        public Border GeneratePlace(Place place, SolidColorBrush brush, bool AddPlaceBool)
        {
            var coordinates = place.GetPlacePositions();

            Border border = new Border
            {
                BorderBrush = Brushes.White,
                Width = 30,
                Height = 30,
                BorderThickness = new Thickness(1),
                Name = "Place_" + place.PlaceID.ToString(),
            };

            Canvas canvasPlace = new Canvas
            {
                Background = brush,
                Name = "Place" + place.PlaceID.ToString(),
            };

            border.Child = canvasPlace;
            Canvas.SetZIndex(canvasPlace, 100);
            Canvas.SetTop(border, coordinates[1]);
            Canvas.SetLeft(border, coordinates[0]);

            field.Children.Add(border);

            if (AddPlaceBool)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = place.PlaceID.ToString(),
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 20,
                };

                canvasPlace.Children.Add(textBlock);
                canvasPlace.Loaded += (sender, e) =>
                {
                    Canvas.SetTop(textBlock, (canvasPlace.ActualHeight - textBlock.ActualHeight) / 2);
                    Canvas.SetLeft(textBlock, (canvasPlace.ActualWidth - textBlock.ActualWidth) / 2);
                };
            }

            Canvas.SetTop(canvasPlace, (border.ActualHeight - canvasPlace.Height) / 2);
            Canvas.SetLeft(canvasPlace, (border.ActualWidth - canvasPlace.Width) / 2);

            if (brush == Brushes.Black)
            {
                canvasPlace.MouseEnter += (sender, e) =>
                {
                    if (_selectedMapButton.Contains("View")) canvasPlace.Background = Brushes.DarkCyan; 
                };

                canvasPlace.MouseLeave += (sender, e) =>
                {
                    if (!((SelectedPlace != null && SelectedPlace.Name.Equals(canvasPlace.Name)) 
                       || (SelectedArea != null && SelectedArea.AreaID == place.AreaID)
                       || (SelectedStreet != null && SelectedStreet.StreetID == place.StreetID)))
                        canvasPlace.Background = Brushes.Black;
                };
            }

            canvasPlace.MouseLeftButtonDown += (sender, e) =>
            {
                if (_selectedMapButton.Contains("View"))
                {
                    if (_previousSelectedCanvas != null) _previousSelectedCanvas.Background = Brushes.Black;
                    GenerateMap();
                    canvasPlace.Background = Brushes.DarkCyan;
                    _previousSelectedCanvas = canvasPlace;
                    SelectedStreet = null;
                    SelectedArea = null;
                    SelectedPlace = canvasPlace;
                    HandlePlaceClick(place, false);
                }
            };
            return border;
        }
        public void HandlePlaceClick(Place place, bool AddPlaceBool)
        {
            PlaceInfo.Visibility = Visibility.Visible;
            StreetInfo.Visibility = Visibility.Hidden;
            AreaInfo.Visibility = Visibility.Hidden;
            PlaceInfoGrid.Visibility = Visibility.Visible;
            StreetInfoGrid.Visibility = Visibility.Hidden;
            AreaInfoGrid.Visibility = Visibility.Hidden;
/*            if (!AddPlaceBool)
            {
                //ClearSelection();
                GenerateMap();
            }*/
            //else
            //{
                ResetInputFields();
                AddPlaceButton.Content = "Toevoegen";
                _editPlaceBool = false;
                PlaceInfoGrid.Visibility = Visibility.Visible;
                AddPlaceButton.Visibility = Visibility.Visible;
            //}
        }
        public void HandleStreetClick(Street street)
        {
            if(SelectedStreet != null)
            {
                HighLightPlaces(SelectedStreet, Brushes.Black);
            }
            GenerateMap();
            SelectedPlace = null;
            SelectedArea = null;
            SelectedStreet = street;
            PlaceInfo.Visibility = Visibility.Hidden;
            StreetInfo.Visibility = Visibility.Visible;
            StreetInfoGrid.Visibility = Visibility.Visible;
            AreaInfo.Visibility = Visibility.Hidden;
            StreetHasDogs.IsChecked = street.Dogs;
            StreetHasPower.IsChecked = street.Power;
            StreetPersons.Text = street.AmountOfPeople.ToString();
            StreetName.Text = street.Name;
            StreetPricePerNight.Text = street.PricePerNightPerPerson.ToString();
            StreetSurfaceArea.Text = street.SurfaceArea.ToString();
            foreach(Grid grid in StreetInfoGrid.Children)
            {
                foreach (var comp in grid.Children)
                {
                    if (comp is TextBox textbox) textbox.IsEnabled = false;
                    if (comp is CheckBox checkbox) checkbox.IsEnabled = false;
                }
            }
            HighLightPlaces(street, Brushes.DarkCyan);
            AddStreetButton.Content = "Aanpassen";

            //Als annuleren -> onzichtbaar maken en deselecteren

            //Als toevoegen -> Check op alle invoervelden
            //              -> Update straat gegevens in de database
            // 
        }

        private void HighLightPlaces(Object type, SolidColorBrush color)
        {

            List<Place> places = new List<Place>() ;

            if (type is Street street)
                places = _places.Where(p => p.StreetID == street.StreetID).ToList();
            if(type is Area area)
                places = _places.Where(p => p.AreaID == area.AreaID).ToList();

            if (places.Count > 0)
            {
                foreach (var comp in field.Children)
                {
                    if (comp is Border placeBlock && placeBlock.Child is Canvas canvas && canvas.Name.Contains("Place"))
                    {
                        foreach (Place placeData in places)
                        {
                            if (canvas.Name.Equals("Place" + placeData.PlaceID.ToString()))
                            {
                                canvas.Background = color;
                            }
                            else
                            {
                                //canvas.Background = Brushes.Black;
                            }
                        }
                    }
                }
            }
        }

        private void ResetInputFields()
        {
            PlaceStreetComboBox.Items.Clear();
            PlaceAreaComboBox.Items.Clear();
            foreach (Street street in _streets)
            {
                PlaceStreetComboBox.Items.Add(street.Name);
            }
            foreach (Area area in _areas)
            {
                PlaceAreaComboBox.Items.Add(area.Name);
            }
            foreach (Grid grid in PlaceInfoGrid.Children)
            {
                foreach (var component in grid.Children)
                {
                    if (component is TextBox textbox)
                    {
                        textbox.Text = null;
                        textbox.IsEnabled = true;
                    }
                    if (component is CheckBox checkbox)
                    {
                        checkbox.IsChecked = false;
                        checkbox.IsEnabled = true;
                    }
                    if (component is ComboBox combobox)
                    {
                        if (!combobox.Name.Equals(PlaceAreaComboBox.Name) && !combobox.Name.Equals(PlaceStreetComboBox.Name))
                        {
                            combobox.SelectedIndex = 2;
                            combobox.IsEnabled = false;
                        }
                    }
                }
            }
            ResetBorders();
        }

        private void ResetBorders()
        {
            StaticUIMethods.ResetTextboxBorder(PlacePlaceID);
            StaticUIMethods.ResetTextboxBorder(PlaceSurfaceArea);
            StaticUIMethods.ResetTextboxBorder(PlacePersons);
            StaticUIMethods.ResetTextboxBorder(PlacePricePerNight);
            ResetComboBoxBorder(PlaceAreaBorder);
            ResetComboBoxBorder(PlaceStreetBorder);
        }

        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectedMapButton.Contains("Place"))
            {
                Point p = Mouse.GetPosition(field);
                foreach (Place place in _places)
                {
                    place.XCord -= 15;
                    place.YCord -= 15;
                }
                _xPressed = (int)Math.Round(p.X) - 15;
                _yPressed = (int)Math.Round(p.Y) - 15;
                List<Area> PlaceWithinAreas = _areas.Where(i => i.XCord1 <= (_xPressed - 15))
                                                   .Where(i => i.XCord1 + i.Width >= (_xPressed + 45))
                                                   .Where(i => i.YCord1 <= (_yPressed - 15))
                                                   .Where(i => i.YCord1 + i.Height >= (_yPressed + 45))
                                                   .ToList();
                List<Place> placesNotInNewPlaceBorder = _places.Where(i => i.XCord >= (_xPressed - 45) && i.XCord <= (_xPressed + 45))
                                                              .Where(i => i.YCord >= (_yPressed - 45) && i.YCord <= (_yPressed + 45))
                                                              .ToList();

                GenerateMap();
                int i = 1;
                if (_places.Count > 0)
                {
                    i = _places.Last().PlaceID + 1;
                }

                Place place1 = new Place(0, false, 1, 1, false, 0, 0, 0, _xPressed, _yPressed);
                GeneratePlace(place1, Brushes.Gray, false);
                EnableExtendComboBoxes(false);
                HandlePlaceClick(place1, true); 
            }
            else if (_selectedMapButton.Contains("Street"))
            {
                GenerateMap();
                Point point = Mouse.GetPosition(field);
                double xCord = Math.Round(point.X);
                double yCord = Math.Round(point.Y); 
                if (_streetPoint1.X == -1 && _streetPoint1.Y == -1) 
                { 
                    _streetPoint1 = new Point(xCord, yCord);
                    Ellipse ellipse = new Ellipse();
                    Canvas.SetLeft(ellipse, xCord-7.5);
                    Canvas.SetTop(ellipse, yCord-7.5);
                    ellipse.Width = 15;
                    ellipse.Height = 15;
                    ellipse.Fill = Brushes.DarkGray;
                    field.Children.Add(ellipse);
                }
                else
                {
                    _streetPoint2 = new Point(xCord, yCord);

                    StreetInfo.Visibility = Visibility.Visible;
                    StreetInfoGrid.Visibility = Visibility.Visible;
                    Grid grid = new Grid();
                    Label label = new Label();
                    label.Content = "Straatnaam";
                    Line line = new Line();
                    line.X1 = _streetPoint1.X;
                    line.Y1 = _streetPoint1.Y;
                    line.X2 = _streetPoint2.X;
                    line.Y2 = _streetPoint2.Y;
                    _xCord1 = line.X1;
                    _yCord1 = line.Y1;
                    _xCord2 = line.X2;
                    _yCord2 = line.Y2;

                    line.StrokeThickness = 20;
                    line.Stroke = Brushes.DarkGray;
                    grid.Children.Add(line);
                    grid.Children.Add(label);

                    double deltaY;
                    double deltaX;
                    if(line.X1 > line.X2) deltaX = line.X1 - line.X2;
                    else deltaX = line.X2 - line.X1;

                    if (line.Y1 > line.Y2) deltaY = line.Y1 - line.Y2;
                    else deltaY = line.Y2 - line.Y1;

                    double degrees = Math.Atan(deltaY/deltaX)*180/Math.PI;
                    if(degrees < 7) line.Y2 = line.Y1;
                    if(degrees > 83) line.X2 = line.X1;

                    _xCord2 = line.X2;
                    _yCord2 = line.Y2;

                    field.Children.Add(grid);
                    
                    //Aanmaken straat openen
                    // -> straat aanmaken 
                    // -> straat aan database toevoegen

                }
            }
            else if (_selectedMapButton.Contains("Area"))
            {
                GenerateMap();
                Point point = Mouse.GetPosition(field);
                double xCord = Math.Round(point.X);
                double yCord = Math.Round(point.Y);
                AreaInfoGrid.Visibility = Visibility.Visible;
                AreaInfo.Visibility = Visibility.Visible;
                if (_areaStartPoint.X == -1 && _areaStartPoint.Y == -1)
                {
                    _areaStartPoint = new Point(xCord, yCord);
                    Ellipse ellipse = new Ellipse();
                    Canvas.SetLeft(ellipse, xCord - 7.5);
                    Canvas.SetTop(ellipse, yCord - 7.5);
                    ellipse.Width = 15;
                    ellipse.Height = 15;
                    ellipse.Fill = Brushes.DarkGray;
                    field.Children.Add(ellipse);
                    SelectedArea = new Area();
                    HandleAreaClick();
                    ResertAreaBorders();
                }
                else
                {

                    if(xCord < _areaStartPoint.X) (xCord, _areaStartPoint.X) = (_areaStartPoint.X, xCord);
                    if(yCord < _areaStartPoint.Y) (yCord, _areaStartPoint.Y) = (_areaStartPoint.Y, yCord);
                    SelectedArea.XCord1 = (int)_areaStartPoint.X;
                    SelectedArea.YCord1 = (int)_areaStartPoint.Y;
                    SelectedArea.Width = (int)xCord - SelectedArea.XCord1;
                    SelectedArea.Height = (int)yCord - SelectedArea.YCord1;
                    Border border = MapPageArea.GenerateArea(SelectedArea);
                    field.Children.Add(border);
                    _newArea = border;
                    SetAreaEvents(border, SelectedArea);

                    ToggleAreaInput(true);
                    AreaInfoVisible();
                }
            }
            else
            {

            }
        }
        private void HandleAddStreet_Click(Object sender, RoutedEventArgs e)
        {
            if (AddStreetButton.Content.Equals("Aanpassen"))
            {
                foreach (Grid grid in StreetInfoGrid.Children)
                {
                    foreach (var comp in grid.Children)
                    {
                        if (comp is TextBox textbox) textbox.IsEnabled = true;
                        if (comp is CheckBox checkbox) checkbox.IsEnabled = true;
                    }
                }
                AddStreetButton.Content = "Opslaan";
            }
            else
            {
                GetAddStreetValues();
                if (!_wrongInput)
                {
                    bool hasPower = (bool)StreetHasPower.IsChecked;
                    bool hasDogs = (bool)StreetHasDogs.IsChecked;
                    if (AddStreetButton.Content.Equals("Opslaan"))
                    {
                        _camping.CampingRepository.CampingMapRepository.UpdateStreetByStreetID(hasPower, hasDogs, _streetSurfaceArea, _streetPricePerNightPerPerson, _streetPersons, SelectedStreet.StreetID);
                    }
                    else
                    {
                        _camping.CampingRepository.CampingMapRepository.AddNewStreet(_streetName, hasPower, hasDogs, _streetSurfaceArea, _streetPricePerNightPerPerson, _streetPersons, (int)_xCord1, (int)_yCord1, (int)_xCord2, (int)_yCord2);
                        _streetPoint1 = new Point(-1, -1);
                    }
                    GenerateMap();
                    ResetAfterAddingMapComponent("Street");
                }
            }
        }

        public void GetAddStreetValues()
        {
            _streetName = GetAddStreetNameTextbox(StreetName, _streetName);
            _streetSurfaceArea = GetAddSurfaceArea(StreetSurfaceArea, _streetSurfaceArea);
            _streetPersons = GetAddAmountOfPeople(StreetPersons, _streetPersons);
            _streetPricePerNightPerPerson = GetAddPricePerNightPerPerson(StreetPricePerNight, _streetPricePerNightPerPerson);
        }

        public void HandleAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            GetAddPlaceValues();
            if (!_wrongInput)
            {
                bool hasPower = false;
                if (PlaceHasPower.IsChecked == true)
                    hasPower = true;
                bool hasDogs = false;
                if (PlaceHasDogs.IsChecked == true)
                    hasDogs = true;
                Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                Area area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                Place place = new Place(_placePlaceID, hasPower, street.StreetID, area.AreaID, hasDogs, _placeSurfaceArea, _placePersons, _placePricePerNightPerPerson, _xPressed, _yPressed);
                if (_editPlaceBool)
                {
                    _camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(place.PlaceID, street.StreetID, area.AreaID, hasPower, _placeSurfaceArea, _placePricePerNightPerPerson, _placePersons, hasDogs);
                    //Update extend table
                }
                else
                {
                    _camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                    _camping.CampingRepository.CampingMapRepository.AddExtend(place.PlaceID,
                                                          GetValueFromExtendComboBox(PlacePowerComboBox),
                                                          GetValueFromExtendComboBox(PlaceDogsComboBox),
                                                          GetValueFromExtendComboBox(PlaceSurfaceAreaComboBox),
                                                          GetValueFromExtendComboBox(PlacePricePerNightPerPersonComboBox),
                                                          GetValueFromExtendComboBox(PlacePersonsComboBox));
                    _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                }
                ResetAfterAddingMapComponent("Place");
            }
        }
        private bool? GetValueFromExtendComboBox(ComboBox combobox)
        {
            bool? extendBool = null;
            if(combobox.SelectedIndex == 0) extendBool = true;
            else if(combobox.SelectedIndex == 1) extendBool = false;
            return extendBool;
        }

        public void HandleCancelAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            ResetAfterAddingMapComponent("Place");
            ResetAfterAddingMapComponent("Street");
        }

        private void ResetAfterAddingMapComponent(string mapComponent)
        {
            UIElementCollection collection = null;
            if (mapComponent.Contains("Place")) collection = PlaceInfoGrid.Children;
            if (mapComponent.Contains("Street"))
            {
                collection = StreetInfoGrid.Children;
                StreetInfoGrid.Visibility = Visibility.Collapsed;
            }

            if(collection != null && collection.Count > 0)
            {
                foreach (Grid grid in collection)
                {
                    foreach (var component in grid.Children)
                    {
                        if (component is TextBox textbox)
                        {
                            StaticUIMethods.ResetTextboxBorder(textbox);
                        }
                        if (component is Border combobox)
                        {
                            ResetComboBoxBorder(combobox);
                        }
                    }
                }
                GenerateMap();
            }
        }

        private void GetAddPlaceValues()
        {
            GetAddPlaceID();
            GetAddStreetID();
            GetAddAreaID();
            _placePersons = GetAddAmountOfPeople(PlacePersons, _placePersons);
            _placeSurfaceArea = GetAddSurfaceArea(PlaceSurfaceArea, _placeSurfaceArea);
            _placePricePerNightPerPerson = GetAddPricePerNightPerPerson(PlacePricePerNight, _placePricePerNightPerPerson);
    }

        private int GetAddAmountOfPeople(TextBox textbox, int amountOfPeople)
        {
            return GetAddTextBox(textbox, amountOfPeople);
        }

        private double GetAddPricePerNightPerPerson(TextBox textbox, double pricePerNightPerPerson)
        {
            double number;
            if (double.TryParse(textbox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(textbox.Text))
                pricePerNightPerPerson = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                _wrongInput = true;
            }
            return pricePerNightPerPerson;
        }

        private int GetAddSurfaceArea(TextBox textbox, int surfaceArea)
        {
            return GetAddTextBox(textbox, surfaceArea);
        }

        private void GetAddPlaceID()
        {
            _placePlaceID = GetAddTextBox(PlacePlaceID, _placePlaceID);
            List<Place> places = _camping.Places.Where(i => i.PlaceID == _placePlaceID).ToList();
            if (places.Count > 0 && !_editPlaceBool)
            {
                StaticUIMethods.SetErrorTextboxBorder(PlacePlaceID);
                _wrongInput = true;
                _placePlaceID = -1;
            }
        }

        private void GetAddStreetID()
        {
            if (PlaceStreetComboBox.SelectedItem != null)
            {
                Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                _placeStreetID = street.StreetID;
            }
            else
            {
                SetErrorComboBoxBorder(PlaceStreetBorder);
                _wrongInput = true;
            }
        }

        private void GetAddAreaID()
        {
            if (PlaceAreaComboBox.SelectedItem != null)
            {
                Area area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                _placeAreaID = area.AreaID;
            }
            else
            {
                SetErrorComboBoxBorder(PlaceAreaBorder);
                _wrongInput = true;
            }
        }

        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetTextboxBorder(textbox);
                _wrongInput = false;
            }
        }
        private string GetAddStreetNameTextbox(TextBox textbox, string name)
        {
            if (!string.IsNullOrEmpty(textbox.Text))
            {
                name = textbox.Text;
            }
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                _wrongInput = true;
            }
            return name;
        }

        private int GetAddTextBox(TextBox textbox, int editNumber)
        {
            int number;
            if (int.TryParse(textbox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(textbox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                editNumber = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                _wrongInput = true;
            }
            return editNumber;
        }
        private void HandleCancelAddArea_Click(object sender, RoutedEventArgs e)
        {
            _areaStartPoint.X = -1;
            _areaStartPoint.Y = -1;
            SelectedArea = null;
            _newArea = null;
            GenerateMap();
            AreaInfoGrid.Visibility = Visibility.Hidden;
        }

        private void HandleAddArea_Click(object sender, RoutedEventArgs e)
        {
            ResertAreaBorders();
            GetAddAreaValues();
            if(!_wrongInput)
            {
                _camping.CampingRepository.CampingMapRepository.AddNewArea(SelectedArea);
                AreaInfoGrid.Visibility = Visibility.Hidden;
                _areaStartPoint.X = -1;
                _areaStartPoint.Y = -1;
                GenerateMap();
            }
        }
        private void GetAddAreaValues()
        {
            _wrongInput = false;
            SelectedArea.Name = GetAddStreetNameTextbox(AreaName, _areaName);
            GetColorID();
            SelectedArea.AmountOfPeople = GetAddAmountOfPeople(AreaAmountOfPeople, _areaPersons);
            SelectedArea.PricePerNightPerPerson = GetAddPricePerNightPerPerson(AreaPrice, _areaPricePerNightPerPerson);
            SelectedArea.SurfaceArea = GetAddSurfaceArea(AreaPlaceSurfaceArea, _areaSurfaceArea);
            SelectedArea.Power = (bool)AreaPower.IsChecked;
            SelectedArea.Dogs = (bool)AreaDogs.IsChecked;
            
        }
        private void SelectedArea_ColorChange(object sender, SelectionChangedEventArgs e)
        {
            if (_newArea != null)
            {
                Canvas usedCanvas = (Canvas)_newArea.Child;
                if (AreaColor.SelectedItem != null)
                {
                    int color = int.Parse(AreaColor.SelectedValue.ToString());
                    usedCanvas.Background = new SolidColorBrush(StaticUIMethods.GetColorFromInt(color));
                }
            }
        }

        public void SetErrorComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.Red;
            border.BorderThickness = new Thickness(3, 3, 3, 3);
        }

        public void ResetComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.White;
            border.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void PlaceStreetAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            Border border = PlaceAreaBorder;
            if (combobox.Name.Equals(PlaceStreetComboBox.Name)) border = PlaceStreetBorder;
            if (border.BorderBrush.Equals(Brushes.Red))
            {
                ResetComboBoxBorder(border);
                _wrongInput = false;
            }
            if (PlaceStreetComboBox.SelectedItem != null && PlaceAreaComboBox.SelectedItem != null)
            {
                EnableExtendComboBoxes(true);

                foreach (Grid grid in PlaceInfoGrid.Children)
                {

                    foreach (var component in grid.Children)
                    {
                        if (component is ComboBox com)
                        {
                            if (com.Name != PlaceStreetComboBox.Name && com.Name != PlaceAreaComboBox.Name) {
                                HandleExtending(com);
                            }
                        }
                    }
                }
            }
            else
            {
                EnableExtendComboBoxes(false);
            }
        }

        private void EnableExtendComboBoxes(bool extend)
        {
            foreach (Grid grid in PlaceInfoGrid.Children)
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

        private void HandleExtendChange_Click(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            HandleExtending(comboBox);
        }

        public void HandleExtending(ComboBox comboBox)
        {
            Area area = null;
            Street street = null;

            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                if (PlaceAreaComboBox.SelectedItem != null)
                    area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                if (PlaceStreetComboBox.SelectedItem != null)
                    street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
            }

            if (street != null || area != null)
            {
                foreach (Grid grid in PlaceInfoGrid.Children)
                {
                    foreach (var component in grid.Children)
                    {
                        if (component is TextBox textbox)
                            SetTextboxValues(comboBox, textbox, street, area);
                        else if (component is CheckBox checkbox)
                            SetCheckboxValues(comboBox, checkbox, street, area);
                    }
                }
            }
        }

        private void SetTextboxValues(ComboBox comboBox, TextBox textbox, Street street, Area area)
        {
            int selectedIndex = comboBox.SelectedIndex;
            if (comboBox.Name.Contains("SurfaceArea") && textbox.Name.Contains("SurfaceArea"))
            {
                textbox.Text = selectedIndex == 0 ? street.SurfaceArea.ToString() : selectedIndex == 1 ? area.SurfaceArea.ToString() : null;
                textbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("PricePerNight") && textbox.Name.Contains("PricePerNight"))
            {
                textbox.Text = selectedIndex == 0 ? street.PricePerNightPerPerson.ToString() : selectedIndex == 1 ? area.PricePerNightPerPerson.ToString() : null;
                textbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("Persons") && textbox.Name.Contains("Persons"))
            {
                textbox.Text = selectedIndex == 0 ? street.AmountOfPeople.ToString() : selectedIndex == 1 ? area.AmountOfPeople.ToString() : null;
                textbox.IsEnabled = selectedIndex == 2;
            }
        }

        private void SetCheckboxValues(ComboBox comboBox, CheckBox checkbox, Street street, Area area)
        {
            int selectedIndex = comboBox.SelectedIndex;
            if (comboBox.Name.Contains("Power") && checkbox.Name.Contains("Power"))
            {
                checkbox.IsChecked = selectedIndex == 0 ? street.Power : selectedIndex == 1 ? area.Power : null;
                checkbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("Dogs") && checkbox.Name.Contains("Dogs"))
            {
                checkbox.IsChecked = selectedIndex == 0 ? street.Dogs : selectedIndex == 1 ? area.Dogs : null;
                checkbox.IsEnabled = selectedIndex == 2;
            }
        }

        private void Handle_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is the Escape key
            if (e.Key == Key.Escape)
            {
                if (_selectedMapButton.Contains("Place")) ResetAfterAddingMapComponent("Place");;
                if(_selectedMapButton.Contains("Area"))
                {
                    
                }
                if(_selectedMapButton.Contains("Street")) ResetAfterAddingMapComponent("Street");
                if (_selectedMapButton.Contains("View"))
                {
                    StreetInfoGrid.Visibility = Visibility.Hidden;
                    AreaInfoGrid.Visibility = Visibility.Hidden;
                    PlaceInfoGrid.Visibility = Visibility.Hidden;
                }
            }
        }

        private void MakeMapComponentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Style applyStyle = (Style)button.FindResource("ButtonStyle1Apply");
            Style editStyle = (Style)button.FindResource("ButtonStyle1Edit");
            if (button.Style.Equals(editStyle))
            {
                foreach (Button gridButton in MapGridButtons.Children)
                {
                    gridButton.Style = editStyle;
                }
                button.Style = applyStyle;
                _selectedMapButton = button.Name;
            }
            else
            {
                button.Style = editStyle;
                GenerateMap();
                _selectedMapButton = "View";
            }
        }
        private void AreaInfoVisible()
        {
            PlaceInfo.Visibility = Visibility.Hidden;
            StreetInfo.Visibility = Visibility.Hidden;
            AreaInfo.Visibility = Visibility.Visible;
            PlaceInfoGrid.Visibility = Visibility.Hidden;
            StreetInfoGrid.Visibility = Visibility.Hidden;
            AreaInfoGrid.Visibility = Visibility.Visible;
        }
        private void HandleAreaClick()
        {
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
        private void SetAreaComboBox()
        {
            AreaColor.Items.Clear();
            List<int> activeColors =_areas.Where(a => SelectedArea != null ? a.Color != SelectedArea.Color : true).Select(a => a.Color).ToList();
            List<int> possibelColors = Enumerable.Range(0, StaticUIMethods.ColorCount).Except(activeColors).ToList();
            for (int i = 0; i < possibelColors.Count; i++)
            {
                AreaColor.Items.Add(new {Text = StaticUIMethods.GetColorNameFromInt(possibelColors[i]), Value = possibelColors[i]});
            }
            if( SelectedArea != null ) AreaColor.SelectedValue = SelectedArea.Color;
        }
        private void ToggleAreaInput(bool enabled)
        {
            AreaName.IsEnabled = enabled;
            AreaColor.IsEnabled = enabled;
            AreaPower.IsEnabled = enabled;
            AreaDogs.IsEnabled = enabled;
            AreaPlaceSurfaceArea.IsEnabled = enabled;
            AreaPrice.IsEnabled = enabled;
            AreaAmountOfPeople.IsEnabled = enabled;
            if (enabled) AddAreaButton.Visibility = Visibility.Visible;
            else AddAreaButton.Visibility = Visibility.Hidden;
        }
        private void GetColorID()
        {
            if (AreaColor.SelectedItem != null)
            {
                SelectedArea.Color = (int)AreaColor.SelectedValue;
            }
            else
            {
                SetErrorComboBoxBorder(AreaColorBorder);
                _wrongInput = true;
            }
        }
        private void ResertAreaBorders()
        {
            StaticUIMethods.ResetTextboxBorder(AreaName);
            StaticUIMethods.ResetTextboxBorder(AreaPrice);
            StaticUIMethods.ResetTextboxBorder(AreaAmountOfPeople);
            StaticUIMethods.ResetTextboxBorder(AreaPlaceSurfaceArea);
            ResetComboBoxBorder(AreaColorBorder);
        }
    }
}
