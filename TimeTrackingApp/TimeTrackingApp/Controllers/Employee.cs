using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Data;
using TimeTrackingApp.Models.Entities;
using TimeTrackingApp.Models.ViewModels;
using TimeTrackingApp.Services;

[Authorize(Roles = "Pracownik,Admin,Kierownik")]
public class Employee : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public Employee(ApplicationDbContext context, UserManager<User> userManager, IEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> StartWork()
    {
        var user = await _userManager.GetUserAsync(User);

        var openEntry = await _context.TimeEntries
            .Where(x => x.userid == user.Id && x.endtime == null)
            .FirstOrDefaultAsync();

        if (openEntry != null)
        {
            TempData["Error"] = "Poprzednia sesja pracy nie została zakończona.";
            return RedirectToAction("Dashboard");
        }

        var entry = new TimeEntry
        {
            userid = user.Id,
            entrydate = DateTime.UtcNow.Date,
            starttime = DateTime.UtcNow.TimeOfDay,
            createdat = DateTime.UtcNow
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Rozpoczęto pracę.";
        return RedirectToAction("Panel");
    }

    [HttpPost]
    public async Task<IActionResult> EndWork()
    {
        var user = await _userManager.GetUserAsync(User);

        var openEntry = await _context.TimeEntries
            .Where(x => x.userid == user.Id && x.endtime == null)
            .FirstOrDefaultAsync();

        if (openEntry == null)
        {
            TempData["Error"] = "Nie masz rozpoczętej pracy.";
            return RedirectToAction("Dashboard");
        }

        openEntry.endtime = DateTime.UtcNow.TimeOfDay;
        openEntry.totalhours = (openEntry.endtime.Value - openEntry.starttime).TotalHours;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Zakończono pracę.";
        return RedirectToAction("Panel");
    }

    public async Task<IActionResult> Panel()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var yesterday = DateTime.UtcNow.AddDays(-1).Date;
        if (yesterday.DayOfWeek != DayOfWeek.Saturday && yesterday.DayOfWeek != DayOfWeek.Sunday)
        {
            bool hasEntry = await _context.TimeEntries
                .AnyAsync(t => t.userid == user.Id && t.entrydate.Date == yesterday);

            if (!hasEntry)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Brak wpisów czasu pracy wczoraj",
                    $@"
                <h3>Witaj {user.FirstName}!</h3>
                <p>Nie odnotowaliśmy wczoraj żadnych wpisów czasu pracy. Prosimy o uzupełnienie danych w systemie.</p>"
                );
            }
        }
        var openEntry = await _context.TimeEntries
            .Where(x => x.userid == user.Id && x.endtime == null)
            .FirstOrDefaultAsync();

        var entries = await _context.TimeEntries
            .Where(x => x.userid == user.Id)
            .OrderByDescending(x => x.entrydate)
            .Take(30)
            .ToListAsync();

        var vm = new EmpPanelViewModel
        {
            IsWorking = openEntry != null,
            CurrentEntry = openEntry,
            Entries = entries
        };

        return View(vm);
    }

    public async Task<IActionResult> Month(int month = 0, int year = 0)
    {
        var user = await _userManager.GetUserAsync(User);

        if (month == 0) month = DateTime.UtcNow.Month;
        if (year == 0) year = DateTime.UtcNow.Year;

        var entries = await _context.TimeEntries
            .Where(x => x.userid == user.Id &&
                        x.entrydate.Month == month &&
                        x.entrydate.Year == year)
            .ToListAsync();

        var vm = new MonthViewModel
        {
            Month = month,
            Year = year,
            Entries = entries,
            TotalHours = entries.Sum(x => x.totalhours)
        };

        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> GetLeavesForCalendar()
    {
        var leaves = await _context.LeaveRequests
            .Include(l => l.User)
            .Where(l => l.status == "Zaakceptowano")
            .Select(l => new
            {
                title = $"{l.User.FirstName} {l.User.LastName} ({l.leavetype})",
                start = l.startdate.ToString("yyyy-MM-dd"),
                end = l.enddate.AddDays(1).ToString("yyyy-MM-dd"),

                backgroundColor =
                    l.leavetype == "Wypoczynkowy" ? "#10274e" :
                    l.leavetype == "Bezpłatny" ? "#3d5eae" :
                    l.leavetype == "Na żądanie" ? "#7c98fb" :
                    "#0d6efd",

                borderColor =
                    l.leavetype == "Wypoczynkowy" ? "#10274e" :
                    l.leavetype == "Bezpłatny" ? "#3d5eae" :
                    l.leavetype == "Na żądanie" ? "#7c98fb" :
                    "#0d6efd"
            })
            .ToListAsync();

        return Json(leaves);
    }

}

