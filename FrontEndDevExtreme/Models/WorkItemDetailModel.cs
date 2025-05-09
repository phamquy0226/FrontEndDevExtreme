namespace FrontEndDevExtreme.Models
{
    public class WorkItemDetailModel
    {
        public int WorkItemID { get; set; }
        public string TaskName { get; set; }
        public int Status { get; set; }
        public int Progress { get; set; }
        public string TaskType { get; set; }
        public bool IsPinned { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AssignerID { get; set; }
        public string AssignerName { get; set; }
        public int Priority { get; set; }
        public string DepartmentList { get; set; }
        public string UserList { get; set; }
        public List<NoteModel> Notes { get; set; } = new List<NoteModel>();

    }

}
