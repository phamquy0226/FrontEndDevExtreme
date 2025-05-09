using FrontEndDevExtreme.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using FrontEndDevExtreme.Repository;

namespace FrontEndDevExtreme.Services
{
    public class DepartmentApiService : IDepartmentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DepartmentApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<List<DepartmentViewModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Department");
                if (!response.IsSuccessStatusCode)
                    return new List<DepartmentViewModel>();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ResponseModel<List<DepartmentViewModel>>>(content);

                return result != null && result.Success ? result.Data : new List<DepartmentViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gọi API phòng ban: " + ex.Message);
                return new List<DepartmentViewModel>();
            }
        }
        public class ResponseModel<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
            public int? ErrorCode { get; set; }
        }

    }
}
