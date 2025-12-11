using System;

    namespace TimeTrackingApp.Models.ViewModel
    {
        public class EditTimeEntriesViewModel
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string Name { get; set; }
            public DateTime EntryDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }
            public double TotalHours { get; set; }
            public string? Note { get; set; }
        }
    }

