using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using FrontEndDevExtreme.Models;
using FrontEndDevExtreme.Services;
using FrontEndDevExtreme.Repository;

public class NoteApiService : INoteApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public NoteApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
    }

    public async Task<List<NoteModel>> GetNotesByWorkItemIdAsync(int workItemId)
    {
        var response = await _httpClient.GetAsync($"/api/workitems/{workItemId}/notes");

        if (!response.IsSuccessStatusCode)
            return new List<NoteModel>();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseModel<List<NoteModel>>>(content);
        return result?.Data ?? new List<NoteModel>();
    }

    public async Task<bool> AddNoteAsync(int workItemId, string content)
    {
        var dateCreate = DateTime.UtcNow;  

        var noteData = new
        {
            WorkItemID = workItemId,
            Content = content,        
            DateCreate = dateCreate   
        };

        var json = JsonConvert.SerializeObject(noteData);  
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"/api/workitems/{workItemId}/notes", httpContent);

        return response.IsSuccessStatusCode;  
    }


    public async Task<bool> DeleteNoteAsync(int workItemId, int noteId)
    {
        var response = await _httpClient.DeleteAsync($"/api/workitems/{workItemId}/notes/{noteId}");
        return response.IsSuccessStatusCode;
    }
}
