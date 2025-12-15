namespace TimeTrackingApp.Models.ViewModel
{
        public class LeaveRequestViewModel
        {
            public int Id { get; set; }
            public string LeaveType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int DaysCount { get; set; }
            public string? Reason { get; set; }
            public string Status { get; set; } 
            public string? ManagerComment { get; set; }
            public DateTime RequestDate { get; set; }

    }
}

