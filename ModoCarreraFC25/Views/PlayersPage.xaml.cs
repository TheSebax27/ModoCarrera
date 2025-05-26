using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;

namespace ModoCarreraFC25.Views
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
                if (_careers != null && _careers.Any())
                {
                    CareerPicker.ItemsSource = _careers.Select(c => $"{c.ManagerName} - {c.InitialClub}").ToList();
                }
                else
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
            if (CareerPicker.SelectedIndex >= 0 && _careers != null && _careers.Count > CareerPicker.SelectedIndex)
            {
                _selectedCareer = _careers[CareerPicker.SelectedIndex];

                if (_selectedCareer.Seasons != null && _selectedCareer.Seasons.Any())
                {
                    SeasonPicker.ItemsSource = _selectedCareer.Seasons.Select(s => $"Temporada {s.Year} - {s.Club}").ToList();
                    SeasonLayout.IsVisible = true;
                }
                else
                {
                    EmptyLabel.Text = "Esta carrera no tiene temporadas. Crea una temporada primero.";
                    PlayersContainer.Children.Clear();
                    PlayersContainer.Children.Add(EmptyLabel);
                    SeasonLayout.IsVisible = false;
                    AddPlayerBtn.IsVisible = false;
                }
            }
        }

        private void OnSeasonSelected(object sender, EventArgs e)
        {
            if (SeasonPicker.SelectedIndex >= 0 && _selectedCareer?.Seasons != null &&
                _selectedCareer.Seasons.Count > SeasonPicker.SelectedIndex)
            {
                _selectedSeason = _selectedCareer.Seasons[SeasonPicker.SelectedIndex];
                LoadPlayers();
                AddPlayerBtn.IsVisible = true;
            }
        }

        private void LoadPlayers()
        {
            PlayersContainer.Children.Clear();

            if (_selectedSeason?.Players == null || !_selectedSeason.Players.Any())
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
                Text = $"{player.Name ?? "Sin nombre"} ({player.Position ?? "Sin posición"})",
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
            if (_selectedSeason == null)
            {
                await DisplayAlert("Error", "Debe seleccionar una temporada primero", "OK");
                return;
            }

            await ShowPlayerForm(new Player());
        }

        private async Task EditPlayer(Player player)
        {
            if (player == null) return;
            await ShowPlayerForm(player);
        }

        private async Task DeletePlayer(Player player)
        {
            if (player == null || _selectedSeason == null) return;

            var result = await DisplayAlert("Confirmar", $"¿Eliminar a {player.Name ?? "este jugador"}?", "Sí", "No");
            if (result)
            {
                try
                {
                    _selectedSeason.Players.Remove(player);
                    await _dataService.SaveCareerAsync(_selectedCareer);
                    LoadPlayers();
                    await DisplayAlert("Éxito", "Jugador eliminado correctamente", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al eliminar jugador: {ex.Message}", "OK");
                }
            }
        }

        private async Task ShowPlayerForm(Player player)
        {
            if (player == null || _selectedSeason == null) return;

            try
            {
                var isNew = string.IsNullOrEmpty(player.Name);
                var title = isNew ? "Agregar Jugador" : "Editar Jugador";

                // Nombre del jugador
                var nameResult = await DisplayPromptAsync(title, "Nombre del jugador:",
                    initialValue: player.Name ?? "",
                    maxLength: 50);
                if (string.IsNullOrWhiteSpace(nameResult))
                {
                    if (isNew) await DisplayAlert("Cancelado", "Debe ingresar un nombre", "OK");
                    return;
                }

                // Posición
                var positionResult = await DisplayActionSheet("Seleccionar posición:", "Cancelar", null,
                    "POR", "DFC", "DFI", "LAD", "LAI", "MCD", "MC", "MCO", "MD", "MI", "DC", "EI", "ED");
                if (positionResult == "Cancelar" || string.IsNullOrEmpty(positionResult))
                {
                    positionResult = player.Position ?? "MC";
                }

                // Edad
                var ageResult = await DisplayPromptAsync(title, "Edad (16-45):",
                    initialValue: player.Age > 0 ? player.Age.ToString() : "25",
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(ageResult, out int age) || age < 16 || age > 45)
                {
                    await DisplayAlert("Error", "La edad debe ser un número entre 16 y 45", "OK");
                    return;
                }

                // Overall
                var overallResult = await DisplayPromptAsync(title, "Overall (40-99):",
                    initialValue: player.Overall > 0 ? player.Overall.ToString() : "70",
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(overallResult, out int overall) || overall < 40 || overall > 99)
                {
                    await DisplayAlert("Error", "El overall debe ser un número entre 40 y 99", "OK");
                    return;
                }

                // Potencial
                var potentialResult = await DisplayPromptAsync(title, "Potencial (40-99):",
                    initialValue: player.Potential > 0 ? player.Potential.ToString() : overall.ToString(),
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(potentialResult, out int potential) || potential < 40 || potential > 99)
                {
                    await DisplayAlert("Error", "El potencial debe ser un número entre 40 y 99", "OK");
                    return;
                }

                // Validar que el potencial sea mayor o igual al overall
                if (potential < overall)
                {
                    await DisplayAlert("Error", "El potencial no puede ser menor que el overall", "OK");
                    return;
                }

                // Goles
                var goalsResult = await DisplayPromptAsync(title, "Goles esta temporada:",
                    initialValue: player.Goals.ToString(),
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(goalsResult, out int goals) || goals < 0)
                {
                    goals = 0;
                }

                // Asistencias
                var assistsResult = await DisplayPromptAsync(title, "Asistencias esta temporada:",
                    initialValue: player.Assists.ToString(),
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(assistsResult, out int assists) || assists < 0)
                {
                    assists = 0;
                }

                // Partidos jugados
                var gamesResult = await DisplayPromptAsync(title, "Partidos jugados:",
                    initialValue: player.GamesPlayed.ToString(),
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(gamesResult, out int games) || games < 0)
                {
                    games = 0;
                }

                // Tarjetas amarillas
                var yellowResult = await DisplayPromptAsync(title, "Tarjetas amarillas:",
                    initialValue: player.YellowCards.ToString(),
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(yellowResult, out int yellowCards) || yellowCards < 0)
                {
                    yellowCards = 0;
                }

                // Tarjetas rojas
                var redResult = await DisplayPromptAsync(title, "Tarjetas rojas:",
                    initialValue: player.RedCards.ToString(),
                    keyboard: Keyboard.Numeric);
                if (!int.TryParse(redResult, out int redCards) || redCards < 0)
                {
                    redCards = 0;
                }

                // Nacionalidad (opcional)
                var nationalityResult = await DisplayPromptAsync(title, "Nacionalidad (opcional):",
                    initialValue: player.Nationality ?? "");

                // Actualizar datos del jugador
                player.Name = nameResult.Trim();
                player.Position = positionResult;
                player.Age = age;
                player.Overall = overall;
                player.Potential = potential;
                player.Goals = goals;
                player.Assists = assists;
                player.GamesPlayed = games;
                player.YellowCards = yellowCards;
                player.RedCards = redCards;
                player.Nationality = !string.IsNullOrWhiteSpace(nationalityResult) ? nationalityResult.Trim() : null;

                // Si es nuevo, agregarlo a la lista
                if (isNew)
                {
                    if (_selectedSeason.Players == null)
                        _selectedSeason.Players = new List<Player>();

                    _selectedSeason.Players.Add(player);
                }

                // Guardar cambios
                await _dataService.SaveCareerAsync(_selectedCareer);
                LoadPlayers();

                var message = isNew ? "Jugador agregado correctamente" : "Jugador actualizado correctamente";
                await DisplayAlert("Éxito", message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al procesar jugador: {ex.Message}", "OK");
            }
        }
    }
}