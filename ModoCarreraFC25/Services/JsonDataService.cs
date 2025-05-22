using System.Text.Json;
using ModoCarreraFC25.Models;

namespace ModoCarreraFC25.Services
{
    public class JsonDataService : IDataService
    {
        private List<Career> _careers;
        private readonly string _dataPath;

        public JsonDataService()
        {
            _careers = new List<Career>();
            _dataPath = Path.Combine(FileSystem.AppDataDirectory, "careers.json");
        }

        public async Task<List<Career>> GetCareersAsync()
        {
            if (_careers == null || !_careers.Any())
            {
                await LoadDataAsync();
            }
            return _careers ?? new List<Career>();
        }

        public async Task SaveCareerAsync(Career career)
        {
            var existingCareer = _careers.FirstOrDefault(c => c.Id == career.Id);
            if (existingCareer != null)
            {
                var index = _careers.IndexOf(existingCareer);
                _careers[index] = career;
            }
            else
            {
                _careers.Add(career);
            }
            await SaveDataAsync();
        }

        public async Task DeleteCareerAsync(string careerId)
        {
            var career = _careers.FirstOrDefault(c => c.Id == careerId);
            if (career != null)
            {
                _careers.Remove(career);
                await SaveDataAsync();
            }
        }

        public async Task<Career> GetCareerByIdAsync(string careerId)
        {
            await LoadDataAsync();
            return _careers.FirstOrDefault(c => c.Id == careerId);
        }

        public async Task<Statistics> GetStatisticsAsync(string careerId)
        {
            var career = await GetCareerByIdAsync(careerId);
            if (career == null) return new Statistics();

            var allPlayers = career.Seasons.SelectMany(s => s.Players).ToList();
            var allTransfers = career.Seasons.SelectMany(s => s.Transfers).ToList();

            var stats = new Statistics
            {
                TopScorer = allPlayers.OrderByDescending(p => p.Goals).FirstOrDefault(),
                TopAssister = allPlayers.OrderByDescending(p => p.Assists).FirstOrDefault(),
                MostValuable = allPlayers.OrderByDescending(p => p.MarketValue).FirstOrDefault(),
                TotalTitles = career.Titles.Count,
                TotalSeasons = career.Seasons.Count,
                TotalGoals = career.Seasons.Sum(s => s.GoalsFor),
                TotalGames = career.Seasons.Sum(s => s.GamesPlayed),
                RecentTitles = career.Titles.OrderByDescending(t => t.Year).Take(5).ToList(),
                MostExpensiveTransfers = allTransfers.OrderByDescending(t => t.Amount).Take(5).ToList()
            };

            var totalWins = career.Seasons.Sum(s => s.Wins);
            var totalGames = career.Seasons.Sum(s => s.GamesPlayed);
            stats.WinPercentage = totalGames > 0 ? (double)totalWins / totalGames * 100 : 0;

            return stats;
        }

        public async Task SaveDataAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_careers, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_dataPath, json);
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                if (File.Exists(_dataPath))
                {
                    var json = await File.ReadAllTextAsync(_dataPath);
                    _careers = JsonSerializer.Deserialize<List<Career>>(json) ?? new List<Career>();
                }
                else
                {
                    _careers = new List<Career>();
                }
            }
            catch (Exception ex)
            {
                // Log error and initialize empty list
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                _careers = new List<Career>();
            }
        }
    }
}