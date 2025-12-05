namespace TimeTrackingApp.Models.Entities
{
    public class LeaveRequest
    {
        public int id { get; set; }
        public string userid { get; set; }
        public User User { get; set; }
        public string leavetype { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int dayscount { get; set; }
        public string? reason { get; set; }
        public string status { get; set; }
        public string? managercomment { get; set; }
        public DateTime requestdate { get; set; }
        public DateTime? decisiondate { get; set; }
        public string? reviewedby { get; set; }
    }
}
