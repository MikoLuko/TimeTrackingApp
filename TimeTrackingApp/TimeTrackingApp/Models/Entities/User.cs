using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        [ValidateNever]
        public List<TimeEntry> TimeEntries { get; set; }
        [ValidateNever]
        public List<LeaveRequest> LeaveRequests { get; set; }
    }

}
