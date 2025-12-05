using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Models.Entities;

namespace TimeTrackingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<EmailLog> Emaillogs { get; set; }

    }
}
