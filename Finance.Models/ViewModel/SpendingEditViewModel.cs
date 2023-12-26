using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finance.Models.Models;

namespace Finance.Models.ViewModel
{
    public class SpendingEditViewModel 
    {
        public Spending SpendingFromDb { get; set; }
        public decimal PreAmount { get; set; }
    }
}