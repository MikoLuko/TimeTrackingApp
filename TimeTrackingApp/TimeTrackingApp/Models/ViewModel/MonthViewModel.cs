using TimeTrackingApp.Models.Entities;

namespace TimeTrackingApp.Models.ViewModels
{
    public class MonthViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public double TotalHours { get; set; }
        public List<TimeEntry> Entries { get; set; }
    }
}

