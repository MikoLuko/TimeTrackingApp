namespace TimeTrackingApp.Models.ViewModel
{
        public class ManagerViewModel
        {
            public string UserId { get; set; }
            public string FullName { get; set; }
            public string Position { get; set; }
            public string Department { get; set; }
            public double TotalHoursThisMonth { get; set; }
            public int EntriesCount { get; set; }
        }
}
