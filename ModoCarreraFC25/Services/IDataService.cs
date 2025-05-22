using ModoCarreraFC25.Models;

namespace ModoCarreraFC25.Services
{
    public interface IDataService
    {
        Task<List<Career>> GetCareersAsync();
        Task SaveCareerAsync(Career career);
        Task DeleteCareerAsync(string careerId);
        Task<Career> GetCareerByIdAsync(string careerId);
        Task<Statistics> GetStatisticsAsync(string careerId);
        Task SaveDataAsync();
        Task LoadDataAsync();
    }
}