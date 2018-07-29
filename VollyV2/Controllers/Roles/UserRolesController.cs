using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Models;
using VollyV2.Models.Volly;

namespace VollyV2.Controllers.Roles
{
    [Authorize(Roles = "PowerUser")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserRolesController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: UserRoles
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> applicationUsers = await _context.ApplicationUser.ToListAsync();

            IEnumerable<UserRoleModel> userRoleModels = applicationUsers.Select(u => new UserRoleModel()
            {
                Id = u.Id,
                UserName = u.UserName,
                RoleNames = _userManager.GetRolesAsync(u).Result
            });

            return View(userRoleModels);
        }

        // GET: UserRoles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            IList<string> roleNames = await _userManager.GetRolesAsync(applicationUser);

            UserRoleModel userRoleModel = new UserRoleModel
            {
                Roles = new SelectList(_roleManager.Roles.ToList(),
                    "Name", "Name"),
                Id = applicationUser.Id,
                RoleNames = roleNames
            };

            return View(userRoleModel);
        }

        // POST: UserRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,RoleNames")] UserRoleModel userRoleModel)
        {
            if (id != userRoleModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
                IList<string> existingRoleNames = await _userManager.GetRolesAsync(applicationUser);
                IList<string> newRoleNames = userRoleModel.RoleNames;

                IEnumerable<string> rolesToRemove = existingRoleNames.Except(newRoleNames);
                IEnumerable<string> rolesToAdd = newRoleNames.Except(existingRoleNames);

                await _userManager.AddToRolesAsync(applicationUser, rolesToAdd);
                await _userManager.RemoveFromRolesAsync(applicationUser, rolesToRemove);

                return RedirectToAction("Index");
            }
            return View(userRoleModel);
        }

        // GET: UserRoles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUser.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }
    }
}
