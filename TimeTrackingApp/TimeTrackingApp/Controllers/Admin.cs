using Microsoft.AspNetCore.Mvc;

namespace TimeTrackingApp.Controllers
{
    using global::TimeTrackingApp.Data;
    using global::TimeTrackingApp.Models.Entities;
    using global::TimeTrackingApp.Models.ViewModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace TimeTrackingApp.Controllers
    {
        public class AdminController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<User> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public AdminController(ApplicationDbContext context, UserManager<User> userManager,RoleManager<IdentityRole> roleManager)
            {
                _context = context;
                _userManager = userManager;
                _roleManager = roleManager;
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

        }

    }
}
