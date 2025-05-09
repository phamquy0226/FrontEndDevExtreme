using FrontEndDevExtreme.Models;

namespace FrontEndDevExtreme.Repository
{
    public interface IUserApiService
    {
        Task<List<UserViewModel>> GetAllAsync();
    }
}
