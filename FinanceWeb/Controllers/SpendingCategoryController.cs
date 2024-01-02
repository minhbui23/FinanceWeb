using Microsoft.AspNetCore.Mvc;
using Finance.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Finance.DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Finance.Controllers
{
    [Authorize]
    public class SpendingCategoryController : Controller
    {
        private readonly ApplicationDBContext _db;

        public SpendingCategoryController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: SpendingCategory
        public async Task<IActionResult> Index()
        {
            return View(await _db.SpendingCategories.ToListAsync());
        }

        // GET: SpendingCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SpendingCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] SpendingCategory spendingCategory)
        {
            if (ModelState.IsValid)
            {
                _db.Add(spendingCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(spendingCategory);
        }

        // GET: SpendingCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spendingCategory = await _db.SpendingCategories.FindAsync(id);
            if (spendingCategory == null)
            {
                return NotFound();
            }
            return View(spendingCategory);
        }

        // POST: SpendingCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] SpendingCategory spendingCategory)
        {
            if (id != spendingCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(spendingCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(spendingCategory);
        }

        // GET: SpendingCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spendingCategory = await _db.SpendingCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spendingCategory == null)
            {
                return NotFound();
            }

            return View(spendingCategory);
        }

        // POST: SpendingCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spendingCategory = await _db.SpendingCategories.FindAsync(id);
            _db.SpendingCategories.Remove(spendingCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}