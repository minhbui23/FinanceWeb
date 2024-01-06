using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Finance.DataAccess.DBContext;
using Finance.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _db;

        public CategoryController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var spendingCategories = await _db.SpendingCategories.ToListAsync();
            var incomeCategories = await _db.IncomeCategories.ToListAsync();

            var viewModel = new CategoryIndexViewModel
            {
                SpendingCategories = spendingCategories,
                IncomeCategories = incomeCategories
            };

            return View(viewModel);
        }
    }
}