using System;

namespace Masareefy.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
    }
}
