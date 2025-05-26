using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;

namespace ModoCarreraFC25.Views
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
                    TransfersContainer.Children.Clear();
                    TransfersContainer.Children.Add(EmptyLabel);
                    SeasonLayout.IsVisible = false;
                    FilterLayout.IsVisible = false;
                    AddTransferBtn.IsVisible = false;
                }
            }
        }

        private void OnSeasonSelected(object sender, EventArgs e)
        {
            if (SeasonPicker.SelectedIndex >= 0 && _selectedCareer?.Seasons != null &&
                _selectedCareer.Seasons.Count > SeasonPicker.SelectedIndex)
            {
                _selectedSeason = _selectedCareer.Seasons[SeasonPicker.SelectedIndex];

                // Inicializar la lista de transfers si es null
                if (_selectedSeason.Transfers == null)
                    _selectedSeason.Transfers = new List<Transfer>();

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

            if (_allTransfers == null || !_allTransfers.Any())
            {
                EmptyLabel.Text = "No hay fichajes registrados en esta temporada";
                TransfersContainer.Children.Add(EmptyLabel);
                return;
            }

            var filteredTransfers = _currentFilter switch
            {
                "Fichajes" => _allTransfers.Where(t => t.TransferType == "Fichaje").ToList(),
                "Ventas" => _allTransfers.Where(t => t.TransferType == "Venta").ToList(),
                "Préstamos" => _allTransfers.Where(t => t.TransferType == "Préstamo").ToList(),
                _ => _allTransfers.ToList()
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
                Text = transfer.PlayerName ?? "Sin nombre",
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
            if (_selectedSeason == null)
            {
                await DisplayAlert("Error", "Debe seleccionar una temporada primero", "OK");
                return;
            }

            await ShowTransferForm(new Transfer());
        }

        private async Task EditTransfer(Transfer transfer)
        {
            if (transfer == null) return;
            await ShowTransferForm(transfer);
        }

        private async Task DeleteTransfer(Transfer transfer)
        {
            if (transfer == null || _selectedSeason == null) return;

            var result = await DisplayAlert("Confirmar", $"¿Eliminar el fichaje de {transfer.PlayerName ?? "este jugador"}?", "Sí", "No");
            if (result)
            {
                try
                {
                    _selectedSeason.Transfers.Remove(transfer);
                    await _dataService.SaveCareerAsync(_selectedCareer);
                    _allTransfers = _selectedSeason.Transfers;
                    LoadTransfers();
                    await DisplayAlert("Éxito", "Fichaje eliminado correctamente", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al eliminar fichaje: {ex.Message}", "OK");
                }
            }
        }

        private async Task ShowTransferForm(Transfer transfer)
        {
            if (transfer == null || _selectedSeason == null) return;

            try
            {
                var isNew = string.IsNullOrEmpty(transfer.PlayerName);
                var title = isNew ? "Registrar Fichaje" : "Editar Fichaje";

                // Nombre del jugador
                var playerResult = await DisplayPromptAsync(title, "Nombre del jugador:",
                    initialValue: transfer.PlayerName ?? "",
                    maxLength: 50);
                if (string.IsNullOrWhiteSpace(playerResult))
                {
                    if (isNew) await DisplayAlert("Cancelado", "Debe ingresar el nombre del jugador", "OK");
                    return;
                }

                // Tipo de fichaje
                var typeResult = await DisplayActionSheet("Tipo de fichaje:", "Cancelar", null, "Fichaje", "Venta", "Préstamo");
                if (typeResult == "Cancelar" || string.IsNullOrEmpty(typeResult))
                {
                    if (isNew) return;
                    typeResult = transfer.TransferType ?? "Fichaje";
                }

                // Club de origen
                var fromResult = await DisplayPromptAsync(title, "Club de origen (opcional):",
                    initialValue: transfer.FromClub ?? "");

                // Club destino
                var toResult = await DisplayPromptAsync(title, "Club destino (opcional):",
                    initialValue: transfer.ToClub ?? "");

                // Cantidad
                var amountResult = await DisplayPromptAsync(title, "Cantidad en euros (0 si es gratis):",
                    initialValue: transfer.Amount.ToString("F0"),
                    keyboard: Keyboard.Numeric);

                decimal amount = 0;
                if (!string.IsNullOrWhiteSpace(amountResult))
                {
                    if (!decimal.TryParse(amountResult, out amount) || amount < 0)
                    {
                        await DisplayAlert("Error", "La cantidad debe ser un número positivo", "OK");
                        return;
                    }
                }

                // Notas opcionales
                var notesResult = await DisplayPromptAsync(title, "Notas adicionales (opcional):",
                    initialValue: transfer.Notes ?? "");

                // Actualizar datos del fichaje
                transfer.PlayerName = playerResult.Trim();
                transfer.TransferType = typeResult;
                transfer.FromClub = !string.IsNullOrWhiteSpace(fromResult) ? fromResult.Trim() : null;
                transfer.ToClub = !string.IsNullOrWhiteSpace(toResult) ? toResult.Trim() : null;
                transfer.Amount = amount;
                transfer.Notes = !string.IsNullOrWhiteSpace(notesResult) ? notesResult.Trim() : null;

                // Si es nuevo, establecer la fecha y agregarlo
                if (isNew)
                {
                    transfer.Date = DateTime.Now;

                    if (_selectedSeason.Transfers == null)
                        _selectedSeason.Transfers = new List<Transfer>();

                    _selectedSeason.Transfers.Add(transfer);
                }

                // Guardar cambios
                await _dataService.SaveCareerAsync(_selectedCareer);
                _allTransfers = _selectedSeason.Transfers;
                LoadTransfers();

                var message = isNew ? "Fichaje registrado correctamente" : "Fichaje actualizado correctamente";
                await DisplayAlert("Éxito", message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al procesar fichaje: {ex.Message}", "OK");
            }
        }
    }
}