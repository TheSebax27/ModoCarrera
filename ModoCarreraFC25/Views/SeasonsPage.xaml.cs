using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;
using System.Collections.ObjectModel;

namespace ModoCarreraFC25.Views
{
    public partial class SeasonsPage : ContentPage
    {
        private readonly IDataService _dataService;
        private ObservableCollection<Season> _seasons;
        private List<Career> _careers;
        private Career _selectedCareer;
        private string _careerIdToLoad; // Changed from int to string

        // Constructor modificado para aceptar careerId como string
        public SeasonsPage(IDataService dataService, string careerId)
        {
            InitializeComponent();
            _dataService = dataService;
            _careerIdToLoad = careerId;
            _seasons = new ObservableCollection<Season>();
            SeasonsCollectionView.ItemsSource = _seasons;
        }

        // Constructor original mantenido para compatibilidad
        public SeasonsPage(IDataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            _careerIdToLoad = null; // Changed from -1 to null
            _seasons = new ObservableCollection<Season>();
            SeasonsCollectionView.ItemsSource = _seasons;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCareers();

            // Si se pasó un careerId específico, seleccionarlo automáticamente
            if (!string.IsNullOrEmpty(_careerIdToLoad))
            {
                await SelectCareerById(_careerIdToLoad);
            }
        }

        private async Task SelectCareerById(string careerId)
        {
            var career = _careers?.FirstOrDefault(c => c.Id == careerId); // Now comparing string to string
            if (career != null)
            {
                var index = _careers.IndexOf(career);
                CareerPicker.SelectedIndex = index;
                _selectedCareer = career;
                await LoadSeasons();
            }
        }

        private async Task LoadCareers()
        {
            try
            {
                _careers = await _dataService.GetCareersAsync();
                CareerPicker.ItemsSource = _careers.Select(c => c.ManagerName + " - " + c.InitialClub).ToList();

                // Solo seleccionar automáticamente si no hay carrera preseleccionada
                if (_careers.Any() && string.IsNullOrEmpty(_careerIdToLoad))
                {
                    CareerPicker.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar carreras: {ex.Message}", "OK");
            }
        }

        private async void OnCareerSelected(object sender, EventArgs e)
        {
            if (CareerPicker.SelectedIndex >= 0 && CareerPicker.SelectedIndex < _careers.Count)
            {
                _selectedCareer = _careers[CareerPicker.SelectedIndex];
                await LoadSeasons();
            }
        }

        private async Task LoadSeasons()
        {
            if (_selectedCareer == null) return;

            try
            {
                _seasons.Clear();
                var orderedSeasons = _selectedCareer.Seasons.OrderBy(s => s.Year);
                foreach (var season in orderedSeasons)
                {
                    _seasons.Add(season);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar temporadas: {ex.Message}", "OK");
            }
        }

        private async void OnAddSeasonClicked(object sender, EventArgs e)
        {
            if (_selectedCareer == null)
            {
                await DisplayAlert("Advertencia", "Selecciona una carrera primero", "OK");
                return;
            }

            await ShowSeasonDialog(null);
        }

        private async void OnEditSeasonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Season season)
            {
                await ShowSeasonDialog(season);
            }
        }

        private async void OnSeasonSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Season selectedSeason)
            {
                await ShowSeasonDialog(selectedSeason);
                SeasonsCollectionView.SelectedItem = null;
            }
        }

        private async Task ShowSeasonDialog(Season season)
        {
            bool isEdit = season != null;
            season ??= new Season { CareerId = _selectedCareer.Id, Club = _selectedCareer.InitialClub };

            // Crear una página modal para el formulario
            var modalPage = new ContentPage
            {
                Title = isEdit ? "Editar Temporada" : "Nueva Temporada"
            };

            var scrollView = new ScrollView();
            var mainStack = new StackLayout
            {
                Padding = 20,
                Spacing = 15
            };

            // Create entries
            var yearEntry = new Entry
            {
                Text = season.Year.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Año"
            };
            var clubEntry = new Entry
            {
                Text = season.Club,
                Placeholder = "Club"
            };
            var positionEntry = new Entry
            {
                Text = season.LeaguePosition.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Posición en Liga"
            };
            var gamesEntry = new Entry
            {
                Text = season.GamesPlayed.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Partidos Jugados"
            };
            var winsEntry = new Entry
            {
                Text = season.Wins.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Victorias"
            };
            var drawsEntry = new Entry
            {
                Text = season.Draws.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Empates"
            };
            var lossesEntry = new Entry
            {
                Text = season.Losses.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Derrotas"
            };
            var goalsForEntry = new Entry
            {
                Text = season.GoalsFor.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Goles a Favor"
            };
            var goalsAgainstEntry = new Entry
            {
                Text = season.GoalsAgainst.ToString(),
                Keyboard = Keyboard.Numeric,
                Placeholder = "Goles en Contra"
            };
            var notesEntry = new Entry
            {
                Text = season.Notes,
                Placeholder = "Notas"
            };

            // Add form fields
            mainStack.Children.Add(new Label { Text = "Año:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(yearEntry);

            mainStack.Children.Add(new Label { Text = "Club:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(clubEntry);

            mainStack.Children.Add(new Label { Text = "Posición Liga:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(positionEntry);

            mainStack.Children.Add(new Label { Text = "Partidos Jugados:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(gamesEntry);

            mainStack.Children.Add(new Label { Text = "Victorias:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(winsEntry);

            mainStack.Children.Add(new Label { Text = "Empates:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(drawsEntry);

            mainStack.Children.Add(new Label { Text = "Derrotas:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(lossesEntry);

            mainStack.Children.Add(new Label { Text = "Goles a Favor:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(goalsForEntry);

            mainStack.Children.Add(new Label { Text = "Goles en Contra:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(goalsAgainstEntry);

            mainStack.Children.Add(new Label { Text = "Notas:", FontAttributes = FontAttributes.Bold });
            mainStack.Children.Add(notesEntry);

            // Buttons
            var buttonStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var saveButton = new Button
            {
                Text = "Guardar",
                BackgroundColor = Color.FromArgb("#00D4FF"),
                TextColor = Colors.White,
                WidthRequest = 100
            };

            var cancelButton = new Button
            {
                Text = "Cancelar",
                BackgroundColor = Color.FromArgb("#DC143C"),
                TextColor = Colors.White,
                WidthRequest = 100
            };

            if (isEdit)
            {
                var deleteButton = new Button
                {
                    Text = "Eliminar",
                    BackgroundColor = Color.FromArgb("#FF6B6B"),
                    TextColor = Colors.White,
                    WidthRequest = 100
                };

                deleteButton.Clicked += async (s, e) =>
                {
                    bool confirm = await DisplayAlert("Confirmar", "¿Eliminar esta temporada?", "Sí", "No");
                    if (confirm)
                    {
                        _selectedCareer.Seasons.Remove(season);
                        _seasons.Remove(season);
                        await _dataService.SaveCareerAsync(_selectedCareer);
                        await Navigation.PopAsync();
                        await DisplayAlert("Éxito", "Temporada eliminada", "OK");
                    }
                };

                buttonStack.Children.Add(deleteButton);
            }

            saveButton.Clicked += async (s, e) =>
            {
                try
                {
                    // Validar y asignar valores
                    season.Year = int.TryParse(yearEntry.Text, out int year) ? year : DateTime.Now.Year;
                    season.Club = clubEntry.Text ?? "";
                    season.LeaguePosition = int.TryParse(positionEntry.Text, out int pos) ? pos : 0;
                    season.GamesPlayed = int.TryParse(gamesEntry.Text, out int games) ? games : 0;
                    season.Wins = int.TryParse(winsEntry.Text, out int wins) ? wins : 0;
                    season.Draws = int.TryParse(drawsEntry.Text, out int draws) ? draws : 0;
                    season.Losses = int.TryParse(lossesEntry.Text, out int losses) ? losses : 0;
                    season.GoalsFor = int.TryParse(goalsForEntry.Text, out int gf) ? gf : 0;
                    season.GoalsAgainst = int.TryParse(goalsAgainstEntry.Text, out int ga) ? ga : 0;
                    season.Notes = notesEntry.Text ?? "";

                    if (!isEdit)
                    {
                        _selectedCareer.Seasons.Add(season);
                        _seasons.Add(season);
                    }

                    await _dataService.SaveCareerAsync(_selectedCareer);
                    await LoadSeasons();
                    await Navigation.PopAsync();
                    await DisplayAlert("Éxito", isEdit ? "Temporada actualizada" : "Temporada agregada", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
                }
            };

            cancelButton.Clicked += async (s, e) =>
            {
                await Navigation.PopAsync();
            };

            buttonStack.Children.Add(saveButton);
            buttonStack.Children.Add(cancelButton);
            mainStack.Children.Add(buttonStack);

            scrollView.Content = mainStack;
            modalPage.Content = scrollView;

            await Navigation.PushAsync(modalPage);
        }
    }
}