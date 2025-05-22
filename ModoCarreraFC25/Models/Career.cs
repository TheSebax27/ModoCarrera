using System.ComponentModel;

namespace ModoCarreraFC25.Models
{
    public class Career : INotifyPropertyChanged
    {
        private string _id;
        private string _managerName;
        private string _initialClub;
        private DateTime _startDate;
        private string _mode;
        private List<Season> _seasons;
        private List<Title> _titles;
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

        public string ManagerName
        {
            get => _managerName;
            set
            {
                _managerName = value;
                OnPropertyChanged(nameof(ManagerName));
            }
        }

        public string InitialClub
        {
            get => _initialClub;
            set
            {
                _initialClub = value;
                OnPropertyChanged(nameof(InitialClub));
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public string Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        public List<Season> Seasons
        {
            get => _seasons ??= new List<Season>();
            set
            {
                _seasons = value;
                OnPropertyChanged(nameof(Seasons));
            }
        }

        public List<Title> Titles
        {
            get => _titles ??= new List<Title>();
            set
            {
                _titles = value;
                OnPropertyChanged(nameof(Titles));
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

        public Career()
        {
            Id = Guid.NewGuid().ToString();
            StartDate = DateTime.Now;
            Seasons = new List<Season>();
            Titles = new List<Title>();
            Mode = "Manager";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}