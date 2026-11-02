using System;
using System.Drawing;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;

namespace Masareefy.Views
{
    /// <summary>صفحة التقارير وملخصها ورسومها التجريبية.</summary>
    public class ReportsView : BaseView
    {
        private readonly ComboBox _type = new ComboBox();
        private readonly ComboBox _month = new ComboBox();
        private readonly ComboBox _year = new ComboBox();
        private readonly Label _period;

        // بنجهز فلاتر الفترة وبطاقات التقرير.
        public ReportsView()
        {
            var header = Theme.Header("التقارير");

            // فلاتر نوع التقرير والشهر والسنة.
            var filterWrap = new Panel { Height = 104, Padding = new Padding(28, 4, 28, 8), BackColor = Color.Transparent, RightToLeft = RightToLeft.Yes };
            var filterCard = new RoundedPanel { Padding = new Padding(12, 10, 12, 10) };
            var flow = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.Transparent, RightToLeft = RightToLeft.Yes };

            _type.Items.AddRange(new object[] { "المصاريف حسب التصنيف", "المصاريف الشهرية", "الميزانية مقابل المصروف" });
            _type.SelectedIndex = 0;
            for (int i = 0; i < 12; i++) _month.Items.Add(SampleData.MonthNames[i]);
            _month.SelectedIndex = 9;
            _year.Items.AddRange(new object[] { "2025", "2026", "2027" });
            _year.SelectedIndex = 1;

            flow.Controls.Add(Field("نوع التقرير", _type, 230));
            flow.Controls.Add(Field("الشهر", _month, 150));
            flow.Controls.Add(Field("السنة", _year, 110));
            var show = new RoundedButton { Text = "عرض التقرير", Size = new Size(170, 44), Margin = new Padding(8, 26, 8, 0) };
            show.Click += (s, e) => UpdatePeriod();
            flow.Controls.Add(show);
            // زر العرض بيتضاف آخر واحد عشان يظهر في الطرف الشمال.

            filterCard.Controls.Add(flow); flow.Dock = DockStyle.Fill;
            filterWrap.Controls.Add(filterCard); filterCard.Dock = DockStyle.Fill;

            // كروت الملخص والرسم الدائري والرسم العمودي.
            var body = new Panel { Padding = new Padding(20, 0, 20, 20), BackColor = Color.Transparent, AutoScroll = true };
            var row = new TableLayoutPanel { ColumnCount = 3, RowCount = 1, BackColor = Color.Transparent, AutoSize = false };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26f));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37f));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37f));
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            row.MinimumSize = new Size(1050, 400);

            // ملخص ثابت للتقرير التجريبي.
            var sum = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(16), Margin = new Padding(8), MinimumSize = new Size(150, 300) };
            var sumTitle = Theme.Lbl("ملخص التقرير", 12, true, Theme.Ink, Align.Start); sumTitle.Height = 34;
            _period = Theme.Lbl("", 9, false, Theme.Muted, Align.Start); _period.Height = 24;
            var items = new Panel { BackColor = Color.Transparent };
            items.Controls.Add(SummaryRow("pie", "أكبر تصنيف", "الطعام (850 EGP)"));
            items.Controls.Add(SummaryRow("expenses", "عدد المصاريف", "12"));
            items.Controls.Add(SummaryRow("wallet", "إجمالي المصروفات", "2,450 EGP"));
            sum.Controls.Add(items); items.Dock = DockStyle.Fill;
            sum.Controls.Add(_period); _period.Dock = DockStyle.Top;
            sum.Controls.Add(sumTitle); sumTitle.Dock = DockStyle.Top;
            row.Controls.Add(sum, 0, 0);

            // توزيع المصاريف حسب التصنيف.
            var dCard = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(16), Margin = new Padding(8), MinimumSize = new Size(150, 300) };
            var slices = SampleData.Distribution();
            var donut = new DonutChart { Slices = slices, MinimumSize = new Size(100, 200) };
            var legend = new LegendList { Width = 170, Padding = new Padding(0, 12, 0, 0) };
            legend.SetItems(slices);
            var dTitle = Theme.Lbl("المصاريف حسب التصنيف", 12, true, Theme.Ink, Align.Start); dTitle.Height = 34;
            dCard.Controls.Add(dTitle); dTitle.Dock = DockStyle.Top;
            dCard.Controls.Add(legend); legend.Dock = DockStyle.Right;
            dCard.Controls.Add(donut); donut.Dock = DockStyle.Fill;
            row.Controls.Add(dCard, 1, 0);

            // المصاريف على مدار الشهور.
            var bCard = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(16), Margin = new Padding(8), MinimumSize = new Size(150, 300) };
            var bar = new BarChart { Labels = SampleData.Last6Labels, Values = SampleData.Last6Values, MinimumSize = new Size(340, 140) };
            var bTitle = Theme.Lbl("المصاريف الشهرية", 12, true, Theme.Ink, Align.Start); bTitle.Height = 34;
            Theme.DockStack(bCard, bar, bTitle);
            row.Controls.Add(bCard, 2, 0);

            body.Controls.Add(row); row.Dock = DockStyle.Fill;

            Theme.DockStack(this, body, header, filterWrap);
            UpdatePeriod();
        }

        // بيحدث عنوان الفترة المختارة.
        private void UpdatePeriod()
        {
            _period.Text = "شهر " + _month.SelectedItem + " " + _year.SelectedItem;
        }

        // بيجهز عنوان وقائمة اختيار جوه كارت الفلاتر.
        private static Panel Field(string label, ComboBox combo, int width)
        {
            var p = new Panel { Size = new Size(width + 16, 76), Margin = new Padding(4, 0, 4, 0), BackColor = Color.Transparent };
            var l = Theme.Lbl(label, 9.5f, false, Theme.Muted, Align.Start);
            l.Location = new Point(8, 0); l.Size = new Size(width, 24);
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.RightToLeft = RightToLeft.Yes;
            combo.Font = Theme.F(11);
            combo.Location = new Point(8, 28);
            combo.Width = width;
            p.Controls.Add(l); p.Controls.Add(combo);
            return p;
        }

        // بيبني صف في ملخص التقرير.
        private static Panel SummaryRow(string icon, string caption, string value)
        {
            // Dock=Top بيخلي آخر صف مضاف يظهر فوق.
            var row = new Panel { Height = 72, BackColor = Color.Transparent };
            var ic = new IconBox { Icon = icon, IconColor = Theme.Primary, CircleFill = Theme.Soft, Width = 52 };
            var cap = Theme.Lbl(caption, 9.5f, false, Theme.Muted, Align.Start); cap.Height = 26;
            var val = Theme.Lbl(value, 12, true, Theme.Ink, Align.Start);
            var txt = new Panel { BackColor = Color.Transparent };
            txt.Controls.Add(val); val.Dock = DockStyle.Fill;
            txt.Controls.Add(cap); cap.Dock = DockStyle.Top;
            row.Controls.Add(txt); txt.Dock = DockStyle.Fill;
            row.Controls.Add(ic); ic.Dock = DockStyle.Right;
            row.Dock = DockStyle.Top;
            return row;
        }
    }
}
