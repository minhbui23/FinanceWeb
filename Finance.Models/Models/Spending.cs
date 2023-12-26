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
        [EnumDataType(typeof(SpendingCategory))]
        public SpendingCategory Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public int IdWallet { get; set; }

        [ForeignKey("IdWallet")]
        public Wallet? Wallet { get; set; }
    }
    public enum SpendingCategory
    {
        [Display(Name = "Food")]
        Food,
        [Display(Name = "Entertainment")]
        Entertainment,
        [Display(Name = "Moving")]
        Moving,
        [Display(Name = "Tuition")]
        Tuition,
        [Display(Name = "RentHouse")]
        RentHouse,
        [Display(Name = "Other")]
        Other
    }
    
}