using Microsoft.AspNetCore.Mvc;
using Finance.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Finance.DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Finance.Controllers
{
    [Authorize]
    public class IncomeCategoryController : Controller
    {
        private readonly ApplicationDBContext _db;

        public IncomeCategoryController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: IncomeCategory
        public async Task<IActionResult> Index()
        {
            return View(await _db.IncomeCategories.ToListAsync());
        }

        // GET: IncomeCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IncomeCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] IncomeCategory IncomeCategory)
        {
            if (ModelState.IsValid)
            {
                _db.Add(IncomeCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(IncomeCategory);
        }

        // GET: IncomeCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeCategory = await _db.IncomeCategories.FindAsync(id);
            if (incomeCategory == null)
            {
                return NotFound();
            }
            return View(incomeCategory);
        }

        // POST: IncomeCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] IncomeCategory IncomeCategory)
        {
            if (id != IncomeCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(IncomeCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(IncomeCategory);
        }

        // GET: IncomeCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeCategory = await _db.IncomeCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incomeCategory == null)
            {
                return NotFound();
            }

            return View(incomeCategory);
        }

        // POST: IncomeCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incomeCategory = await _db.IncomeCategories.FindAsync(id);
            _db.IncomeCategories.Remove(incomeCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}