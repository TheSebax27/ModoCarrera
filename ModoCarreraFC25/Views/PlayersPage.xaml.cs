using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;

namespace ModoCarreraFC25
{
    public partial class PlayersPage : ContentPage
    {
        private readonly IDataService _dataService;
        private List<Career> _careers;
        private Career _selectedCareer;
        private Season _selectedSeason;

        public PlayersPage(IDataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            LoadCareers();
        }

        private async void LoadCareers()
        {
            try
            {
                _careers = await _dataService.GetCareersAsync();
                CareerPicker.ItemsSource = _careers.Select(c => $"{c.ManagerName} - {c.InitialClub}").ToList();

                if (!_careers.Any())
                {
                    EmptyLabel.Text = "No hay carreras disponibles. Crea una carrera primero.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar carreras: {ex.Message}", "OK");
            }
        }

        private void OnCareerSelected(object sender, EventArgs e)
        {
            if (CareerPicker.SelectedIndex >= 0)
            {
                _selectedCareer = _careers[CareerPicker.SelectedIndex];
                SeasonPicker.ItemsSource = _selectedCareer.Seasons.Select(s => $"Temporada {s.Year} - {s.Club}").ToList();
                SeasonLayout.IsVisible = true;

                if (!_selectedCareer.Seasons.Any())
                {
                    EmptyLabel.Text = "Esta carrera no tiene temporadas. Crea una temporada primero.";
                    PlayersContainer.Children.Clear();
                    PlayersContainer.Children.Add(EmptyLabel);
                }
            }
        }

        private void OnSeasonSelected(object sender, EventArgs e)
        {
            if (SeasonPicker.SelectedIndex >= 0)
            {
                _selectedSeason = _selectedCareer.Seasons[SeasonPicker.SelectedIndex];
                LoadPlayers();
                AddPlayerBtn.IsVisible = true;
            }
        }

        private void LoadPlayers()
        {
            PlayersContainer.Children.Clear();

            if (!_selectedSeason.Players.Any())
            {
                EmptyLabel.Text = "No hay jugadores en esta temporada";
                PlayersContainer.Children.Add(EmptyLabel);
                return;
            }

            foreach (var player in _selectedSeason.Players.OrderByDescending(p => p.Overall))
            {
                var playerFrame = CreatePlayerCard(player);
                PlayersContainer.Children.Add(playerFrame);
            }
        }

        private Frame CreatePlayerCard(Player player)
        {
            var frame = new Frame
            {
                BackgroundColor = Color.FromHex("#1A2332"),
                BorderColor = Color.FromHex("#00D4FF"),
                CornerRadius = 10,
                Padding = 15,
                Margin = new Thickness(0, 5)
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            // Player Name and Position
            var nameLabel = new Label
            {
                Text = $"{player.Name} ({player.Position})",
                TextColor = Colors.White,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold
            };
            Grid.SetRow(nameLabel, 0);
            Grid.SetColumnSpan(nameLabel, 3);

            // Age, Overall, Potential
            var statsLabel = new Label
            {
                Text = $"Edad: {player.Age} | Overall: {player.Overall} | Potencial: {player.Potential}",
                TextColor = Color.FromHex("#B0C4DE"),
                FontSize = 14
            };
            Grid.SetRow(statsLabel, 1);
            Grid.SetColumnSpan(statsLabel, 3);

            // Goals and Assists
            var performanceLabel = new Label
            {
                Text = $"⚽ {player.Goals} goles | 🅰️ {player.Assists} asistencias | 🎮 {player.GamesPlayed} partidos",
                TextColor = Color.FromHex("#00D4FF"),
                FontSize = 14
            };
            Grid.SetRow(performanceLabel, 2);
            Grid.SetColumn(performanceLabel, 0);

            // Edit Button
            var editButton = new Button
            {
                Text = "✏️",
                BackgroundColor = Color.FromHex("#FFD700"),
                TextColor = Colors.Black,
                FontSize = 16,
                CornerRadius = 20,
                WidthRequest = 40,
                HeightRequest = 40
            };
            editButton.Clicked += async (s, e) => await EditPlayer(player);
            Grid.SetRow(editButton, 2);
            Grid.SetColumn(editButton, 1);

            // Delete Button
            var deleteButton = new Button
            {
                Text = "🗑️",
                BackgroundColor = Color.FromHex("#FF4444"),
                TextColor = Colors.White,
                FontSize = 16,
                CornerRadius = 20,
                WidthRequest = 40,
                HeightRequest = 40
            };
            deleteButton.Clicked += async (s, e) => await DeletePlayer(player);
            Grid.SetRow(deleteButton, 2);
            Grid.SetColumn(deleteButton, 2);

            grid.Children.Add(nameLabel);
            grid.Children.Add(statsLabel);
            grid.Children.Add(performanceLabel);
            grid.Children.Add(editButton);
            grid.Children.Add(deleteButton);

            frame.Content = grid;
            return frame;
        }

        private async void OnAddPlayerClicked(object sender, EventArgs e)
        {
            await ShowPlayerForm(new Player());
        }

        private async Task EditPlayer(Player player)
        {
            await ShowPlayerForm(player);
        }

        private async Task DeletePlayer(Player player)
        {
            var result = await DisplayAlert("Confirmar", $"¿Eliminar a {player.Name}?", "Sí", "No");
            if (result)
            {
                _selectedSeason.Players.Remove(player);
                await _dataService.SaveCareerAsync(_selectedCareer);
                LoadPlayers();
            }
        }

        private async Task ShowPlayerForm(Player player)
        {
            var isNew = string.IsNullOrEmpty(player.Name);
            var title = isNew ? "Agregar Jugador" : "Editar Jugador";

            var nameResult = await DisplayPromptAsync(title, "Nombre del jugador:", initialValue: player.Name ?? "");
            if (string.IsNullOrWhiteSpace(nameResult)) return;

            var positionResult = await DisplayPromptAsync(title, "Posición:", initialValue: player.Position ?? "");
            if (string.IsNullOrWhiteSpace(positionResult)) return;

            var ageResult = await DisplayPromptAsync(title, "Edad:", initialValue: player.Age.ToString(), keyboard: Keyboard.Numeric);
            if (!int.TryParse(ageResult, out int age)) return;

            var overallResult = await DisplayPromptAsync(title, "Overall:", initialValue: player.Overall.ToString(), keyboard: Keyboard.Numeric);
            if (!int.TryParse(overallResult, out int overall)) return;

            var potentialResult = await DisplayPromptAsync(title, "Potencial:", initialValue: player.Potential.ToString(), keyboard: Keyboard.Numeric);
            if (!int.TryParse(potentialResult, out int potential)) return;

            var goalsResult = await DisplayPromptAsync(title, "Goles:", initialValue: player.Goals.ToString(), keyboard: Keyboard.Numeric);
            if (!int.TryParse(goalsResult, out int goals)) return;

            var assistsResult = await DisplayPromptAsync(title, "Asistencias:", initialValue: player.Assists.ToString(), keyboard: Keyboard.Numeric);
            if (!int.TryParse(assistsResult, out int assists)) return;

            var gamesResult = await DisplayPromptAsync(title, "Partidos jugados:", initialValue: player.GamesPlayed.ToString(), keyboard: Keyboard.Numeric);
            if (!int.TryParse(gamesResult, out int games)) return;

            // Update player data
            player.Name = nameResult;
            player.Position = positionResult;
            player.Age = age;
            player.Overall = overall;
            player.Potential = potential;
            player.Goals = goals;
            player.Assists = assists;
            player.GamesPlayed = games;

            if (isNew)
            {
                _selectedSeason.Players.Add(player);
            }

            await _dataService.SaveCareerAsync(_selectedCareer);
            LoadPlayers();
        }
    }
}