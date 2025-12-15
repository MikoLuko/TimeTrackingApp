using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Data;
using TimeTrackingApp.Models.Entities;
using TimeTrackingApp.Models.ViewModel;

namespace TimeTrackingApp.Controllers
{
    public class Manager : Controller
    {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<User> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public Manager(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
            {
                _context = context;
                _userManager = userManager;
                _roleManager = roleManager;
            }

        public async Task<IActionResult> ManagerPanel()
        {
            var manager = await _userManager.GetUserAsync(User);
            if (manager == null)
                return Unauthorized();

            var managerDept = manager.Department;

            var employees = await _userManager.Users
                .Where(u => u.Department == managerDept && u.IsActive)
                .ToListAsync();


            var model = new List<ManagerViewModel>();

            foreach (var emp in employees)
            {
                var entries = await _context.TimeEntries
                    .Where(t => t.userid == emp.Id
                        && t.entrydate.Month == DateTime.UtcNow.Month
                        && t.entrydate.Year == DateTime.UtcNow.Year)
                    .ToListAsync();

                model.Add(new ManagerViewModel
                {
                    UserId = emp.Id,
                    FullName = $"{emp.FirstName} {emp.LastName}",
                    Position = emp.Position,
                    Department = emp.Department,
                    TotalHoursThisMonth = entries.Sum(e => e.totalhours),
                    EntriesCount = entries.Count
                });
            }

            return View(model);
        }
        public async Task<IActionResult> History(string id)
        {
            var employee = await _userManager.FindByIdAsync(id);
            if (employee == null)
                return NotFound();

            var entries = await _context.TimeEntries
                .Where(t => t.userid == id)
                .OrderByDescending(t => t.entrydate)
                .ToListAsync();

            var model = entries.Select(e => new TimeEntryEditViewModel
            {
                Id = e.id,
                Name = $"{employee.FirstName} {employee.LastName}",
                EntryDate = e.entrydate,
                StartTime = e.starttime,
                EndTime = e.endtime,
                TotalHours = e.totalhours,
                Note = e.note
            }).ToList();

            ViewData["EmployeeId"] = id;

            return View(model);
        }
        public async Task<IActionResult> EditEntries(int id)
        {
            var entry = await _context.TimeEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            var employee = await _userManager.FindByIdAsync(entry.userid);

            var model = new EditTimeEntriesViewModel
            {
                Id = entry.id,
                Name = $"{employee.FirstName} {employee.LastName}",
                EntryDate = entry.entrydate,
                StartTime = entry.starttime,
                EndTime = entry.endtime,
                TotalHours = entry.totalhours,
                Note = entry.note
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEntries(EditTimeEntriesViewModel model)
        {
            var entry = await _context.TimeEntries.FindAsync(model.Id);
            if (entry == null)
                return NotFound();

            entry.starttime = model.StartTime;
            entry.endtime = model.EndTime;
            entry.note = model.Note;
            entry.modifiedby = User.Identity.Name;

            if (entry.endtime != null)
                entry.totalhours = (entry.endtime.Value - entry.starttime).TotalHours;

            await _context.SaveChangesAsync();
            return RedirectToAction("History", new { id = entry.userid });
        }
        public async Task<IActionResult> LeaveRequestsToApprove()
        {
            var manager = await _userManager.GetUserAsync(User);
            if (manager == null)
                return Unauthorized();

            var employees = await _userManager.Users
                .Where(u => u.Department == manager.Department && u.IsActive)
                .ToListAsync();

            var employeeIds = employees.Select(e => e.Id).ToList();

            var requests = await _context.LeaveRequests
                .Where(r => employeeIds.Contains(r.userid))
                .OrderByDescending(r => r.requestdate)
                .ToListAsync();

            var model = requests.Select(r =>
            {
                var emp = employees.First(e => e.Id == r.userid);
                return new LeaveRequestApprovalViewModel
                {
                    Id = r.id,
                    EmployeeName = $"{emp.FirstName} {emp.LastName}",
                    LeaveType = r.leavetype,
                    StartDate = r.startdate,
                    EndDate = r.enddate,
                    DaysCount = r.dayscount,
                    Reason = r.reason,
                    Status = r.status,
                    ManagerComment = r.managercomment,
                    RequestDate = r.requestdate
                };
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeaveRequest(LeaveRequestDecisionViewModel model)
        {
            var request = await _context.LeaveRequests.FindAsync(model.Id);
            if (request == null)
                return NotFound();

            request.status = model.Decision;
            request.managercomment = model.Comment;
            request.decisiondate = DateTime.UtcNow;
            request.reviewedby = User.Identity.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction("LeaveRequestsToApprove");
        }

    }
}
