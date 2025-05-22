using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;

namespace ModoCarreraFC25
{
    public partial class TransfersPage : ContentPage
    {
        private readonly IDataService _dataService;
        private List<Career> _careers;
        private Career _selectedCareer;
        private Season _selectedSeason;
        private List<Transfer> _allTransfers;
        private string _currentFilter = "Todos";

        public TransfersPage(IDataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            FilterPicker.SelectedIndex = 0;
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
                    TransfersContainer.Children.Clear();
                    TransfersContainer.Children.Add(EmptyLabel);
                }
            }
        }

        private void OnSeasonSelected(object sender, EventArgs e)
        {
            if (SeasonPicker.SelectedIndex >= 0)
            {
                _selectedSeason = _selectedCareer.Seasons[SeasonPicker.SelectedIndex];
                _allTransfers = _selectedSeason.Transfers;
                FilterLayout.IsVisible = true;
                AddTransferBtn.IsVisible = true;
                LoadTransfers();
            }
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            if (FilterPicker.SelectedIndex >= 0)
            {
                _currentFilter = FilterPicker.Items[FilterPicker.SelectedIndex];
                LoadTransfers();
            }
        }

        private void LoadTransfers()
        {
            TransfersContainer.Children.Clear();

            var filteredTransfers = _currentFilter switch
            {
                "Fichajes" => _allTransfers.Where(t => t.TransferType == "Fichaje").ToList(),
                "Ventas" => _allTransfers.Where(t => t.TransferType == "Venta").ToList(),
                "Préstamos" => _allTransfers.Where(t => t.TransferType == "Préstamo").ToList(),
                _ => _allTransfers
            };

            if (!filteredTransfers.Any())
            {
                EmptyLabel.Text = _currentFilter == "Todos" ?
                    "No hay fichajes registrados en esta temporada" :
                    $"No hay {_currentFilter.ToLower()} registrados en esta temporada";
                TransfersContainer.Children.Add(EmptyLabel);
                return;
            }

            foreach (var transfer in filteredTransfers.OrderByDescending(t => t.Date))
            {
                var transferFrame = CreateTransferCard(transfer);
                TransfersContainer.Children.Add(transferFrame);
            }
        }

        private Frame CreateTransferCard(Transfer transfer)
        {
            var frame = new Frame
            {
                BackgroundColor = Color.FromHex("#1A2332"),
                BorderColor = GetTransferColor(transfer.TransferType),
                CornerRadius = 10,
                Padding = 15,
                Margin = new Thickness(0, 5)
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            // Transfer Type Badge
            var typeLabel = new Label
            {
                Text = GetTransferIcon(transfer.TransferType) + " " + transfer.TransferType,
                TextColor = GetTransferColor(transfer.TransferType),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            };
            Grid.SetRow(typeLabel, 0);
            Grid.SetColumnSpan(typeLabel, 3);

            // Player Name
            var playerLabel = new Label
            {
                Text = transfer.PlayerName,
                TextColor = Colors.White,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold
            };
            Grid.SetRow(playerLabel, 1);
            Grid.SetColumnSpan(playerLabel, 3);

            // Transfer Details
            var detailsLabel = new Label
            {
                Text = $"De: {transfer.FromClub ?? "Academia"} → A: {transfer.ToClub ?? "Vendido"}",
                TextColor = Color.FromHex("#B0C4DE"),
                FontSize = 14
            };
            Grid.SetRow(detailsLabel, 2);
            Grid.SetColumnSpan(detailsLabel, 3);

            // Amount and Date
            var amountLabel = new Label
            {
                Text = $"💰 {transfer.Amount:C} | 📅 {transfer.Date:dd/MM/yyyy}",
                TextColor = Color.FromHex("#00D4FF"),
                FontSize = 14
            };
            Grid.SetRow(amountLabel, 3);
            Grid.SetColumn(amountLabel, 0);

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
            editButton.Clicked += async (s, e) => await EditTransfer(transfer);
            Grid.SetRow(editButton, 3);
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
            deleteButton.Clicked += async (s, e) => await DeleteTransfer(transfer);
            Grid.SetRow(deleteButton, 3);
            Grid.SetColumn(deleteButton, 2);

            grid.Children.Add(typeLabel);
            grid.Children.Add(playerLabel);
            grid.Children.Add(detailsLabel);
            grid.Children.Add(amountLabel);
            grid.Children.Add(editButton);
            grid.Children.Add(deleteButton);

            frame.Content = grid;
            return frame;
        }

        private Color GetTransferColor(string transferType)
        {
            return transferType switch
            {
                "Fichaje" => Color.FromHex("#00FF7F"),
                "Venta" => Color.FromHex("#FF6B6B"),
                "Préstamo" => Color.FromHex("#FFD700"),
                _ => Color.FromHex("#00D4FF")
            };
        }

        private string GetTransferIcon(string transferType)
        {
            return transferType switch
            {
                "Fichaje" => "📥",
                "Venta" => "📤",
                "Préstamo" => "🔄",
                _ => "💼"
            };
        }

        private async void OnAddTransferClicked(object sender, EventArgs e)
        {
            await ShowTransferForm(new Transfer());
        }

        private async Task EditTransfer(Transfer transfer)
        {
            await ShowTransferForm(transfer);
        }

        private async Task DeleteTransfer(Transfer transfer)
        {
            var result = await DisplayAlert("Confirmar", $"¿Eliminar el fichaje de {transfer.PlayerName}?", "Sí", "No");
            if (result)
            {
                _selectedSeason.Transfers.Remove(transfer);
                await _dataService.SaveCareerAsync(_selectedCareer);
                _allTransfers = _selectedSeason.Transfers;
                LoadTransfers();
            }
        }

        private async Task ShowTransferForm(Transfer transfer)
        {
            var isNew = string.IsNullOrEmpty(transfer.PlayerName);
            var title = isNew ? "Registrar Fichaje" : "Editar Fichaje";

            // Player Name
            var playerResult = await DisplayPromptAsync(title, "Nombre del jugador:", initialValue: transfer.PlayerName ?? "");
            if (string.IsNullOrWhiteSpace(playerResult)) return;

            // Transfer Type
            var typeResult = await DisplayActionSheet("Tipo de fichaje:", "Cancelar", null, "Fichaje", "Venta", "Préstamo");
            if (typeResult == "Cancelar") return;

            // From Club
            var fromResult = await DisplayPromptAsync(title, "Club de origen:", initialValue: transfer.FromClub ?? "");

            // To Club
            var toResult = await DisplayPromptAsync(title, "Club destino:", initialValue: transfer.ToClub ?? "");

            // Amount
            var amountResult = await DisplayPromptAsync(title, "Cantidad (€):", initialValue: transfer.Amount.ToString(), keyboard: Keyboard.Numeric);
            if (!decimal.TryParse(amountResult, out decimal amount)) amount = 0;

            // Notes
            var notesResult = await DisplayPromptAsync(title, "Notas (opcional):", initialValue: transfer.Notes ?? "");

            // Update transfer data
            transfer.PlayerName = playerResult;
            transfer.TransferType = typeResult;
            transfer.FromClub = fromResult;
            transfer.ToClub = toResult;
            transfer.Amount = amount;
            transfer.Notes = notesResult;

            if (isNew)
            {
                transfer.Date = DateTime.Now;
                _selectedSeason.Transfers.Add(transfer);
            }

            await _dataService.SaveCareerAsync(_selectedCareer);
            _allTransfers = _selectedSeason.Transfers;
            LoadTransfers();
        }
    }
}