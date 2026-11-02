using System.Drawing;

namespace Masareefy.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color ColorValue { get; set; }
        public decimal MonthlyBudget { get; set; }
        public decimal Spent { get; set; }

        public int Percent
        {
            get { return MonthlyBudget <= 0 ? 0 : (int)System.Math.Round(Spent / MonthlyBudget * 100); }
        }
    }
}
