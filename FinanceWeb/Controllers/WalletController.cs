using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Finance.DataAccess.DBContext;
using Finance.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace FinanceWeb.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager; 
        public WalletController(ApplicationDBContext db, UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            
            var user = await _userManager.Users
                .Include(u => u.Wallets)  // Eager loading to include Wallets
                .SingleOrDefaultAsync(u => u.Id == _userManager.GetUserAsync(User).Result.Id);

            if (user != null)
            {
                return View(user.Wallets);
            }
            
            return View();
        }

        public IActionResult Create(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Wallet obj)
        {
            if (ModelState.IsValid)
            {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {                        
                        _db.Wallets.Add(obj);                                               
                        await _db.SaveChangesAsync();
                        TempData["success"] = "Wallet created successfully";
                        return RedirectToAction("Index");
                    }
                
            }

            //check error
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }


            return View(obj);
        }   
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Wallet walletFromDb = await _db.Wallets.FindAsync(id);

            if (walletFromDb == null)
            {
                return NotFound();
            }

            return View(walletFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Wallet updatedWallet)
        {

            if (ModelState.IsValid)
            {
                _db.Wallets.Update(updatedWallet);
                await _db.SaveChangesAsync();

                TempData["success"] = "Wallet updated successfully";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(updatedWallet);
        }

        public async Task<IActionResult> Delete(int? id){

            if(id == null || id == -0) {
                return NotFound();
            }
            Wallet? walletFromDb = await _db.Wallets.FindAsync(id);

            if(walletFromDb == null) {
                return NotFound();
            }

            return View(walletFromDb);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id){

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            Wallet? obj = await _db.Wallets.FindAsync(id);
            if (obj == null || obj.UserId != user.Id)
            {
                return NotFound();
            }

            if (id == user.ActiveWalletId)
            {
                user.ActiveWalletId = null;
            }

            _db.Wallets.Remove(obj);
            await _db.SaveChangesAsync();

            TempData["success"] = "Wallet deleted successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SwitchWallet(int walletId)
        {
            if (_signManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var userWithWallets = _userManager.Users.Include(u => u.Wallets).SingleOrDefault(u => u.Id == user.Id);
                    var selectedWallet = userWithWallets?.Wallets?.Any(w => w.Id == walletId);
                    if (selectedWallet != null)
                    {
                        // Update the user's active wallet
                        user.ActiveWalletId = walletId;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            return RedirectToAction("Index");
        }


    }
}