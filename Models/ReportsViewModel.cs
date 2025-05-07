using System;
using System.Collections.Generic;

namespace ADAProject.Models
{
    public class ReportsViewModel
    {
        // 1) Monthly turnover chart
        public List<string>   MonthlyLabels   { get; set; } = new();
        public List<decimal>  MonthlyTurnover { get; set; } = new();

        // 2) Category counts chart
        public Dictionary<string, int> CategoryCounts { get; set; } = new();

        // 3) Weekly forecast vs actual
        public List<string> WeekLabels    { get; set; } = new();
        public List<int>    ActualSales   { get; set; } = new();
        public List<int>    ForecastSales { get; set; } = new();
    }
}
