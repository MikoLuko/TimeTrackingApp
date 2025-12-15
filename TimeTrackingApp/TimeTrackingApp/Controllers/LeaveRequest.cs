using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Data;
using TimeTrackingApp.Models.Entities;
using TimeTrackingApp.Models.ViewModel;
using TimeTrackingApp.Models.ViewModels;

namespace TimeTrackingApp.Controllers
{
    public class LeaveRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public LeaveRequestController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var requests = await _context.LeaveRequests
                .Where(r => r.userid == user.Id)
                .OrderByDescending(r => r.requestdate)
                .ToListAsync();

            var model = requests.Select(r => new LeaveRequestViewModel
            {
                Id = r.id,
                LeaveType = r.leavetype,
                StartDate = r.startdate,
                EndDate = r.enddate,
                DaysCount = r.dayscount,
                Reason = r.reason,
                Status = r.status,
                ManagerComment = r.managercomment,
                RequestDate = r.requestdate
            }).ToList();

            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            var leaveRequest = new LeaveRequest
            {
                userid = user.Id,
                leavetype = model.LeaveType,
                startdate = model.StartDate.ToUniversalTime(),
                enddate = model.EndDate.ToUniversalTime(),
                dayscount = (model.EndDate.Date - model.StartDate.Date).Days + 1,
                reason = model.Reason,
                status = "Oczekuje",
                requestdate = DateTime.UtcNow
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();


            TempData["Success"] = "Wniosek został utworzony.";
            return RedirectToAction("MyRequests");
        }
    }
}
