using System;

namespace Masareefy.Models
{
    /// <summary>بيمثل مصروف واحد وبياناته الأساسية.</summary>
    public class Expense
    {
        // بيانات المصروف اللي بتستخدمها الشاشات والجداول.
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
    }
}
