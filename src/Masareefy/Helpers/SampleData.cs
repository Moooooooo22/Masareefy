using System;
using System.Collections.Generic;
using System.Drawing;
using Masareefy.Models;

namespace Masareefy.Helpers
{
    /// <summary>
    /// Fake data so the screens look real. LATER we replace this with the database.
    /// </summary>
    public static class SampleData
    {
        public static readonly List<Category> Categories = new List<Category>
        {
            new Category { Id = 1, Name = "الطعام",     ColorValue = Theme.Success, MonthlyBudget = 1000, Spent = 850 },
            new Category { Id = 2, Name = "المواصلات",  ColorValue = Theme.Blue,    MonthlyBudget = 600,  Spent = 400 },
            new Category { Id = 3, Name = "الفواتير",   ColorValue = Theme.Warning, MonthlyBudget = 400,  Spent = 300 },
            new Category { Id = 4, Name = "التسوق",     ColorValue = Theme.Purple,  MonthlyBudget = 800,  Spent = 450 },
            new Category { Id = 5, Name = "الترفيه",    ColorValue = Theme.Pink,    MonthlyBudget = 500,  Spent = 200 },
            new Category { Id = 6, Name = "أخرى",       ColorValue = Theme.Gray,    MonthlyBudget = 300,  Spent = 250 },
        };

        public static readonly List<Expense> Expenses = new List<Expense>
        {
            E(1,  2026,10,16, "غداء مع الأصدقاء",   "الطعام",    250),
            E(2,  2026,10,15, "مواصلات عمل",        "المواصلات", 80),
            E(3,  2026,10,14, "شراء ملابس",         "التسوق",    600),
            E(4,  2026,10,12, "فاتورة كهرباء",      "الفواتير",  300),
            E(5,  2026,10,10, "اشتراك نادي",        "الترفيه",   200),
            E(6,  2026,10,9,  "سوبر ماركت",         "الطعام",    420),
            E(7,  2026,10,8,  "بنزين",              "المواصلات", 320),
            E(8,  2026,10,7,  "فاتورة إنترنت",      "الفواتير",  250),
            E(9,  2026,10,5,  "سينما",              "الترفيه",   150),
            E(10, 2026,10,4,  "قهوة",               "الطعام",    90),
            E(11, 2026,10,3,  "هدية",               "أخرى",      250),
            E(12, 2026,10,2,  "أحذية",              "التسوق",    350),
            E(13, 2026,10,1,  "عشاء",               "الطعام",    90),
            E(14, 2026,9,30,  "تاكسي",              "المواصلات", 120),
        };

        public static int NextExpenseId = 15;
        public static int NextCategoryId = 7;

        public static readonly string[] MonthNames =
        {
            "يناير","فبراير","مارس","أبريل","مايو","يونيو",
            "يوليو","أغسطس","سبتمبر","أكتوبر","نوفمبر","ديسمبر"
        };

        // last 6 months (left to right, oldest first) - like the design
        public static readonly string[] Last6Labels = { "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر" };
        public static readonly double[] Last6Values = { 1500, 1300, 2600, 2400, 2500, 3100 };

        public static List<ChartSlice> Distribution()
        {
            return new List<ChartSlice>
            {
                new ChartSlice("الطعام",    35, Theme.Success),
                new ChartSlice("المواصلات", 20, Theme.Blue),
                new ChartSlice("الفواتير",  15, Theme.Warning),
                new ChartSlice("التسوق",    12, Theme.Purple),
                new ChartSlice("الترفيه",   10, Theme.Pink),
                new ChartSlice("أخرى",      8,  Theme.Gray),
            };
        }

        public static Color ColorOf(string categoryName)
        {
            foreach (var c in Categories)
                if (c.Name == categoryName) return c.ColorValue;
            return Theme.Gray;
        }

        static Expense E(int id, int y, int m, int d, string desc, string cat, decimal amount)
        {
            return new Expense { Id = id, Date = new DateTime(y, m, d), Description = desc, CategoryName = cat, Amount = amount, Notes = "" };
        }
    }
}
