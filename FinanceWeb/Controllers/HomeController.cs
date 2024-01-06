using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Finance.Models;
using Finance.DataAccess.DBContext;
using Microsoft.AspNetCore.Identity;
using Finance.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
        private void GetLineChartSpendingData(){
            // Fetch the spending data for the active wallet from the database
            var spendings = _db.Spendings
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()  // Switch to LINQ to Objects
                .GroupBy(s => s.Time.Date)
                .Select(g => new { Date = g.Key, TotalSpend = g.Sum(s => (decimal)s.Amount) })
                .OrderBy(x => x.Date)
                .ToList();

            // Convert the data to a format suitable for the chart
            var spend_labels = spendings.Select(x => x.Date.ToShortDateString()).ToArray();
            var spend_data = spendings.Select(x => x.TotalSpend).ToArray();

            // Pass the data to the view
            ViewBag.SpendLabels = Newtonsoft.Json.JsonConvert.SerializeObject(spend_labels);
            ViewBag.SpendData = Newtonsoft.Json.JsonConvert.SerializeObject(spend_data);
        }

        private void GetLineChartIncomeData(){
            // Fetch the spending data for the active wallet from the database
            var incomes = _db.Incomes
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()  // Switch to LINQ to Objects
                .GroupBy(s => s.Time.Date)
                .Select(g => new { Date = g.Key, TotalIncome = g.Sum(s => (decimal)s.Amount) })
                .OrderBy(x => x.Date)
                .ToList();

            // Convert the data to a format suitable for the chart
            var income_labels = incomes.Select(x => x.Date.ToShortDateString()).ToArray();
            var income_data = incomes.Select(x => x.TotalIncome).ToArray();

            // Pass the data to the view
            ViewBag.IncomeLabels = Newtonsoft.Json.JsonConvert.SerializeObject(income_labels);
            ViewBag.IncomeData = Newtonsoft.Json.JsonConvert.SerializeObject(income_data);
        }
        private void GetPieChartSpendingData()
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
            var spends_data = spendingsByCategory.Select(x => x.TotalSpend).ToArray();

            // Pass the data to the view
            ViewBag.SpendCategories = Newtonsoft.Json.JsonConvert.SerializeObject(spend_categories);
            ViewBag.SpendData = Newtonsoft.Json.JsonConvert.SerializeObject(spends_data);
        }

        private void GetPieChartIncomeData()
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
            var incomes_data = incomeByCategory.Select(x => x.TotalIncome).ToArray();

            // Pass the data to the view
            ViewBag.IncomeCategories = Newtonsoft.Json.JsonConvert.SerializeObject(income_categories);
            ViewBag.IncomeData = Newtonsoft.Json.JsonConvert.SerializeObject(incomes_data);
        }
    public IActionResult Index()
    {
        
        var user =  _userManager.GetUserAsync(User).Result;
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

            GetLineChartSpendingData();
            GetLineChartIncomeData();

            GetPieChartSpendingData();
            GetPieChartIncomeData();

        }
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }
}