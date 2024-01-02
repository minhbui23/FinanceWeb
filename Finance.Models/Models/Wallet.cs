using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Finance.Models.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 1)]
        public string ID_Card {get; set;}

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public ICollection<Spending> Spendings { get; set; } = new List<Spending>();
        public ICollection<Income> Incomes { get; set; } = new List<Income>();
    }
}