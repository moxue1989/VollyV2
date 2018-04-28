using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;

namespace VollyV2.Controllers.Mvc
{
    [Authorize(Roles = "Admin")]
    public class CausesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CausesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Causes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Causes.ToListAsync());
        }

        // GET: Causes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cause = await _context.Causes
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cause == null)
            {
                return NotFound();
            }

            return View(cause);
        }

        // GET: Causes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Causes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Cause cause)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cause);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cause);
        }

        // GET: Causes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cause = await _context.Causes.SingleOrDefaultAsync(m => m.Id == id);
            if (cause == null)
            {
                return NotFound();
            }
            return View(cause);
        }

        // POST: Causes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Cause cause)
        {
            if (id != cause.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cause);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CauseExists(cause.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cause);
        }

        // GET: Causes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cause = await _context.Causes
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cause == null)
            {
                return NotFound();
            }

            return View(cause);
        }

        // POST: Causes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cause = await _context.Causes.SingleOrDefaultAsync(m => m.Id == id);
            _context.Causes.Remove(cause);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CauseExists(int id)
        {
            return _context.Causes.Any(e => e.Id == id);
        }
    }
}
