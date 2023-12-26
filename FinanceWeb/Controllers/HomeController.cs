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
        private void GetLineChartData(){
            // Fetch the spending data for the active wallet from the database
            var spendings = _db.Spendings
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()  // Switch to LINQ to Objects
                .GroupBy(s => s.Time.Date)
                .Select(g => new { Date = g.Key, TotalSpend = g.Sum(s => (decimal)s.Amount) })
                .OrderBy(x => x.Date)
                .ToList();

            // Convert the data to a format suitable for the chart
            var labels = spendings.Select(x => x.Date.ToShortDateString()).ToArray();
            var data = spendings.Select(x => x.TotalSpend).ToArray();

            // Pass the data to the view
            ViewBag.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(labels);
            ViewBag.Data = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
        private void GetPieChartData(){
            

            // Fetch the data from the database
            var spendingsByCategory = _db.Spendings
                .Where(s => s.IdWallet == _userWithActiveWallet.ActiveWalletId)
                .AsEnumerable()
                .GroupBy(s => s.Description) // Group by description (which is the category)
                .Select(g => new { Category = g.Key, TotalSpend = g.Sum(s => s.Amount) })
                .ToList();

            // Convert the data to a format suitable for the chart
            var categories = spendingsByCategory.Select(x => x.Category.ToString()).ToArray(); // Convert enum to string
            var spends = spendingsByCategory.Select(x => x.TotalSpend).ToArray();

            // Pass the data to the view
            ViewBag.Categories = Newtonsoft.Json.JsonConvert.SerializeObject(categories);
            ViewBag.Spends = Newtonsoft.Json.JsonConvert.SerializeObject(spends);

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

            GetLineChartData();
            GetPieChartData();

        }
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }
}