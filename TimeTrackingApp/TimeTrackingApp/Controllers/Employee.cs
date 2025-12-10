using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Data;
using TimeTrackingApp.Models.Entities;
using TimeTrackingApp.Models.ViewModels;

[Authorize(Roles = "Pracownik,Admin,Kierownik")]
public class Employee : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public Employee(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
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

        var openEntry = await _context.TimeEntries
            .Where(x => x.userid == user.Id && x.endtime == null)
            .FirstOrDefaultAsync();

        var vm = new EmpPanelViewModel
        {
            IsWorking = openEntry != null,
            CurrentEntry = openEntry
        };

        return View(vm);
    }

    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);

        var entries = await _context.TimeEntries
            .Where(x => x.userid == user.Id)
            .OrderByDescending(x => x.entrydate)
            .ToListAsync();

        var vm = new HistoryViewModel
        {
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
}
