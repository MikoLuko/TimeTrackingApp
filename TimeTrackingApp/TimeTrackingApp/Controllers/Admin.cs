using Microsoft.AspNetCore.Mvc;

namespace TimeTrackingApp.Controllers
{
    using global::TimeTrackingApp.Data;
    using global::TimeTrackingApp.Models.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace TimeTrackingApp.Controllers
    {
        public class AdminController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<User> _userManager;

            public AdminController(ApplicationDbContext context, UserManager<User> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task<IActionResult> Panel()
            {
                var users = await _context.Users.ToListAsync();
                return View(users);
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Position,Department,IsActive")] User user, string password)
            {
                if (ModelState.IsValid)
                {
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return View(user);
            }

            public async Task<IActionResult> Edit(string id)
            {
                if (id == null) return NotFound();

                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                return View(user);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,Position,Department,IsActive")] User user)
            {
                if (id != user.Id) return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Users.Any(e => e.Id == id))
                            return NotFound();
                        else
                            throw;
                    }
                }
                return View(user);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(string id)
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }

}
