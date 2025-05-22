using System.ComponentModel;

namespace ModoCarreraFC25.Models
{
    public class Transfer : INotifyPropertyChanged
    {
        private string _id;
        private string _playerName;
        private string _transferType;
        private string _fromClub;
        private string _toClub;
        private decimal _amount;
        private DateTime _date;
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

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged(nameof(PlayerName));
            }
        }

        public string TransferType
        {
            get => _transferType;
            set
            {
                _transferType = value;
                OnPropertyChanged(nameof(TransferType));
            }
        }

        public string FromClub
        {
            get => _fromClub;
            set
            {
                _fromClub = value;
                OnPropertyChanged(nameof(FromClub));
            }
        }

        public string ToClub
        {
            get => _toClub;
            set
            {
                _toClub = value;
                OnPropertyChanged(nameof(ToClub));
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
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

        public Transfer()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTime.Now;
            TransferType = "Fichaje";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}