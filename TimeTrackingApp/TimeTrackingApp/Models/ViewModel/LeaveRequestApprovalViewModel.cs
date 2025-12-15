namespace TimeTrackingApp.Models.ViewModel
{
    public class LeaveRequestApprovalViewModel
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = "";
        public string LeaveType { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysCount { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "";
        public string? ManagerComment { get; set; }
        public DateTime RequestDate { get; set; }
    }

    public class LeaveRequestDecisionViewModel
    {
        public int Id { get; set; }
        public string Decision { get; set; } = ""; 
        public string? Comment { get; set; }
    }
}
