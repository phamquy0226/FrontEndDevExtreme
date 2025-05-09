using FrontEndDevExtreme.Models;

namespace FrontEndDevExtreme.Repository
{
    public interface INoteApiService
    {
        Task<List<NoteModel>> GetNotesByWorkItemIdAsync(int workItemId);
        Task<bool> AddNoteAsync(int workItemId, string content);
        Task<bool> DeleteNoteAsync(int workItemId, int noteId);
    }


}
