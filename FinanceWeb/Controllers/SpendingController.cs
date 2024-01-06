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
    public class SpendingController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager; 
        public SpendingController(ApplicationDBContext db, UserManager<ApplicationUser> userManager,
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
                            .ThenInclude(w => w.Spendings)
                                .ThenInclude(s => s.Spending_Category)
                        .SingleOrDefault(u => u.Id == user.Id && u.ActiveWalletId != null);


                    if (userWithActiveWallet != null && userWithActiveWallet.ActiveWallet != null)
                    {
                        var spendingsForActiveWallet = userWithActiveWallet.ActiveWallet.Spendings.ToList();

                        return View(spendingsForActiveWallet);
                    }
                }
            }
            return View();
        }

        //when click Create in Index page
        public IActionResult Create(){
            var model = new SpendingCreateViewModel
            {
                SpendingCategories = _db.SpendingCategories.ToList(),
                Spending = new Spending()
            };
            return View(model);
        }

        //When Click Create in Create Page, a Spending object will be included in the Post method 
        [HttpPost]
        public async Task<IActionResult> Create(SpendingCreateViewModel model)
        {

                // Set the IdWallet property of the Spending object to the retrieved ActiveWalletID
                if (model.Spending == null)
                {
                    return BadRequest("Spending cannot be null");
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
                        var spendingtoDb = new Spending
                        {
                            Time = model.Spending.Time,
                            SpendingCategoryId = model.Spending.SpendingCategoryId,
                            Spending_Category = model.Spending.Spending_Category,
                            Amount = model.Spending.Amount,
                            IdWallet = userWithActiveWallet.ActiveWalletId.Value,
                        };

                        if (userWithActiveWallet.ActiveWallet != null)
                        {
                            userWithActiveWallet.ActiveWallet.Balance -= model.Spending.Amount;
                        }

                        _db.Spendings.Add(spendingtoDb);
                        await _db.SaveChangesAsync();

                        transaction.Commit();

                        TempData["success"] = "Spending created successfully";
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        transaction.Rollback();
                        // Handle any errors that occurred during the transaction
                    }
                }
            
            // Handle invalid model state or other errors
            return View(model);
        }


        public async Task<IActionResult> Edit(int? id){
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Spending? spendingFromDb = await _db.Spendings.FindAsync(id);
            decimal preAmount = spendingFromDb?.Amount ?? 0; // Add null check and provide a default value
            if (spendingFromDb == null)
            {
                return NotFound();
            }
            var viewModel = new SpendingEditViewModel
            {
                SpendingCategories = _db.SpendingCategories.ToList(),
                SpendingFromDb = spendingFromDb,
                PreAmount = preAmount
            };
            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(SpendingEditViewModel obj)
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
                    var spendingFromDb = await _db.Spendings.FindAsync(obj.SpendingFromDb.Id);
                    if (spendingFromDb == null)
                    {
                        return NotFound("Spending not found");
                    }

                    // Calculate the difference between the old and new amount
                    var amountDifference = obj.SpendingFromDb.Amount - spendingFromDb.Amount;

                    // Update the wallet balance
                    if (userWithActiveWallet.ActiveWallet != null)
                    {
                        userWithActiveWallet.ActiveWallet.Balance -= amountDifference;
                    }

                    // Update the spending
                    spendingFromDb.Amount = obj.SpendingFromDb.Amount;
                    spendingFromDb.SpendingCategoryId = obj.SpendingFromDb.SpendingCategoryId;
                    spendingFromDb.Time = obj.SpendingFromDb.Time;

                    await _db.SaveChangesAsync();

                    TempData["success"] = "Spending updated successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(obj);
        } 

        public async Task<IActionResult> Delete(int? id){
            if(id == null || id == -0) {
                return NotFound();
            }
            Spending? spendingFromDb = await _db.Spendings.FindAsync(id);
            if(spendingFromDb == null) {
                return NotFound();
            }
            var viewModel = new SpendingCreateViewModel
            {
                SpendingCategories = _db.SpendingCategories.ToList(),
                Spending = new Spending()
            };
            return View(viewModel);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id){
            Spending? obj = await _db.Spendings.FindAsync(id);
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
                
                _db.Spendings.Remove(obj);
                await _db.SaveChangesAsync();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }
    }
}