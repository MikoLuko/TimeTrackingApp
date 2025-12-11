namespace TimeTrackingApp.Models.ViewModel
{
    public class TimeEntryEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public double TotalHours { get; set; }
        public string? Note { get; set; }
    }
}
