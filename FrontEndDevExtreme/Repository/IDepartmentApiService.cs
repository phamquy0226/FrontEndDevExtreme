using FrontEndDevExtreme.Models;

namespace FrontEndDevExtreme.Repository
{
    public interface IDepartmentApiService
    {
        Task<List<DepartmentViewModel>> GetAllAsync();
    }
}
