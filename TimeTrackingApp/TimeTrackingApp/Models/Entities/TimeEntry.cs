namespace TimeTrackingApp.Models.Entities
{
    public class TimeEntry
    {
        public int id { get; set; }
        public string userid { get; set; }
        public User User { get; set; }
        public DateTime entrydate { get; set; }
        public TimeSpan starttime { get; set; }
        public TimeSpan endtime { get; set; }
        public double totalhours { get; set; }
        public string? note { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
    }
}
