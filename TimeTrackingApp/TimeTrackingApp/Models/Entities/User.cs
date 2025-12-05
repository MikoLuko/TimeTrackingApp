using Microsoft.AspNetCore.Identity;

namespace TimeTrackingApp.Models.Entities
{

        public class User: IdentityUser
   {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Position { get; set; } = "";
        public string Department { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<TimeEntry> TimeEntries { get; set; }
        public List<LeaveRequest> LeaveRequests { get; set; }
    }

}
