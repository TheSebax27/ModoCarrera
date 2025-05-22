using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;

namespace ModoCarreraFC25
{
    public partial class StatisticsPage : ContentPage
    {
        private readonly IDataService _dataService;
        private List<Career> _careers;
        private Career _selectedCareer;

        public StatisticsPage(IDataService dataService)
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
                    StatsContainer.IsVisible = false;
                    await DisplayAlert("Información", "No hay carreras disponibles. Crea una carrera primero.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar las carreras: {ex.Message}", "OK");
            }
        }

        private async void OnCareerSelected(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker.SelectedIndex >= 0 && picker.SelectedIndex < _careers.Count)
            {
                _selectedCareer = _careers[picker.SelectedIndex];
                await LoadStatistics();
            }
        }

        private async Task LoadStatistics()
        {
            if (_selectedCareer == null) return;

            try
            {
                var statistics = await _dataService.GetStatisticsAsync(_selectedCareer.Id);

                // Update General Statistics
                TotalTitlesLabel.Text = statistics.TotalTitles.ToString();
                TotalSeasonsLabel.Text = statistics.TotalSeasons.ToString();
                TotalGoalsLabel.Text = statistics.TotalGoals.ToString();
                WinPercentageLabel.Text = $"{statistics.WinPercentage:F1}%";

                // Update Top Players
                UpdateTopPlayers(statistics);

                // Update Recent Titles
                RecentTitlesCollectionView.ItemsSource = statistics.RecentTitles;

                // Update Most Expensive Transfers
                ExpensiveTransfersCollectionView.ItemsSource = statistics.MostExpensiveTransfers;

                StatsContainer.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar las estadísticas: {ex.Message}", "OK");
            }
        }

        private void UpdateTopPlayers(Statistics statistics)
        {
            // Top Scorer
            if (statistics.TopScorer != null)
            {
                TopScorerLabel.Text = $"{statistics.TopScorer.Name} ({statistics.TopScorer.Goals} goles)";
                TopScorerContainer.IsVisible = true;
            }
            else
            {
                TopScorerContainer.IsVisible = false;
            }

            // Top Assister
            if (statistics.TopAssister != null)
            {
                TopAssisterLabel.Text = $"{statistics.TopAssister.Name} ({statistics.TopAssister.Assists} asistencias)";
                TopAssisterContainer.IsVisible = true;
            }
            else
            {
                TopAssisterContainer.IsVisible = false;
            }

            // Most Valuable
            if (statistics.MostValuable != null)
            {
                MostValuableLabel.Text = $"{statistics.MostValuable.Name} ({statistics.MostValuable.MarketValue:C})";
                MostValuableContainer.IsVisible = true;
            }
            else
            {
                MostValuableContainer.IsVisible = false;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Refresh statistics when page appears
            if (_selectedCareer != null)
            {
                await LoadStatistics();
            }
        }
    }
}