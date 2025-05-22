using System.ComponentModel;

namespace ModoCarreraFC25.Models
{
    public class Title : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _type;
        private int _year;
        private string _club;
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

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
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

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public Title()
        {
            Id = Guid.NewGuid().ToString();
            Year = DateTime.Now.Year;
            Type = "Liga";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}