using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Masareefy.Models;

namespace Masareefy.Helpers
{
    /// <summary>بيانات ديمو للواجهات، والتعديلات عليها مش بتتحفظ.</summary>
    public static class SampleData
    {
        // تصنيفات ومصاريف للتجربة بس.
        public static readonly List<Category> Categories = new List<Category>
        {
            new Category { Id = 1, Name = "الطعام",     ColorValue = Theme.Success, MonthlyBudget = 1000, Spent = 850 },
            new Category { Id = 2, Name = "المواصلات",  ColorValue = Theme.Blue,    MonthlyBudget = 600,  Spent = 400 },
            new Category { Id = 3, Name = "الفواتير",   ColorValue = Theme.Warning, MonthlyBudget = 400,  Spent = 300 },
            new Category { Id = 4, Name = "التسوق",     ColorValue = Theme.Purple,  MonthlyBudget = 800,  Spent = 450 },
            new Category { Id = 5, Name = "الترفيه",    ColorValue = Theme.Pink,    MonthlyBudget = 500,  Spent = 200 },
            new Category { Id = 6, Name = "أخرى",       ColorValue = Theme.Gray,    MonthlyBudget = 300,  Spent = 250 },
            new Category { Id = 7, Name = "الصحة",       ColorValue = ColorTranslator.FromHtml("#14B8A6"), MonthlyBudget = 450, Spent = 180 },
            new Category { Id = 8, Name = "التعليم",     ColorValue = ColorTranslator.FromHtml("#F97316"), MonthlyBudget = 700, Spent = 520 },
            new Category { Id = 9, Name = "المنزل",      ColorValue = ColorTranslator.FromHtml("#A855F7"), MonthlyBudget = 900, Spent = 610 },
            new Category { Id = 10, Name = "الهدايا",    ColorValue = ColorTranslator.FromHtml("#E11D48"), MonthlyBudget = 350, Spent = 140 },
            new Category { Id = 11, Name = "الاشتراكات", ColorValue = ColorTranslator.FromHtml("#0EA5E9"), MonthlyBudget = 250, Spent = 160 },
            new Category { Id = 12, Name = "العناية الشخصية", ColorValue = ColorTranslator.FromHtml("#84CC16"), MonthlyBudget = 300, Spent = 190 },
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

        // العدادات اللي بتدي رقم جديد وقت الإضافة.
        public static int NextExpenseId = 15;
        public static int NextCategoryId = 13;
        public static int ExpensesVersion;
        public static int CategoriesVersion;

        public static readonly string[] MonthNames =
        {
            "يناير","فبراير","مارس","أبريل","مايو","يونيو",
            "يوليو","أغسطس","سبتمبر","أكتوبر","نوفمبر","ديسمبر"
        };

        // قيم الرسم الشهري، من الأقدم للأحدث.
        public static readonly string[] Last6Labels = { "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر" };
        public static readonly double[] Last6Values = { 1500, 1300, 2600, 2400, 2500, 3100 };

        // بيرجع شرائح توزيع المصاريف حسب التصنيف.
        public static List<ChartSlice> Distribution()
        {
            return Categories
                .Select(category => new ChartSlice(category.Name, (double)category.Spent, category.ColorValue))
                .ToList();
        }

        // بيرجع لون التصنيف، أو الرمادي لو مش موجود.
        public static Color ColorOf(string categoryName)
        {
            foreach (var c in Categories)
                if (c.Name == categoryName) return c.ColorValue;
            return Theme.Gray;
        }

        // بيختصر إنشاء مصروف تجريبي.
        static Expense E(int id, int y, int m, int d, string desc, string cat, decimal amount)
        {
            return new Expense { Id = id, Date = new DateTime(y, m, d), Description = desc, CategoryName = cat, Amount = amount, Notes = "" };
        }
    }
}
