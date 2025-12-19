using Microsoft.AspNetCore.Mvc;

namespace TimeTrackingApp.Controllers
{
    using global::TimeTrackingApp.Data;
    using global::TimeTrackingApp.Models.Entities;
    using global::TimeTrackingApp.Models.ViewModel;
    using global::TimeTrackingApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Text;

    namespace TimeTrackingApp.Controllers
    {
        [Authorize(Roles ="Admin")]
        public class AdminController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<User> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IEmailService _emailService;

            public AdminController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
            {
                _context = context;
                _userManager = userManager;
                _roleManager = roleManager;
                _emailService = emailService;
            }

            public async Task<IActionResult> Panel()
            {
                var users = await _context.Users.ToListAsync();

                var model = new List<PanelViewModel>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    model.Add(new PanelViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Position = user.Position,
                        Department = user.Department,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt,
                        Role = roles.FirstOrDefault() ?? "Brak roli"
                    });
                }

                return View(model);
            }


            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(User user, string password)
            {
                if (!ModelState.IsValid)
                    return View(user);

                user.UserName = user.Email;
                user.NormalizedUserName = user.Email.ToUpper();
                user.NormalizedEmail = user.Email.ToUpper();
                user.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Panel));

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(user);
            }

            public async Task<IActionResult> Edit(string id)
            {
                if (id == null)
                    return NotFound();

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound();
                var userRoles = await _userManager.GetRolesAsync(user);
                var model = new EditViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Position = user.Position,
                    Department = user.Department,
                    IsActive = user.IsActive,
                    Role = userRoles.FirstOrDefault(),
                    AllRoles = _roleManager.Roles.Select(r => r.Name).ToList()
                };
                return View(model);
            }


            [HttpPost]
            public async Task<IActionResult> Edit(EditViewModel model)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                    return NotFound();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Position = model.Position;
                user.Department = model.Department;
                user.IsActive = model.IsActive;

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!string.IsNullOrEmpty(model.Role))
                    await _userManager.AddToRoleAsync(user, model.Role);

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Panel));

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }




            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                    await _userManager.DeleteAsync(user);

                return RedirectToAction(nameof(Panel));
            }

            [HttpPost]
            public async Task<IActionResult> Deactivate(string id)
            {
                if (id == null) return NotFound();

                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                user.IsActive = false;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Panel));
            }

            [HttpPost]
            public async Task<IActionResult> Activate(string id)
            {
                if (id == null) return NotFound();

                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                user.IsActive = true;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Panel));
            }

        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ResetUserPassword(string userId)
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest();

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var newPassword = "NoweHaslo123!";

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Reset hasła w TimeTrackingApp",
                        $@"
            <h3>Witaj {user.FirstName}!</h3>
            <p>Twoje hasło zostało zresetowane przez administratora.</p>
            <p>Nowe hasło: <b>{newPassword}</b></p>
            <p>Po zalogowaniu prosimy o zmianę hasła na własne.</p>"
                    );

                    TempData["Success"] = "Hasło zostało zresetowane i wysłane e-mailem do użytkownika.";
                }
                else
                {
                    TempData["Error"] = "Nie udało się zresetować hasła.";
                }

                return RedirectToAction("Panel");
            }
            public async Task<IActionResult> WorkReport(int? year, int? month)
            {
                year ??= DateTime.UtcNow.Year;
                month ??= DateTime.UtcNow.Month;

                var entries = await _context.TimeEntries
                    .Include(t => t.User)
                    .Where(t => t.entrydate.Year == year && t.entrydate.Month == month)
                    .OrderBy(t => t.User.LastName)
                    .ThenBy(t => t.entrydate)
                    .ToListAsync();

                ViewBag.Year = year;
                ViewBag.Month = month;

                return View(entries);
            }
            public async Task<IActionResult> LeaveRequests()
            {
                var requests = await _context.LeaveRequests
                    .Include(l => l.User)
                    .OrderByDescending(l => l.requestdate)
                    .ToListAsync();

                return View(requests);
            }
            public async Task<IActionResult> ExportWorkReportCsv(int year, int month)
            {
                var entries = await _context.TimeEntries
                    .Include(t => t.User)
                    .Where(t => t.entrydate.Year == year && t.entrydate.Month == month)
                    .ToListAsync();

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Imię,Nazwisko,Email,Data,Start,Koniec,Godziny");

                foreach (var e in entries)
                {
                    sb.AppendLine(
                        $"{e.User.FirstName}," +
                        $"{e.User.LastName}," +
                        $"{e.User.Email}," +
                        $"{e.entrydate:yyyy-MM-dd}," +
                        $"{e.starttime}," +
                        $"{e.endtime}," +
                        $"{e.totalhours:F2}"
                    );
                }

                return File(
                    System.Text.Encoding.UTF8.GetBytes(sb.ToString()),
                    "text/csv",
                    $"raport_czasu_{year}_{month}.csv"
                );
            }
            public async Task<IActionResult> ExportLeaveRequestsCsv()
            {
                var requests = await _context.LeaveRequests
                    .Include(l => l.User)
                    .OrderByDescending(l => l.requestdate)
                    .ToListAsync();

                var sb = new StringBuilder();
                sb.AppendLine("Imię,Nazwisko,Email,Typ urlopu,Od,Do,Liczba dni,Status,Złożono,Komentarz kierownika");

                foreach (var r in requests)
                {
                    sb.AppendLine(
                        $"{r.User?.FirstName}," +
                        $"{r.User?.LastName}," +
                        $"{r.User?.Email}," +
                        $"{r.leavetype}," +
                        $"{r.startdate:yyyy-MM-dd}," +
                        $"{r.enddate:yyyy-MM-dd}," +
                        $"{r.dayscount}," +
                        $"{r.status}," +
                        $"{r.requestdate:yyyy-MM-dd}," +
                        $"{r.managercomment}"
                    );
                }

                return File(
                    Encoding.UTF8.GetBytes(sb.ToString()),
                    "text/csv",
                    $"wnioski_urlopowe_{DateTime.UtcNow:yyyy_MM_dd}.csv"
                );
            }





        }
    }
}
