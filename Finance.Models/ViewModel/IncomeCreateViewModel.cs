using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finance.Models.Models;

namespace Finance.Models.ViewModel
{
    public class IncomeCreateViewModel
    {
        public List<IncomeCategory>? IncomeCategories { get; set; }
        public Income Income { get; set; }
    }
}