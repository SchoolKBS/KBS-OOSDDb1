using CampingCore;
using CampingDataAccess;
using CampingUI.GenerateComponentsMap;
using CampingUI.NewFolder;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
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
using Transform = CampingUI.NewFolder.Transform;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Camping _camping;
        private List<Area> _areas;
        private List<Place> _places;
        private List<Street> _streets;
        private int _placeSurfaceArea, _placePersons, _placePlaceID, _placeStreetID, _placeAreaID, SelectedPlace, _yPressed, _xPressed;
        private int _streetSurfaceArea, _streetPersons;
        private double _placePricePerNightPerPerson, _streetPricePerNightPerPerson, _xCord1, _yCord1, _xCord2, _yCord2;
        private string _streetName;
        private Canvas _previousSelectedCanvas;
        private bool _editPlaceBool, _wrongInput;
        private string _selectedMapButton = "View";
        private Point _streetPoint1 = new Point(-1, -1), _streetPoint2;
        private bool _AddPlace = false;
        private bool _AddStreet = false;
        private List<Border> AreaBorderList;
        private List<Border> PlaceBorderList;
        public Area SelectedArea { get; private set; }
        private Street _streetSelected;
        public MainPage(Camping camping)
        {
            InitializeComponent();
            AreaBorderList = new List<Border>();
            PlaceBorderList = new List<Border>();
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
                
                foreach(var comp in list)
                {
                   if(comp is Area && !comp.Equals(SelectedArea))
                    {
                        AreaBorderList.Add(CreateBorder((Area)(object)comp));
                        field.Children.Add(AreaBorderList.Last());
                    }
                    if (comp is Street)
                    {
                        MapPageStreet.GenerateStreet((Street)(object)comp, Brushes.Black);
                        Line line = MapPageStreet.GetLine();
                        field.Children.Add(line);
                        field.Children.Add(MapPageStreet.GetTextBlock());
                        line.MouseDown += (sender, e) => { HandleStreetClick((Street)(object)comp); };
                    }
                        
                    if (comp is Place)
                    {
                        PlaceBorderList.Add(GeneratePlace((Place)(object)comp, Brushes.Black, true));
                        //GeneratePlace((Place)(object)comp, Brushes.Black, true);
                    }
                }
            }
        }
        private void ClearAreaSelection()
        {
            for(int i =0; i < AreaBorderList.Count; i++)
            {
                MapPageArea.DeselectBorder(AreaBorderList[i], _areas[i]);
            }
        }
        private void ClearSelection()
        {
            ClearAreaSelection();
            PlaceBorderList.Clear();
            AreaBorderList.Clear();
            PlaceInfo.Visibility = Visibility.Hidden;
            AreaInfo.Visibility = Visibility.Hidden;
        }
        private Border CreateBorder(Area comp)
        {
            Border border = MapPageArea.GenerateArea((Area)(object)comp);
            border.MouseLeftButtonDown += (sender, e) =>
            {
                if (_selectedMapButton == "View")
                {
                    SelectedArea = (Area)(object)comp;
                    ClearAreaSelection();
                    HandleAreaClick();
                    border = MapPageArea.SelectBorder(border, comp);
                }
                else
                {
                    SelectedArea = null;
                    ClearAreaSelection();
                }
            };
            return border;
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
                Background = brush
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
                    if(canvasPlace.Background.ToString() != "#FF018B8B")
                    canvasPlace.Background = Brushes.DarkCyan; // Change the background color on hover
                };

                canvasPlace.MouseLeave += (sender, e) =>
                {
                    if (place.PlaceID != SelectedPlace && canvasPlace.Background.ToString() != "#FF018B8B")
                    {
                        canvasPlace.Background = Brushes.Black;
                    }
                };
            }

            canvasPlace.MouseLeftButtonDown += (sender, e) =>
            {
                if (_selectedMapButton.Contains("View"))
                {

                    if (_previousSelectedCanvas != null)
                    {
                        _previousSelectedCanvas.Background = Brushes.Black;
                    }

                    canvasPlace.Background = Brushes.DarkCyan;
                    _previousSelectedCanvas = canvasPlace;
                    HandlePlaceClick(place, false);
                }
            };
            return border;
        }

        public void HandlePlaceClick(Place place, bool AddPlaceBool)
        {
            PlaceInfoGrid.Visibility = Visibility.Visible;
            StreetInfoGrid.Visibility = Visibility.Hidden;
            if (!AddPlaceBool)
            {
                ClearSelection();
/*                field.Children.Clear();
                GenerateMap();*/
            }
            else
            {
                ResetInputFields();
                AddPlaceButton.Content = "Toevoegen";
                _editPlaceBool = false;
                PlaceInfoGrid.Visibility = Visibility.Visible;
                AddPlaceButton.Visibility = Visibility.Visible;
            }
        }
        public void HandleStreetClick(Street street)
        {
            if(_streetSelected != null)
            {
                HighLightPlacesStreet(_streetSelected, Brushes.Black);
            }
            _streetSelected = street;
            PlaceInfo.Visibility = Visibility.Hidden;
            StreetInfo.Visibility = Visibility.Visible;
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
            HighLightPlacesStreet(street, Brushes.DarkCyan);
            AddStreetButton.Content = "Aanpassen";
            //Als annuleren -> onzichtbaar maken en deselecteren

            //Als toevoegen -> Check op alle invoervelden
            //              -> Update straat gegevens in de database
            // 
        }

        private void HighLightPlacesStreet(Street street, SolidColorBrush color)
        {
            List<Place> places = _places.Where(p => p.StreetID == street.StreetID).ToList();
            if (places.Count() > 0)
            {
                foreach (var comp in field.Children)
                {
                    if (comp is Border placeBlock && placeBlock.Child is Canvas canvas && canvas.Name.Contains("Place_"))
                    {
                        foreach (Place placeData in places)
                        {
                            if (canvas.Name.Equals("Place_" + placeData.PlaceID.ToString()))
                            {
                                canvas.Background = color;
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
            foreach (Street street in _camping.CampingRepository.CampingMapRepository.GetStreets())
            {
                PlaceStreetComboBox.Items.Add(street.Name);
            }
            foreach (Area area in _camping.CampingRepository.CampingMapRepository.GetAreas())
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
                field.Children.Clear();
                GenerateMap();
                Point p = Mouse.GetPosition(field);
                List<Area> areas = _camping.CampingRepository.CampingMapRepository.GetAreas();
                List<Street> streets = _camping.CampingRepository.CampingMapRepository.GetStreets();
                List<Place> places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                foreach (Place place in places)
                {
                    place.XCord -= 15;
                    place.YCord -= 15;
                }
                _xPressed = (int)Math.Round(p.X) - 15;
                _yPressed = (int)Math.Round(p.Y) - 15;
                List<Area> PlaceWithinAreas = areas.Where(i => i.XCord1 <= (_xPressed - 15))
                                                   .Where(i => i.XCord1 + i.Width >= (_xPressed + 45))
                                                   .Where(i => i.YCord1 <= (_yPressed - 15))
                                                   .Where(i => i.YCord1 + i.Height >= (_yPressed + 45))
                                                   .ToList();
                List<Place> placesNotInNewPlaceBorder = places.Where(i => i.XCord >= (_xPressed - 45) && i.XCord <= (_xPressed + 45))
                                                              .Where(i => i.YCord >= (_yPressed - 45) && i.YCord <= (_yPressed + 45))
                                                              .ToList();
/*                if (PlaceWithinAreas.Count == 1 && placesNotInNewPlaceBorder.Count == 0)
                {*/
                    field.Children.Clear();
                    ClearSelection();
                    GenerateMap();
                    _camping.Places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                    int i = 1;
                    if(_camping.Places.Count > 0)
                    {
                        i = _camping.Places.Last().PlaceID + 1;
                    }
                   

                    Place place1 = new Place(0, false, 1, 1, false, 0, 0, 0, _xPressed, _yPressed);
                    GeneratePlace(place1, Brushes.Gray, false);
                    EnableExtendComboBoxes(false);
                    HandlePlaceClick(place1, true);
                //}
            }
            if (_selectedMapButton.Contains("Street"))
            {
                field.Children.Clear();
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
            if (_selectedMapButton.Contains("View"))
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
                        _camping.CampingRepository.CampingMapRepository.UpdateStreetByStreetID(hasPower, hasDogs, _streetSurfaceArea, _streetPricePerNightPerPerson, _streetPersons, _streetSelected.StreetID);
                    }
                    else
                    {
                        _camping.CampingRepository.CampingMapRepository.AddNewStreet(_streetName, hasPower, hasDogs, _streetSurfaceArea, _streetPricePerNightPerPerson, _streetPersons, (int)_xCord1, (int)_yCord1, (int)_xCord2, (int)_yCord2);
                        _streetPoint1 = new Point(-1, -1);
                    }
                    GenerateMap();
                    ResetAfterAddStreet();
                }
            }
        }
        private void HandleAddStreet()
        {
            PlaceInfo.Visibility = Visibility.Visible;
            StreetInfo.Visibility = Visibility.Hidden;
            ResetInputFields();
            _editPlaceBool = false;
            StreetInfoGrid.Visibility = Visibility.Visible;
            
            
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
                ResetAfterAddPlace();
            }
        }
        private bool? GetValueFromExtendComboBox(ComboBox combobox)
        {
            bool? extendBool = null;
            if(combobox.SelectedIndex == 0)
            {
                extendBool = true;
            }
            else if(combobox.SelectedIndex == 1)
            {
                extendBool = false;
            }
            return extendBool;
        }

        public void HandleCancelAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            ResetAfterAddPlace();
            ResetAfterAddStreet();
        }

        private void ResetAfterAddPlace()
        {
            foreach (Grid grid in PlaceInfoGrid.Children)
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
            ClearSelection();
            field.Children.Clear();
            GenerateMap();
        }

        private void ResetAfterAddStreet()
        {
            foreach (Grid grid in PlaceInfoGrid.Children)
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
            StreetInfoGrid.Visibility = Visibility.Collapsed;
            field.Children.Clear();
            GenerateMap();
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

        public void SetErrorComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.Red;
            border.BorderThickness = new Thickness(3, 3, 3, 3);
        }

        private void MakeAreaButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

        }

        public void ResetComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.White;
            border.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void HandleExtendChange_Click(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            HandleExtending(comboBox);
        }

        public void HandleExtending(ComboBox comboBox)
        {
            Area area = null;
            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                if (PlaceAreaComboBox.SelectedItem != null)
                {
                    area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                }
            }

            Street street = null;
            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                if (PlaceStreetComboBox.SelectedItem != null)
                {
                    street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                }
            }

            if (street != null || area != null)
            {
                foreach (Grid grid in PlaceInfoGrid.Children)
                {

                    foreach (var component in grid.Children)
                    {
                        if (component is TextBox textbox)
                        {
                            if (comboBox.Name.Contains("SurfaceArea") && textbox.Name.Contains("SurfaceArea"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    textbox.Text = street.SurfaceArea.ToString();
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    textbox.Text = area.SurfaceArea.ToString();
                                }
                                textbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    textbox.IsEnabled = true;
                                }
                            }
                            if (comboBox.Name.Contains("PricePerNight") && textbox.Name.Contains("PricePerNight"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    textbox.Text = street.PricePerNightPerPerson.ToString();
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    textbox.Text = area.PricePerNightPerPerson.ToString();
                                }
                                textbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    textbox.IsEnabled = true;
                                }
                            }
                            if (comboBox.Name.Contains("Persons") && textbox.Name.Contains("Persons"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    textbox.Text = street.AmountOfPeople.ToString();
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    textbox.Text = area.AmountOfPeople.ToString();
                                }
                                textbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    textbox.IsEnabled = true;
                                }
                            }
                        }

                        if (component is CheckBox checkbox)
                        {
                            if (comboBox.Name.Contains("Power") && checkbox.Name.Contains("Power"))
                            {

                                if (comboBox.SelectedIndex == 0)
                                {
                                    checkbox.IsChecked = street.Power;
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    checkbox.IsChecked = area.Power;
                                }
                                checkbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    checkbox.IsEnabled = true;
                                }
                            }
                            if (comboBox.Name.Contains("Dogs") && checkbox.Name.Contains("Dogs"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    checkbox.IsChecked = street.Dogs;
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    checkbox.IsChecked = area.Dogs;
                                }
                                checkbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    checkbox.IsEnabled = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Handle_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is the Escape key
            if (e.Key == Key.Escape)
            {

                if (_selectedMapButton.Contains("Place"))
                {
                    ResetAfterAddPlace();
                   /* foreach(Button button in MapGridButtons.Children)
                    {
                        Style editStyle = (Style)button.FindResource("ButtonStyle1Edit");
                        button.Style = editStyle;
                    }
                    _selectedMapButton = "View";*/
                }
                if(StreetInfo.Visibility == Visibility.Visible)
                {
                    StreetInfo.Visibility = Visibility.Hidden;
                }
                else if(AreaInfo.Visibility == Visibility.Visible)
                {
                    AreaInfo.Visibility = Visibility.Hidden;
                    ClearAreaSelection();
                    ChangePlaceBackground(PlaceBorderList, false);
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
                ResetAfterAddPlace();
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
                _selectedMapButton = "View";
            }
        }
        private void HandleAreaClick()
        {
            if(SelectedArea != null)
            {
                AreaName.Content = SelectedArea.Name;
                AreaColor.Content = StaticUIMethods.GetColorNameFromInt(SelectedArea.Color);
                AreaPower.IsChecked = SelectedArea.Power;
                AreaDogs.IsChecked = SelectedArea.Dogs;
                AreaPlaceSurfaceArea.Content = SelectedArea.SurfaceArea;
                AreaPrice.Content = SelectedArea.PricePerNightPerPerson;
                AreaAmountOfPeople.Content = SelectedArea.AmountOfPeople;
                ChangePlaceBackground(PlaceBorderList, false);
                ChangePlaceBackground(SelectAreaPlaces(), true);
                AreaInfo.Visibility = Visibility.Visible;
            }
            else
            {
                ChangePlaceBackground(PlaceBorderList, false);
            }
        }
        private List<Border> SelectAreaPlaces()
        {
            List<Place> places = _places.Where(p => p.AreaID == SelectedArea.AreaID).ToList();
            List<Border> borders = PlaceBorderList.IntersectBy(places.Select(p => p.PlaceID), b =>
            {
                string[] strings = b.Name.Split('_');
                return int.Parse(strings[1]);
            }).ToList();
            return borders;
        }

        private void ChangePlaceBackground(List<Border> PlaceBorders, bool Select)
        {
            foreach(Border PlaceBorder in PlaceBorders)
            {
                Canvas child = (Canvas)PlaceBorder.Child;
                if (Select) child.Background = (Brush)new BrushConverter().ConvertFromString("#FF018B8B");
                else child.Background = Brushes.Black;
            }
        }
    }
}
