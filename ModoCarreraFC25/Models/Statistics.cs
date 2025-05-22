namespace ModoCarreraFC25.Models
{
    public class Statistics
    {
        public Player TopScorer { get; set; }
        public Player TopAssister { get; set; }
        public Player MostValuable { get; set; }
        public int TotalTitles { get; set; }
        public int TotalSeasons { get; set; }
        public int TotalGoals { get; set; }
        public int TotalGames { get; set; }
        public double WinPercentage { get; set; }
        public List<Title> RecentTitles { get; set; }
        public List<Transfer> MostExpensiveTransfers { get; set; }

        public Statistics()
        {
            RecentTitles = new List<Title>();
            MostExpensiveTransfers = new List<Transfer>();
        }
    }
}