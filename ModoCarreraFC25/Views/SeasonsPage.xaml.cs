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

        public SeasonsPage(IDataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            _seasons = new ObservableCollection<Season>();
            SeasonsCollectionView.ItemsSource = _seasons;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCareers();
        }

        private async Task LoadCareers()
        {
            try
            {
                _careers = await _dataService.GetCareersAsync();
                CareerPicker.ItemsSource = _careers.Select(c => c.ManagerName + " - " + c.InitialClub).ToList();

                if (_careers.Any())
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

            // Create entry grid
            var grid = new Grid
            {
                RowSpacing = 10,
                ColumnSpacing = 10,
                Padding = 20
            };

            // Add row definitions
            for (int i = 0; i < 10; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Create entries
            var yearEntry = new Entry { Text = season.Year.ToString(), Keyboard = Keyboard.Numeric };
            var clubEntry = new Entry { Text = season.Club };
            var positionEntry = new Entry { Text = season.LeaguePosition.ToString(), Keyboard = Keyboard.Numeric };
            var gamesEntry = new Entry { Text = season.GamesPlayed.ToString(), Keyboard = Keyboard.Numeric };
            var winsEntry = new Entry { Text = season.Wins.ToString(), Keyboard = Keyboard.Numeric };
            var drawsEntry = new Entry { Text = season.Draws.ToString(), Keyboard = Keyboard.Numeric };
            var lossesEntry = new Entry { Text = season.Losses.ToString(), Keyboard = Keyboard.Numeric };
            var goalsForEntry = new Entry { Text = season.GoalsFor.ToString(), Keyboard = Keyboard.Numeric };
            var goalsAgainstEntry = new Entry { Text = season.GoalsAgainst.ToString(), Keyboard = Keyboard.Numeric };
            var notesEntry = new Entry { Text = season.Notes };

            // Add labels and entries to grid
            grid.Add(new Label { Text = "Año:", VerticalOptions = LayoutOptions.Center }, 0, 0);
            grid.Add(yearEntry, 1, 0);

            grid.Add(new Label { Text = "Club:", VerticalOptions = LayoutOptions.Center }, 0, 1);
            grid.Add(clubEntry, 1, 1);

            grid.Add(new Label { Text = "Posición Liga:", VerticalOptions = LayoutOptions.Center }, 0, 2);
            grid.Add(positionEntry, 1, 2);

            grid.Add(new Label { Text = "Partidos Jugados:", VerticalOptions = LayoutOptions.Center }, 0, 3);
            grid.Add(gamesEntry, 1, 3);

            grid.Add(new Label { Text = "Victorias:", VerticalOptions = LayoutOptions.Center }, 0, 4);
            grid.Add(winsEntry, 1, 4);

            grid.Add(new Label { Text = "Empates:", VerticalOptions = LayoutOptions.Center }, 0, 5);
            grid.Add(drawsEntry, 1, 5);

            grid.Add(new Label { Text = "Derrotas:", VerticalOptions = LayoutOptions.Center }, 0, 6);
            grid.Add(lossesEntry, 1, 6);

            grid.Add(new Label { Text = "Goles a Favor:", VerticalOptions = LayoutOptions.Center }, 0, 7);
            grid.Add(goalsForEntry, 1, 7);

            grid.Add(new Label { Text = "Goles en Contra:", VerticalOptions = LayoutOptions.Center }, 0, 8);
            grid.Add(goalsAgainstEntry, 1, 8);

            grid.Add(new Label { Text = "Notas:", VerticalOptions = LayoutOptions.Center }, 0, 9);
            grid.Add(notesEntry, 1, 9);

            var scrollView = new ScrollView { Content = grid, HeightRequest = 400 };

            var action = await DisplayActionSheet(
                isEdit ? "Editar Temporada" : "Nueva Temporada",
                "Cancelar",
                isEdit ? "Eliminar" : null,
                "Guardar");

            if (action == "Guardar")
            {
                try
                {
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

                    await DisplayAlert("Éxito", isEdit ? "Temporada actualizada" : "Temporada agregada", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
                }
            }
            else if (action == "Eliminar" && isEdit)
            {
                bool confirm = await DisplayAlert("Confirmar", "¿Eliminar esta temporada?", "Sí", "No");
                if (confirm)
                {
                    _selectedCareer.Seasons.Remove(season);
                    _seasons.Remove(season);
                    await _dataService.SaveCareerAsync(_selectedCareer);
                    await DisplayAlert("Éxito", "Temporada eliminada", "OK");
                }
            }
        }
    }
}