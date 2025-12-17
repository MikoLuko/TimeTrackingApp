namespace TimeTrackingApp.Models.ViewModel
{
public class LeaveRequestReportViewModel
{
    public string EmployeeName { get; set; } = "";
    public string LeaveType { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysCount { get; set; }
    public string Status { get; set; } = "";
}
}

