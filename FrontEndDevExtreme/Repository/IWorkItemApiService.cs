using FrontEndDevExtreme.Models;

namespace FrontEndDevExtreme.Repository
{
    public interface IWorkItemApiService
    {
        Task<List<WorkItemViewModel>> GetAllAsync();
        Task<bool> CreateAsync(WorkItemCreateModel model);
        Task<WorkItemDetailModel> GetWorkItemDetailAsync(int id);
        Task<List<WorkItemViewModel>> GetFilteredAsync(WorkItemFilterModel filter);
        Task<bool> UpdateAsync(int id, WorkItemEditModel model);
    }

}
