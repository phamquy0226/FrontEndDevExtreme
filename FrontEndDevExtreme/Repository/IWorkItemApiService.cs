using FrontEndDevExtreme.Models;

namespace FrontEndDevExtreme.Repository
{
    public interface IWorkItemApiService
    {
        Task<List<WorkItemViewModel>> GetAllAsync();
        Task<bool> CreateAsync(WorkItemCreateModel model);
        Task<WorkItemDetailModel> GetWorkItemDetailAsync(int id);
        Task<ResponseModel<List<WorkItemViewModel>>> GetFilteredAsync(WorkItemFilterModel filter, int pageNumber, int pageSize);

        Task<bool> UpdateAsync(int id, WorkItemEditModel model);
        Task<bool> DeleteAsync(int id);
    }

}
