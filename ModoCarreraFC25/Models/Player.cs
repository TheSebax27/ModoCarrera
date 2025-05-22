using System.ComponentModel;

namespace ModoCarreraFC25.Models
{
    public class Player : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _position;
        private int _age;
        private int _overall;
        private int _potential;
        private int _goals;
        private int _assists;
        private int _yellowCards;
        private int _redCards;
        private int _gamesPlayed;
        private decimal _marketValue;
        private string _nationality;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged(nameof(Age));
            }
        }

        public int Overall
        {
            get => _overall;
            set
            {
                _overall = value;
                OnPropertyChanged(nameof(Overall));
            }
        }

        public int Potential
        {
            get => _potential;
            set
            {
                _potential = value;
                OnPropertyChanged(nameof(Potential));
            }
        }

        public int Goals
        {
            get => _goals;
            set
            {
                _goals = value;
                OnPropertyChanged(nameof(Goals));
            }
        }

        public int Assists
        {
            get => _assists;
            set
            {
                _assists = value;
                OnPropertyChanged(nameof(Assists));
            }
        }

        public int YellowCards
        {
            get => _yellowCards;
            set
            {
                _yellowCards = value;
                OnPropertyChanged(nameof(YellowCards));
            }
        }

        public int RedCards
        {
            get => _redCards;
            set
            {
                _redCards = value;
                OnPropertyChanged(nameof(RedCards));
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

        public decimal MarketValue
        {
            get => _marketValue;
            set
            {
                _marketValue = value;
                OnPropertyChanged(nameof(MarketValue));
            }
        }

        public string Nationality
        {
            get => _nationality;
            set
            {
                _nationality = value;
                OnPropertyChanged(nameof(Nationality));
            }
        }

        public double GoalsPerGame => GamesPlayed > 0 ? (double)Goals / GamesPlayed : 0;
        public double AssistsPerGame => GamesPlayed > 0 ? (double)Assists / GamesPlayed : 0;

        public Player()
        {
            Id = Guid.NewGuid().ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}