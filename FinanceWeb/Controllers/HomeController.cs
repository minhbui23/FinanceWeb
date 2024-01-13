using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Finance.Models;
using Finance.DataAccess.DBContext;
using Microsoft.AspNetCore.Identity;
using Finance.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Finance.Models.ViewModel;

namespace FinanceWeb.Controllers;

[Authorize]
public class HomeController : Controller
{
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager; 
        private ApplicationUser _userWithActiveWallet;
        public HomeController(ApplicationDBContext db, UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signManager = signInManager;
            
        }
        private SpendingLineChartViewModel GetLineChartSpendingData(){
            // Fetch the spending data for the active wallet from the database
            var spendings = _db.Spendings
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()  // Switch to LINQ to Objects
                .GroupBy(s => s.Time.Date)
                .Select(g => new { Date = g.Key, TotalSpend = g.Sum(s => (decimal)s.Amount) })
                .OrderBy(x => x.Date)
                .ToList();

            // Convert the data to a format suitable for the chart
            var date_of_spend = spendings.Select(x => x.Date.ToShortDateString()).ToArray();
            var total_spend_pdate = spendings.Select(x => x.TotalSpend).ToArray();

            var viewModel = new SpendingLineChartViewModel
            {
                Labels = date_of_spend,
                Data = total_spend_pdate
            };

            return viewModel;

        }

        private IncomeLineChartViewModel GetLineChartIncomeData(){
            // Fetch the spending data for the active wallet from the database
            var incomes = _db.Incomes
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()  // Switch to LINQ to Objects
                .GroupBy(s => s.Time.Date)
                .Select(g => new { Date = g.Key, TotalIncome = g.Sum(s => (decimal)s.Amount) })
                .OrderBy(x => x.Date)
                .ToList();

            // Convert the data to a format suitable for the chart
            var date_of_income = incomes.Select(x => x.Date.ToShortDateString()).ToArray();
            var total_income_pdate = incomes.Select(x => x.TotalIncome).ToArray();

            var viewModel = new IncomeLineChartViewModel
            {
                Labels = date_of_income,
                Data = total_income_pdate
            };

            return viewModel;
        }
        private SpendingPieChartViewModel GetPieChartSpendingData()
        {
            // Fetch the data from the database
            var spendingsByCategory = _db.Spendings
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()
                .GroupBy(s => s.SpendingCategoryId) // Group by description (which is the category)
                .Select(g => new { Category = _db.SpendingCategories.FirstOrDefault(c => c.Id == g.Key)?.Name, TotalSpend = g.Sum(s => s.Amount) })
                .ToList();

            // Convert the data to a format suitable for the chart
            var spend_categories = spendingsByCategory.Select(x => x.Category).ToArray();
            var total_spend_pcategories = spendingsByCategory.Select(x => x.TotalSpend).ToArray();

            var viewModel = new SpendingPieChartViewModel
            {
                Labels = spend_categories,
                Data = total_spend_pcategories
            };
            return viewModel;
        }

        private IncomePieChartViewModel GetPieChartIncomeData()
        {
            // Fetch the data from the database
            var incomeByCategory = _db.Incomes
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()
                .GroupBy(s => s.IncomeCategoryId) // Group by description (which is the category)
                .Select(g => new { Category = _db.IncomeCategories.FirstOrDefault(c => c.Id == g.Key)?.Name, TotalIncome = g.Sum(s => s.Amount) })
                .ToList();

            // Convert the data to a format suitable for the chart
            var income_categories = incomeByCategory.Select(x => x.Category).ToArray();
            var total_income_pcategories = incomeByCategory.Select(x => x.TotalIncome).ToArray();

            var viewModel = new IncomePieChartViewModel
            {
                Labels = income_categories,
                Data = total_income_pcategories
            };
            return viewModel;
        }
    public async Task<IActionResult> Index()
    {
        
        var user = await _userManager.GetUserAsync(User);
        if(user != null){
            // Get the active wallet for the user
            _userWithActiveWallet = _userManager.Users
                .Include(u => u.Wallets)
                .ThenInclude(w => w.Spendings)
                .SingleOrDefault(u => u.Id == user.Id && u.ActiveWalletId != null);

            if (_userWithActiveWallet == null || _userWithActiveWallet.ActiveWalletId == null)
            {
                return NotFound("Active wallet not found");
            }

            var viewModel = new HomeIndexViewModel
            {
                SpendingLineChartViewModel = GetLineChartSpendingData(),
                IncomeLineChartViewModel = GetLineChartIncomeData(),
                SpendingPieChartViewModel = GetPieChartSpendingData(),
                IncomePieChartViewModel = GetPieChartIncomeData()
            };

            return View(viewModel);
            
        }
        return View();
    }
    

    public IActionResult Privacy()
    {
        return View();
    }
}