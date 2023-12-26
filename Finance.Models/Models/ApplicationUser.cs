using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name {  get; set; }
        public string? Address { get; set; }

        public ICollection<Wallet> Wallets {  get; set; } = new List<Wallet>();
        public int? ActiveWalletId { get; set; }
        
        [ForeignKey("ActiveWalletId")]
        public  Wallet? ActiveWallet { get; set; } 

    }
}
