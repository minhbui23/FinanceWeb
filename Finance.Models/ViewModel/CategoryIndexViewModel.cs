using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finance.Models.Models;

namespace Finance.Models.ViewModel
{
    public class CategoryIndexViewModel
    {
        public List<SpendingCategory>? SpendingCategories { get; set; }
        public List<IncomeCategory>? IncomeCategories { get; set; }
    }
}