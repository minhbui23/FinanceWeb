using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finance.Models.Models;

namespace Finance.Models.ViewModel
{
    public class IncomeEditViewModel
    {
        public List<IncomeCategory>? IncomeCategories { get; set; }
        public Income IncomeFromDb { get; set; }
        public decimal PreAmount { get; set; }
    }
}