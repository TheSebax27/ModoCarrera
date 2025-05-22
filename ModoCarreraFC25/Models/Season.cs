using System.ComponentModel;

namespace ModoCarreraFC25.Models
{
    public class Season : INotifyPropertyChanged
    {
        private string _id;
        private string _careerId;
        private int _year;
        private string _club;
        private int _leaguePosition;
        private int _gamesPlayed;
        private int _wins;
        private int _draws;
        private int _losses;
        private int _goalsFor;
        private int _goalsAgainst;
        private List<Player> _players;
        private List<Transfer> _transfers;
        private string _notes;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string CareerId
        {
            get => _careerId;
            set
            {
                _careerId = value;
                OnPropertyChanged(nameof(CareerId));
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        public string Club
        {
            get => _club;
            set
            {
                _club = value;
                OnPropertyChanged(nameof(Club));
            }
        }

        public int LeaguePosition
        {
            get => _leaguePosition;
            set
            {
                _leaguePosition = value;
                OnPropertyChanged(nameof(LeaguePosition));
            }
        }

        public int GamesPlayed
        {
            get => _gamesPlayed;
            set
            {
                _gamesPlayed = value;
                OnPropertyChanged(nameof(GamesPlayed));
            }
        }

        public int Wins
        {
            get => _wins;
            set
            {
                _wins = value;
                OnPropertyChanged(nameof(Wins));
            }
        }

        public int Draws
        {
            get => _draws;
            set
            {
                _draws = value;
                OnPropertyChanged(nameof(Draws));
            }
        }

        public int Losses
        {
            get => _losses;
            set
            {
                _losses = value;
                OnPropertyChanged(nameof(Losses));
            }
        }

        public int GoalsFor
        {
            get => _goalsFor;
            set
            {
                _goalsFor = value;
                OnPropertyChanged(nameof(GoalsFor));
            }
        }

        public int GoalsAgainst
        {
            get => _goalsAgainst;
            set
            {
                _goalsAgainst = value;
                OnPropertyChanged(nameof(GoalsAgainst));
            }
        }

        public List<Player> Players
        {
            get => _players ??= new List<Player>();
            set
            {
                _players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        public List<Transfer> Transfers
        {
            get => _transfers ??= new List<Transfer>();
            set
            {
                _transfers = value;
                OnPropertyChanged(nameof(Transfers));
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int Points => (Wins * 3) + Draws;

        public Season()
        {
            Id = Guid.NewGuid().ToString();
            Players = new List<Player>();
            Transfers = new List<Transfer>();
            Year = DateTime.Now.Year;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}