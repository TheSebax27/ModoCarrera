using ModoCarreraFC25.Models;
using ModoCarreraFC25.Services;

namespace ModoCarreraFC25.Views
{
    public partial class CareerPage : ContentPage
    {
        private readonly IDataService _dataService;
        private List<Career> _careers;
        private Frame _careerFormModal;
        private Career _editingCareer;

        public CareerPage(IDataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            CreateCareerFormModal();
            LoadCareers();
        }

        private void CreateCareerFormModal()
        {
            _careerFormModal = new Frame
            {
                BackgroundColor = Color.FromArgb("#1A2332"),
                BorderColor = Color.FromArgb("#00D4FF"),
                CornerRadius = 15,
                Padding = 20,
                IsVisible = false,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = 20
            };

            var modalContent = new StackLayout { Spacing = 15 };

            modalContent.Children.Add(new Label
            {
                Text = "Nueva Carrera",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#00D4FF"),
                HorizontalOptions = LayoutOptions.Center
            });

            modalContent.Children.Add(ManagerNameEntry);
            modalContent.Children.Add(InitialClubEntry);
            modalContent.Children.Add(ModePicker);
            modalContent.Children.Add(NotesEditor);

            var buttonGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var cancelButton = new Button
            {
                Text = "Cancelar",
                BackgroundColor = Color.FromArgb("#DC143C"),
                TextColor = Colors.White,
                CornerRadius = 10
            };
            cancelButton.Clicked += OnCancelCareerClicked;

            var saveButton = new Button
            {
                Text = "Guardar",
                BackgroundColor = Color.FromArgb("#00D4FF"),
                TextColor = Colors.White,
                CornerRadius = 10
            };
            saveButton.Clicked += OnSaveCareerClicked;

            buttonGrid.Children.Add(cancelButton);
            Grid.SetColumn(cancelButton, 0);

            buttonGrid.Children.Add(saveButton);
            Grid.SetColumn(saveButton, 1);

            modalContent.Children.Add(buttonGrid);
            _careerFormModal.Content = modalContent;

            // Add modal to page
            var mainGrid = (Grid)Content;
            mainGrid.Children.Add(_careerFormModal);
            Grid.SetRowSpan(_careerFormModal, 3);
        }

        private async void LoadCareers()
        {
            try
            {
                _careers = await _dataService.GetCareersAsync();
                DisplayCareers();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error cargando carreras: {ex.Message}", "OK");
            }
        }

        private void DisplayCareers()
        {
            CareersContainer.Children.Clear();

            if (_careers == null || !_careers.Any())
            {
                var emptyLabel = new Label
                {
                    Text = "No hay carreras creadas.\n¡Crea tu primera carrera!",
                    FontSize = 16,
                    TextColor = Color.FromArgb("#B0C4DE"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 50)
                };
                CareersContainer.Children.Add(emptyLabel);
                return;
            }

            foreach (var career in _careers)
            {
                var careerFrame = CreateCareerFrame(career);
                CareersContainer.Children.Add(careerFrame);
            }
        }

        private Frame CreateCareerFrame(Career career)
        {
            var frame = new Frame
            {
                BackgroundColor = Color.FromArgb("#1A2332"),
                BorderColor = Color.FromArgb("#00D4FF"),
                CornerRadius = 10,
                Padding = 15,
                HasShadow = true
            };

            var stackLayout = new StackLayout { Spacing = 8 };

            // Career name and mode
            var headerGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var nameLabel = new Label
            {
                Text = career.ManagerName,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#00D4FF")
            };

            var modeLabel = new Label
            {
                Text = career.Mode,
                FontSize = 12,
                TextColor = Color.FromArgb("#B0C4DE"),
                HorizontalOptions = LayoutOptions.End
            };

            headerGrid.Children.Add(nameLabel);
            Grid.SetColumn(nameLabel, 0);

            headerGrid.Children.Add(modeLabel);
            Grid.SetColumn(modeLabel, 1);

            stackLayout.Children.Add(headerGrid);

            // Club and date info
            var infoGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            var clubLabel = new Label
            {
                Text = $"??? {career.InitialClub}",
                FontSize = 14,
                TextColor = Colors.White
            };

            var dateLabel = new Label
            {
                Text = $"?? {career.StartDate:dd/MM/yyyy}",
                FontSize = 14,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.End
            };

            infoGrid.Children.Add(clubLabel);
            Grid.SetColumn(clubLabel, 0);

            infoGrid.Children.Add(dateLabel);
            Grid.SetColumn(dateLabel, 1);

            stackLayout.Children.Add(infoGrid);

            // Stats
            var statsGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            var seasonsLabel = new Label
            {
                Text = $"Temporadas: {career.Seasons.Count}",
                FontSize = 12,
                TextColor = Color.FromArgb("#B0C4DE")
            };

            var titlesLabel = new Label
            {
                Text = $"Títulos: {career.Titles.Count}",
                FontSize = 12,
                TextColor = Color.FromArgb("#B0C4DE"),
                HorizontalOptions = LayoutOptions.Center
            };

            var editButton = new Button
            {
                Text = "??",
                FontSize = 16,
                BackgroundColor = Colors.Transparent,
                TextColor = Color.FromArgb("#00D4FF"),
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = 40,
                HeightRequest = 30
            };
            editButton.Clicked += (s, e) => OnEditCareerClicked(career);

            statsGrid.Children.Add(seasonsLabel);
            Grid.SetColumn(seasonsLabel, 0);

            statsGrid.Children.Add(titlesLabel);
            Grid.SetColumn(titlesLabel, 1);

            statsGrid.Children.Add(editButton);
            Grid.SetColumn(editButton, 2);

            stackLayout.Children.Add(statsGrid);

            frame.Content = stackLayout;

            // Add tap gesture to navigate to career details
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnCareerTapped(career);
            frame.GestureRecognizers.Add(tapGesture);

            return frame;
        }

        private async void OnCareerTapped(Career career)
        {
           
            await Navigation.PushAsync(new SeasonsPage(_dataService, career.Id));
        }

        private void OnEditCareerClicked(Career career)
        {
            _editingCareer = career;
            ManagerNameEntry.Text = career.ManagerName;
            InitialClubEntry.Text = career.InitialClub;
            ModePicker.SelectedItem = career.Mode;
            NotesEditor.Text = career.Notes;

            _careerFormModal.IsVisible = true;
        }

        private void OnAddCareerClicked(object sender, EventArgs e)
        {
            _editingCareer = null;
            ManagerNameEntry.Text = "";
            InitialClubEntry.Text = "";
            ModePicker.SelectedIndex = 0;
            NotesEditor.Text = "";

            _careerFormModal.IsVisible = true;
        }

        private void OnCancelCareerClicked(object sender, EventArgs e)
        {
            _careerFormModal.IsVisible = false;
        }

        private async void OnSaveCareerClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerNameEntry.Text) ||
                string.IsNullOrWhiteSpace(InitialClubEntry.Text))
            {
                await DisplayAlert("Error", "Por favor completa todos los campos requeridos", "OK");
                return;
            }

            try
            {
                var career = _editingCareer ?? new Career();

                career.ManagerName = ManagerNameEntry.Text.Trim();
                career.InitialClub = InitialClubEntry.Text.Trim();
                career.Mode = ModePicker.SelectedItem?.ToString() ?? "Manager";
                career.Notes = NotesEditor.Text?.Trim();

                await _dataService.SaveCareerAsync(career);

                _careerFormModal.IsVisible = false;
                LoadCareers();

                await DisplayAlert("Éxito", "Carrera guardada correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error guardando carrera: {ex.Message}", "OK");
            }
        }
    }
}