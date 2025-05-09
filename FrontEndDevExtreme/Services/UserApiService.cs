using System.Net.Http;
using System.Text;
using FrontEndDevExtreme.Models;
using FrontEndDevExtreme.Repository;
using Newtonsoft.Json;

namespace FrontEndDevExtreme.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<List<UserViewModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/User");
                if (!response.IsSuccessStatusCode)
                    return new List<UserViewModel>();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseModel<List<UserViewModel>>>(content);

                return result.Success ? result.Data : new List<UserViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gọi API lấy người dùng: " + ex.Message);
                return new List<UserViewModel>();
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
