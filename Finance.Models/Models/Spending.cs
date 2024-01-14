using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Models.Models
{
    public class Spending
    {        
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public int SpendingCategoryId { get; set; }

        [ForeignKey("SpendingCategoryId")]
        public SpendingCategory? Spending_Category { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public int IdWallet { get; set; }

        [ForeignKey("IdWallet")]
        public Wallet? Wallet { get; set; }
    }
    
}