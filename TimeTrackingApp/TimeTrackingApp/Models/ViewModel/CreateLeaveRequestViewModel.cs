namespace TimeTrackingApp.Models.ViewModel
{
    public class LeaveRequestCreateViewModel
    {
        public string LeaveType { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        public string? Reason { get; set; }
        public DateTime RequestDate { get; set; }
    }
}

