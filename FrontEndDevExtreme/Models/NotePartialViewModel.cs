namespace FrontEndDevExtreme.Models
{
    public class NotePartialViewModel
    {
        public int WorkItemId { get; set; }
        public IEnumerable<NoteViewModel> Notes { get; set; }
    }

}
