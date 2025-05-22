using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;

namespace ModoCarreraFC25
{
    public partial class TitlesPage : ContentPage
    {
        private readonly IDataService _dataService;
        private List<Career> _careers;
        private Career _selectedCareer;

        public TitlesPage(IDataService dataService)
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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar las carreras: {ex.Message}", "OK");
            }
        }

        private void OnCareerSelected(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker.SelectedIndex >= 0 && picker.SelectedIndex < _careers.Count)
            {
                _selectedCareer = _careers[picker.SelectedIndex];
                LoadTitles();
            }
        }

        private void LoadTitles()
        {
            if (_selectedCareer != null)
            {
                TitlesCollectionView.ItemsSource = _selectedCareer.Titles.OrderByDescending(t => t.Year).ToList();
            }
        }

        private async void OnAddTitleClicked(object sender, EventArgs e)
        {
            if (_selectedCareer == null)
            {
                await DisplayAlert("Aviso", "Selecciona una carrera primero", "OK");
                return;
            }

            var result = await ShowTitleDialog(new Title());
            if (result != null)
            {
                _selectedCareer.Titles.Add(result);
                await _dataService.SaveCareerAsync(_selectedCareer);
                LoadTitles();
                await DisplayAlert("Éxito", "Título agregado correctamente", "OK");
            }
        }

        private async void OnEditTitleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Title title)
            {
                var result = await ShowTitleDialog(title);
                if (result != null)
                {
                    var index = _selectedCareer.Titles.FindIndex(t => t.Id == title.Id);
                    if (index >= 0)
                    {
                        _selectedCareer.Titles[index] = result;
                        await _dataService.SaveCareerAsync(_selectedCareer);
                        LoadTitles();
                        await DisplayAlert("Éxito", "Título actualizado correctamente", "OK");
                    }
                }
            }
        }

        private async void OnDeleteTitleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Title title)
            {
                var confirm = await DisplayAlert("Confirmar",
                    $"¿Estás seguro de eliminar el título '{title.Name}'?",
                    "Sí", "No");

                if (confirm)
                {
                    _selectedCareer.Titles.Remove(title);
                    await _dataService.SaveCareerAsync(_selectedCareer);
                    LoadTitles();
                    await DisplayAlert("Éxito", "Título eliminado correctamente", "OK");
                }
            }
        }

        private async Task<Title> ShowTitleDialog(Title title)
        {
            var isEditing = !string.IsNullOrEmpty(title.Name);
            var dialogTitle = isEditing ? "Editar Título" : "Nuevo Título";

            // Title Name
            var name = await DisplayPromptAsync(dialogTitle, "Nombre del título:",
                initialValue: title.Name ?? "", maxLength: 50);
            if (string.IsNullOrWhiteSpace(name)) return null;

            // Title Type
            var titleTypes = new[] { "Liga", "Copa Nacional", "Copa Internacional", "Supercopa", "Otros" };
            var typeAction = await DisplayActionSheet("Tipo de título", "Cancelar", null, titleTypes);
            if (typeAction == "Cancelar" || string.IsNullOrEmpty(typeAction)) return null;

            // Year
            var yearStr = await DisplayPromptAsync(dialogTitle, "Año:",
                initialValue: title.Year.ToString(), keyboard: Keyboard.Numeric);
            if (!int.TryParse(yearStr, out int year)) return null;

            // Club
            var club = await DisplayPromptAsync(dialogTitle, "Club:",
                initialValue: title.Club ?? "", maxLength: 30);
            if (string.IsNullOrWhiteSpace(club)) return null;

            // Notes
            var notes = await DisplayPromptAsync(dialogTitle, "Notas (opcional):",
                initialValue: title.Notes ?? "", maxLength: 200);

            var newTitle = new Title
            {
                Id = title.Id ?? Guid.NewGuid().ToString(),
                Name = name,
                Type = typeAction,
                Year = year,
                Club = club,
                Notes = notes ?? ""
            };

            return newTitle;
        }
    }
}