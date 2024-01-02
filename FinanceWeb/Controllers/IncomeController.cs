using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Finance.DataAccess.DBContext;
using Finance.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Finance.Models.ViewModel;
using Microsoft.CodeAnalysis.CSharp;

namespace Finance_Web.Controllers
{
    [Authorize]
    public class IncomeController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager; 
        public IncomeController(ApplicationDBContext db, UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager){
            _db=db;
            _userManager = userManager;
            _signManager = signInManager;
        }
        
        public async Task<IActionResult> Index()
        {
            if (_signManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    
                    var userWithActiveWallet = _userManager.Users
                        .Include(u => u.Wallets)
                        .ThenInclude(w => w.Incomes)
                        .SingleOrDefault(u => u.Id == user.Id && u.ActiveWalletId != null);
                    

                    if (userWithActiveWallet != null && userWithActiveWallet.ActiveWallet != null)
                    {
                        var incomesForActiveWallet = userWithActiveWallet.ActiveWallet.Incomes.ToList();

                        return View(incomesForActiveWallet);
                    }
                }
            }
            return View();
        }

        
        public IActionResult Create(){
            return View();
        }

         
        [HttpPost]
        public async Task<IActionResult> Create(Income income)
        {
            if (ModelState.IsValid)
            {
                // Set the IdWallet property of the income object to the retrieved ActiveWalletID
                if (income == null)
                {
                    return BadRequest("Income cannot be null");
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var userWithActiveWallet = await _userManager.Users
                        .Include(u => u.Wallets)
                        .SingleOrDefaultAsync(u => u.Id == user.Id);

                if (userWithActiveWallet?.ActiveWalletId == null)
                {
                    return NotFound("Active wallet not found");
                }

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var incometoDb = new Income
                        {
                            Time = income.Time,
                            IncomeCategoryId = income.IncomeCategoryId,
                            Amount = income.Amount,
                            IdWallet = userWithActiveWallet.ActiveWalletId.Value,
                        };

                        if (userWithActiveWallet.ActiveWallet != null)
                        {
                            userWithActiveWallet.ActiveWallet.Balance += income.Amount;
                        }

                        _db.Incomes.Add(incometoDb);
                        await _db.SaveChangesAsync();

                        transaction.Commit();

                        TempData["success"] = "Income created successfully";
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        transaction.Rollback();
                        // Handle any errors that occurred during the transaction
                    }
                }
            }
            // Handle invalid model state or other errors
            return View(income);
        }


        public async Task<IActionResult> Edit(int? id){
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Income? incomeFromDb = await _db.Incomes.FindAsync(id);
            decimal preAmount = incomeFromDb?.Amount ?? 0; // Add null check and provide a default value
            if (incomeFromDb == null)
            {
                return NotFound();
            }
            var viewModel = new IncomeEditViewModel
            {
                IncomeFromDb = incomeFromDb,
                PreAmount = preAmount
            };
            return View(viewModel);
        }
        
        [HttpPost]
public async Task<IActionResult> Edit(IncomeEditViewModel obj)
{
    if (ModelState.IsValid)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var userWithActiveWallet = await _userManager.Users
            .Include(u => u.Wallets)
            .SingleOrDefaultAsync(u => u.Id == user.Id);

        if (userWithActiveWallet != null && userWithActiveWallet.ActiveWalletId.HasValue)
        {
            var incomeFromDb = await _db.Incomes.FindAsync(obj.IncomeFromDb.Id);
            if (incomeFromDb == null)
            {
                return NotFound("Income not found");
            }

            // Calculate the difference between the old and new amount
            var amountDifference = obj.IncomeFromDb.Amount - incomeFromDb.Amount;

            // Update the wallet balance
            if (userWithActiveWallet.ActiveWallet != null)
            {
                userWithActiveWallet.ActiveWallet.Balance += amountDifference;
            }

            // Update the income
            incomeFromDb.Amount = obj.IncomeFromDb.Amount;
            incomeFromDb.IncomeCategoryId = obj.IncomeFromDb.IncomeCategoryId;
            incomeFromDb.Time = obj.IncomeFromDb.Time;

            await _db.SaveChangesAsync();

            TempData["success"] = "Income updated successfully";
            return RedirectToAction("Index");
        }
    }

    return View(obj);
} 

        public async Task<IActionResult> Delete(int? id){
            if(id == null || id == -0) {
                return NotFound();
            }
            Income? incomeFromDb = await _db.Incomes.FindAsync(id);
            if(incomeFromDb == null) {
                return NotFound();
            }
            return View(incomeFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id){
            Income? obj = await _db.Incomes.FindAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var userWithActiveWallet = await _userManager.Users
                    .Include(u => u.Wallets)
                    .SingleOrDefaultAsync(u => u.Id == user.Id);

            if (userWithActiveWallet != null && userWithActiveWallet.ActiveWalletId.HasValue)
            {
                if (userWithActiveWallet.ActiveWallet != null)
                {
                    userWithActiveWallet.ActiveWallet.Balance += obj.Amount;
                }
                
                _db.Incomes.Remove(obj);
                await _db.SaveChangesAsync();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }
    }
}