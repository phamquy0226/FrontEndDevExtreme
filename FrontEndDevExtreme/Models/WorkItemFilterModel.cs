namespace FrontEndDevExtreme.Models
{
    public class WorkItemFilterModel
    {
        public string? UserName { get; set; }
        public string? AssignerName { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public int? Status { get; set; }
        public int? DepartmentID { get; set; }
        public string? SearchTaskName { get; set; }
        public int? Priority { get; set; }
        public bool? IsPinned { get; set; }
        public List<NoteModel> Notes { get; set; } = new List<NoteModel>();
    }

}
