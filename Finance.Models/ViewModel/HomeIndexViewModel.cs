using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Finance.Models.ViewModel
{
    public class IncomeLineChartViewModel
    {
        public string[] Labels { get; set; }
        public decimal[] Data { get; set; }
    }
    public class SpendingLineChartViewModel
    {
        public string[] Labels { get; set; }
        public decimal[] Data { get; set; }
    }
    public class IncomePieChartViewModel
    {
        public string[] Labels { get; set; }
        public decimal[] Data { get; set; }
    }
    public class SpendingPieChartViewModel
    {
        public string[] Labels { get; set; }
        public decimal[] Data { get; set; }
    }
    public class HomeIndexViewModel
    {
        public IncomeLineChartViewModel IncomeLineChartViewModel { get; set; }
        public SpendingLineChartViewModel SpendingLineChartViewModel { get; set; }
        public IncomePieChartViewModel IncomePieChartViewModel { get; set; }
        public SpendingPieChartViewModel SpendingPieChartViewModel { get; set; }
    }
}