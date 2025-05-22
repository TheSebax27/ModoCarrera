using ModoCarreraFC25.Services;
using ModoCarreraFC25.Views;

namespace ModoCarreraFC25
{
    public partial class MainPage : ContentPage
    {
        private readonly IDataService _dataService;

        public MainPage()
        {
            InitializeComponent();
            _dataService = new JsonDataService();
        }

        private async void OnCareersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CareerPage(_dataService));
        }

        private async void OnSeasonsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SeasonsPage(_dataService));
        }

        private async void OnPlayersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlayersPage(_dataService));
        }

        private async void OnTransfersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TransfersPage(_dataService));
        }

        private async void OnTitlesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TitlesPage(_dataService));
        }

        private async void OnStatisticsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StatisticsPage(_dataService));
        }
    }
}