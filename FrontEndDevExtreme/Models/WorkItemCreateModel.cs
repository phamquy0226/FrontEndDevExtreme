namespace FrontEndDevExtreme.Models
{
    public class WorkItemCreateModel
    {
        public string TaskName { get; set; }
        public int Status { get; set; }
        public int Progress { get; set; }
        public string TaskType { get; set; }
        public bool IsPinned { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int AssignerID { get; set; }

        public int SelectedDepartmentID { get; set; } 
        public List<int> DepartmentIDs { get; set; } = new();

        public int SelectedUserID { get; set; } 
        public List<int> UserIDs { get; set; } = new();
        public int Priority { get; set; }
       
    }

}
