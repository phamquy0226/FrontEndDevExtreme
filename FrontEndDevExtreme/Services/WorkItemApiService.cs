using System.Text;
using FrontEndDevExtreme.Models;
using FrontEndDevExtreme.Repository;
using Newtonsoft.Json;

namespace FrontEndDevExtreme.Services
{
    public class WorkItemApiService : IWorkItemApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WorkItemApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<List<WorkItemViewModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/WorkItem");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API call failed with status code: " + response.StatusCode);
                    return new List<WorkItemViewModel>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseModel<List<WorkItemViewModel>>>(content);

                if (result != null && result.Success)
                    return result.Data;

                Console.WriteLine("Error: " + (result?.Message ?? "Unknown error"));
                return new List<WorkItemViewModel>();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("Error calling GetAllAsync: " + ex.Message);
                return new List<WorkItemViewModel>();
            }
        }

        public async Task<ResponseModel<List<WorkItemViewModel>>> GetFilteredAsync(WorkItemFilterModel filter, int pageNumber, int pageSize)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(filter.UserName)) queryParams.Add($"userName={Uri.EscapeDataString(filter.UserName)}");
            if (!string.IsNullOrEmpty(filter.AssignerName)) queryParams.Add($"assignerName={Uri.EscapeDataString(filter.AssignerName)}");
            if (filter.StartDateFrom.HasValue) queryParams.Add($"startDateFrom={filter.StartDateFrom:yyyy-MM-dd}");
            if (filter.StartDateTo.HasValue) queryParams.Add($"startDateTo={filter.StartDateTo:yyyy-MM-dd}");
            if (filter.EndDateFrom.HasValue) queryParams.Add($"endDateFrom={filter.EndDateFrom:yyyy-MM-dd}");
            if (filter.EndDateTo.HasValue) queryParams.Add($"endDateTo={filter.EndDateTo:yyyy-MM-dd}");
            if (filter.Status.HasValue) queryParams.Add($"status={filter.Status}");
            if (filter.DepartmentID.HasValue) queryParams.Add($"departmentID={filter.DepartmentID}");
            if (!string.IsNullOrEmpty(filter.SearchTaskName)) queryParams.Add($"searchTaskName={Uri.EscapeDataString(filter.SearchTaskName)}");
            if (filter.Priority.HasValue) queryParams.Add($"priority={filter.Priority}");
            if (filter.IsPinned.HasValue) queryParams.Add($"isPinned={filter.IsPinned}");

            queryParams.Add($"pageNumber={pageNumber}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/api/WorkItem?{queryString}");

            if (!response.IsSuccessStatusCode)
                return new ResponseModel<List<WorkItemViewModel>>()
                {
                    Success = false,
                    Data = new List<WorkItemViewModel>(),
                    Message = "Failed to get data"
                };

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel<List<WorkItemViewModel>>>(content);
            return result ?? new ResponseModel<List<WorkItemViewModel>>()
            {
                Success = false,
                Data = new List<WorkItemViewModel>(),
                Message = "Empty response"
            };
        }





        public async Task<bool> CreateAsync(WorkItemCreateModel model)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/WorkItem", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to create work item, status code: " + response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Error calling CreateAsync: " + ex.Message);
                return false;
            }
        }

        public async Task<WorkItemDetailModel> GetWorkItemDetailAsync(int id)
        {
            try
            {
               
                var response = await _httpClient.GetAsync($"/api/WorkItem/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseModel<WorkItemDetailModel>>(content);

                    
                    if (result != null && result.Success)
                    {
                        return result.Data;
                    }
                    else
                    {
                        Console.WriteLine("Không thể lấy chi tiết cho ID " + id + ": " + (result?.Message ?? "Lỗi không xác định"));
                    }
                }
                else
                {
                    Console.WriteLine("Lỗi khi gọi API, mã trạng thái: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
              
                Console.WriteLine("Lỗi khi gọi GetWorkItemDetailAsync: " + ex.Message);
            }

            return null; 
        }

        public async Task<bool> UpdateAsync(int id, WorkItemEditModel model)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/WorkItem/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to update work item {id}, status code: {response.StatusCode}");
                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseModel<object>>(responseContent);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling UpdateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/WorkItem/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed to delete work item, status code: " + response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error calling DeleteAsync: " + ex.Message);
                return false;
            }
        }


    }
}
