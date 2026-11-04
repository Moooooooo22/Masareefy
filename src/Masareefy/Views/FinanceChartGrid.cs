using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;
using Masareefy.Models;

namespace Masareefy.Views
{
    internal static class FinanceChartGrid
    {
        public static Panel Create(bool scrollable = true)
        {
            var charts = new Panel
            {
                Padding = new Padding(12, 4, 12, 16),
                BackColor = Color.Transparent,
                AutoScroll = scrollable
            };
            var grid = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Size = new Size(1088, 720),
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
                Anchor = AnchorStyles.Top,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 360));
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 360));

            var monthlyCard = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(16), Margin = new Padding(8) };
            var monthlyChart = new BarChart { Labels = SampleData.Last6Labels, Values = SampleData.Last6Values };
            var monthlyTitle = Theme.Lbl("المصاريف آخر 6 أشهر", 12, true, Theme.Ink, Align.Start);
            monthlyTitle.Height = 36;
            Theme.DockStack(monthlyCard, monthlyChart, monthlyTitle);
            grid.Controls.Add(monthlyCard, 0, 0);

            var slices = SampleData.Distribution();
            var categoryCard = DonutCard("توزيع المصاريف حسب التصنيف", slices);
            grid.Controls.Add(categoryCard, 1, 0);

            var categoryLabels = SampleData.Categories.Select(category => category.Name).ToArray();
            var categoryValues = SampleData.Categories.Select(category => (double)category.Spent).ToArray();
            var spendingCard = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(16), Margin = new Padding(8) };
            var spendingChart = new BarChart { Labels = categoryLabels, Values = categoryValues, BarColor = Theme.Blue };
            var spendingTitle = Theme.Lbl("المصروف حسب التصنيف", 12, true, Theme.Ink, Align.Start).H(36);
            Theme.DockStack(spendingCard, spendingChart, spendingTitle);
            grid.Controls.Add(spendingCard, 0, 1);

            decimal spent = SampleData.Categories.Sum(category => category.Spent);
            decimal budget = SampleData.Categories.Sum(category => category.MonthlyBudget);
            decimal withinBudget = Math.Min(spent, budget);
            decimal remainingBudget = Math.Max(0, budget - spent);
            decimal overBudget = Math.Max(0, spent - budget);
            var budgetSlices = new System.Collections.Generic.List<ChartSlice>
            {
                new ChartSlice("المصروف", (double)withinBudget, Theme.Primary),
                new ChartSlice("المتبقي", (double)remainingBudget, Theme.Track)
            };
            if (overBudget > 0)
                budgetSlices.Add(new ChartSlice("تجاوز الميزانية", (double)overBudget, Theme.Danger));
            grid.Controls.Add(DonutCard("استخدام الميزانية الشهرية", budgetSlices), 1, 1);

            charts.Controls.Add(grid);
            charts.Resize += (sender, args) => ResizeContent(charts);
            ResizeContent(charts);
            return charts;
        }

        private static RoundedPanel DonutCard(string title, System.Collections.Generic.List<ChartSlice> slices)
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(16), Margin = new Padding(8) };
            var donut = new DonutChart { Slices = slices, MinimumSize = new Size(120, 120), Dock = DockStyle.Fill };
            var legend = new LegendList { Width = 150, Padding = new Padding(0, 8, 0, 0), Dock = DockStyle.Right, AutoScroll = true };
            legend.SetItems(slices);
            var heading = Theme.Lbl(title, 12, true, Theme.Ink, Align.Start);
            heading.Height = 36;
            var content = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            content.Controls.Add(donut);
            content.Controls.Add(legend);
            card.Controls.Add(content);
            card.Controls.Add(heading);
            heading.Dock = DockStyle.Top;
            return card;
        }

        public static void ResizeContent(Panel charts)
        {
            if (charts.Controls.Count == 0 || !(charts.Controls[0] is TableLayoutPanel grid)) return;
            int availableWidth = charts.ClientSize.Width - charts.Padding.Horizontal;
            if (availableWidth > 0) grid.Width = availableWidth;
            grid.Location = new Point(charts.Padding.Left, charts.Padding.Top);
        }
    }

}
