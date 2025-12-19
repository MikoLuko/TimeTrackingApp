using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Data;
using TimeTrackingApp.Models.Entities;
using TimeTrackingApp.Models.ViewModel;
using TimeTrackingApp.Models.ViewModels;
using TimeTrackingApp.Services;

namespace TimeTrackingApp.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public LeaveRequestController(ApplicationDbContext context, UserManager<User> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
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
        public async Task<IActionResult> Create(string leavetype, DateTime startdate, DateTime enddate, string? reason)
        {
            var user = await _userManager.GetUserAsync(User);

            if (startdate > enddate)
            {
                TempData["error"] = "Data początkowa nie może być później niż końcowa.";
                return RedirectToAction("Create");
            }

            var leaveRequest = new LeaveRequest
            {
                userid = user.Id,
                leavetype = leavetype,
                startdate = startdate.ToUniversalTime(),
                enddate = enddate.ToUniversalTime(),
                dayscount = (enddate.Date - startdate.Date).Days + 1,
                reason = reason,
                status = "Oczekuje",
                requestdate = DateTime.UtcNow
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Potwierdzenie złożenia wniosku urlopowego",
                $@"
        <h3>Witaj {user.FirstName}!</h3>
        <p>Twój wniosek urlopowy został złożony pomyślnie.</p>
        <ul>
            <li>Typ urlopu: <b>{leaveRequest.leavetype}</b></li>
            <li>Od: <b>{leaveRequest.startdate:dd.MM.yyyy}</b></li>
            <li>Do: <b>{leaveRequest.enddate:dd.MM.yyyy}</b></li>
            <li>Liczba dni: <b>{leaveRequest.dayscount}</b></li>
            <li>Powód: {leaveRequest.reason}</li>
        </ul>
        <p>Status wniosku: <b>{leaveRequest.status}</b></p>"
            );

            TempData["Success"] = "Wniosek został utworzony.";
            return RedirectToAction("MyRequests");
        }
    }
}
