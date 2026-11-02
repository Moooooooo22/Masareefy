using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;
using Masareefy.Models;

namespace Masareefy.Views
{
    /// <summary>Screen 2: the home page (four summary cards and four charts).</summary>
    public class DashboardView : BaseView
    {
        public DashboardView()
        {
            // ----- header: title on the left, bell + user on the right -----
            var right = new Panel { Width = 270, BackColor = Color.Transparent, RightToLeft = RightToLeft.Yes };
            var bell = new BellButton { Count = 3, Location = new Point(0, 4) };
            var chip = new RoundedPanel { Radius = 24, Location = new Point(66, 8), Size = new Size(204, 48) };
            var avatar = new IconBox { Icon = "user", IconColor = Color.White, CircleFill = Theme.Primary, Size = new Size(36, 36), Location = new Point(162, 6) };
            var name = Theme.Lbl(Session.UserName, 11, true, Theme.Ink, Align.Start);
            name.Location = new Point(8, 0); name.Size = new Size(150, 48);
            chip.Controls.Add(avatar); chip.Controls.Add(name);
            right.Controls.Add(bell); right.Controls.Add(chip);
            var header = Theme.Header("الصفحة الرئيسية", right);

            // ----- 4 summary cards -----
            var stats = new Panel { Height = 312, Padding = new Padding(12, 0, 12, 0), BackColor = Color.Transparent, AutoScroll = true };
            var row = FixedGrid(2, 2, 536, 156);

            row.Controls.Add(Card(Icon("wallet", Theme.Primary, Theme.Soft),
                Theme.Lbl("إجمالي هذا الشهر", 10, false, Theme.Muted) .H(26),
                Theme.Lbl("2,450 EGP", 17, true).H(34),
                Theme.Lbl("▲ 12%", 10, true, Theme.Success).H(22),
                Theme.Lbl("مقارنة بالشهر الماضي", 9, false, Theme.Muted).H(20)), 0, 0);

            row.Controls.Add(Card(Icon("pie", Theme.Success, Theme.Soft),
                Theme.Lbl("أكبر تصنيف", 10, false, Theme.Muted).H(26),
                Theme.Lbl("الطعام", 14, true).H(30),
                Theme.Lbl("850 EGP", 13, true).H(26),
                Theme.Lbl("35% من إجمالي المصاريف", 9, false, Theme.Muted).H(20)), 1, 0);

            var ring = new ProgressRing { Percent = 65 };
            row.Controls.Add(Card(ring,
                Theme.Lbl("حالة الميزانية", 10, false, Theme.Muted).H(26),
                Theme.Lbl("1,950 / 3,000 EGP", 12, true).H(32),
                Theme.Lbl("تم استخدام 65% من الميزانية", 9, false, Theme.Muted).H(40)), 0, 1);

            row.Controls.Add(Card(Icon("bell", Theme.Warning, Color.FromArgb(255, 243, 220)),
                Theme.Lbl("التنبيهات", 10, false, Theme.Muted).H(26),
                Theme.Lbl("تنبيه ميزانية", 13, true).H(32),
                Theme.Lbl("تصنيفان قريبان من الحد", 9, false, Theme.Muted).H(40)), 1, 1);
            stats.Controls.Add(row);
            stats.AutoScrollMinSize = new Size(row.Width + stats.Padding.Horizontal, row.Height + stats.Padding.Vertical);
            stats.Resize += (sender, args) =>
            {
                int availableWidth = stats.ClientSize.Width - stats.Padding.Horizontal;
                row.Location = new Point(stats.Padding.Left + Math.Max(0, (availableWidth - row.Width) / 2), stats.Padding.Top);
            };

            // ----- charts in a scrollable 2x2 grid -----
            var charts = new Panel { Padding = new Padding(12, 4, 12, 16), BackColor = Color.Transparent, AutoScroll = true };
            var crow = FixedGrid(2, 2, 536, 340);

            var barCard = new RoundedPanel { Size = new Size(520, 320), Padding = new Padding(16), Margin = new Padding(8), Anchor = AnchorStyles.None };
            var bar = new BarChart { Labels = SampleData.Last6Labels, Values = SampleData.Last6Values };
            var barTitle = Theme.Lbl("المصاريف آخر 6 أشهر", 12, true, Theme.Ink, Align.Start).H(36);
            Theme.DockStack(barCard, bar, barTitle);
            crow.Controls.Add(barCard, 0, 0);

            var donutCard = new RoundedPanel { Size = new Size(520, 320), Padding = new Padding(16), Margin = new Padding(8), Anchor = AnchorStyles.None };
            var slices = SampleData.Distribution();
            var donut = new DonutChart { Slices = slices };
            var legend = new LegendList { Width = 160, Padding = new Padding(0, 8, 0, 0) };
            legend.SetItems(slices);
            var donutTitle = Theme.Lbl("توزيع المصاريف حسب التصنيف", 12, true, Theme.Ink, Align.Start).H(36);
            donutCard.Controls.Add(donutTitle); donutTitle.Dock = DockStyle.Top;     // Top first (full width)
            donutCard.Controls.Add(legend); legend.Dock = DockStyle.Right;           // then Right
            donutCard.Controls.Add(donut); donut.Dock = DockStyle.Fill;              // Fill last
            crow.Controls.Add(donutCard, 1, 0);

            var categoryLabels = SampleData.Categories.Select(category => category.Name).ToArray();
            var categoryValues = SampleData.Categories.Select(category => (double)category.Spent).ToArray();
            var categoryCard = new RoundedPanel { Size = new Size(520, 320), Padding = new Padding(16), Margin = new Padding(8), Anchor = AnchorStyles.None };
            var categoryChart = new BarChart { Labels = categoryLabels, Values = categoryValues, BarColor = Theme.Blue };
            var categoryTitle = Theme.Lbl("المصروف حسب التصنيف", 12, true, Theme.Ink, Align.Start).H(36);
            Theme.DockStack(categoryCard, categoryChart, categoryTitle);
            crow.Controls.Add(categoryCard, 0, 1);

            decimal totalSpent = SampleData.Categories.Sum(category => category.Spent);
            decimal totalBudget = SampleData.Categories.Sum(category => category.MonthlyBudget);
            decimal withinBudget = Math.Min(totalSpent, totalBudget);
            decimal remainingBudget = Math.Max(0, totalBudget - totalSpent);
            decimal overBudget = Math.Max(0, totalSpent - totalBudget);
            var budgetSlices = new System.Collections.Generic.List<ChartSlice>
            {
                new ChartSlice("المصروف", (double)withinBudget, Theme.Primary),
                new ChartSlice("المتبقي", (double)remainingBudget, Theme.Track)
            };
            if (overBudget > 0)
                budgetSlices.Add(new ChartSlice("تجاوز الميزانية", (double)overBudget, Theme.Danger));

            var budgetCard = new RoundedPanel { Size = new Size(520, 320), Padding = new Padding(16), Margin = new Padding(8), Anchor = AnchorStyles.None };
            var budgetChart = new DonutChart { Slices = budgetSlices };
            var budgetLegend = new LegendList { Width = 160, Padding = new Padding(0, 24, 0, 0) };
            budgetLegend.SetItems(budgetSlices);
            var budgetTitle = Theme.Lbl("استخدام الميزانية الشهرية", 12, true, Theme.Ink, Align.Start).H(36);
            budgetCard.Controls.Add(budgetTitle); budgetTitle.Dock = DockStyle.Top;
            budgetCard.Controls.Add(budgetLegend); budgetLegend.Dock = DockStyle.Right;
            budgetCard.Controls.Add(budgetChart); budgetChart.Dock = DockStyle.Fill;
            crow.Controls.Add(budgetCard, 1, 1);

            charts.Controls.Add(crow);
            charts.AutoScrollMinSize = new Size(crow.Width + charts.Padding.Horizontal, crow.Height + charts.Padding.Vertical);
            charts.Resize += (sender, args) =>
            {
                int availableWidth = charts.ClientSize.Width - charts.Padding.Horizontal;
                crow.Location = new Point(charts.Padding.Left + Math.Max(0, (availableWidth - crow.Width) / 2), charts.Padding.Top);
            };

            Theme.DockStack(this, charts, header, stats);
        }

        // ---------- small helpers ----------
        private static TableLayoutPanel FixedGrid(int columns, int rows, int columnWidth, int rowHeight)
        {
            var t = new TableLayoutPanel
            {
                ColumnCount = columns,
                RowCount = rows,
                BackColor = Color.Transparent,
                Size = new Size(columns * columnWidth, rows * rowHeight),
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            for (int i = 0; i < columns; i++)
                t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, columnWidth));
            for (int i = 0; i < rows; i++)
                t.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            return t;
        }

        private static IconBox Icon(string icon, Color color, Color circle)
        {
            return new IconBox { Icon = icon, IconColor = color, CircleFill = circle };
        }

        /// <summary>A white card: icon on the right, text lines (top to bottom) next to it.</summary>
        private static RoundedPanel Card(Control iconControl, params Label[] lines)
        {
            var card = new RoundedPanel { Size = new Size(520, 140), Padding = new Padding(16), Margin = new Padding(8), Anchor = AnchorStyles.None };
            var stack = new Panel { BackColor = Color.Transparent };
            card.Controls.Add(stack); stack.Dock = DockStyle.Fill;
            card.Controls.Add(iconControl); iconControl.Width = 54; iconControl.Dock = DockStyle.Right;
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                lines[i].Dock = DockStyle.Top;
                stack.Controls.Add(lines[i]);
            }
            return card;
        }
    }

    internal static class LabelExt
    {
        /// <summary>Sets the height and returns the label (so we can write it in one line).</summary>
        public static Label H(this Label l, int height) { l.Height = height; return l; }
    }
}
