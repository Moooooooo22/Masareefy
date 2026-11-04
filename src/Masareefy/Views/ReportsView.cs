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

        // بنجهز فلاتر الفترة وشبكة الرسوم.
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
            _period = Theme.Lbl("", 9.5f, false, Theme.Muted, Align.Center);
            _period.Size = new Size(160, 44);
            _period.Margin = new Padding(8, 26, 8, 0);
            flow.Controls.Add(_period);
            var show = new RoundedButton { Text = "عرض التقرير", Size = new Size(170, 44), Margin = new Padding(8, 26, 8, 0) };
            show.Click += (s, e) => UpdatePeriod();
            flow.Controls.Add(show);
            // زر العرض بيتضاف آخر واحد عشان يظهر في الطرف الشمال.

            filterCard.Controls.Add(flow); flow.Dock = DockStyle.Fill;
            filterWrap.Controls.Add(filterCard); filterCard.Dock = DockStyle.Fill;

            Theme.DockStack(this, FinanceChartGrid.Create(), header, filterWrap);
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

    }
}
