using System;
using System.Drawing;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;

namespace Masareefy.Views
{
    /// <summary>الهوم: ملخص سريع وأربع رسوم على بيانات الديمو.</summary>
    public class DashboardView : BaseView
    {
        // بنجهز العنوان وكروت الملخص والرسوم.
        public DashboardView()
        {
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.Transparent };
            var content = new Panel { BackColor = Color.Transparent };
            scroll.Controls.Add(content);
            Controls.Add(scroll);

            // عنوان الصفحة واسم المستخدم والجرس.
            var right = new Panel { Width = 270, BackColor = Color.Transparent, RightToLeft = RightToLeft.Yes };
            var bell = new BellButton { Count = 3, Location = new Point(0, 4) };
            var chip = new RoundedPanel { Radius = 24, Location = new Point(66, 8), Size = new Size(204, 48) };
            var avatar = new IconBox { Icon = "user", IconColor = Color.White, CircleFill = Theme.Primary, Size = new Size(36, 36), Location = new Point(162, 6) };
            var name = Theme.Lbl(Session.UserName, 11, true, Theme.Ink, Align.Start);
            name.Location = new Point(8, 0); name.Size = new Size(150, 48);
            chip.Controls.Add(avatar); chip.Controls.Add(name);
            right.Controls.Add(bell); right.Controls.Add(chip);
            var header = Theme.Header("الصفحة الرئيسية", right);

            // كروت الملخص الأربعة.
            var stats = new Panel { Height = 180, Padding = new Padding(12, 0, 12, 0), BackColor = Color.Transparent };
            var row = FixedGrid(4, 1, 272, 160);
            row.Dock = DockStyle.Fill;

            var monthlyTotal = Theme.Lbl("EGP 2,450", 17, true, Theme.Ink, Align.End).H(34);
            monthlyTotal.RightToLeft = RightToLeft.No;
            row.Controls.Add(Card(Icon("wallet", Theme.Primary, Theme.Soft),
                Theme.Lbl("إجمالي هذا الشهر", 10, false, Theme.Muted).H(24),
                monthlyTotal,
                Theme.Lbl("12% ▲", 10, true, Theme.Success).H(22),
                Theme.Lbl("مقارنة بالشهر الماضي", 9, false, Theme.Muted).H(20)), 0, 0);

            row.Controls.Add(Card(Icon("pie", Theme.Success, Theme.Soft),
                Theme.Lbl("أكبر تصنيف", 10, false, Theme.Muted).H(26),
                Theme.Lbl("الطعام", 14, true).H(30),
                Theme.Lbl("850 EGP", 13, true).H(26),
                Theme.Lbl("35% من إجمالي المصاريف", 9, false, Theme.Muted).H(20)), 1, 0);

            var ring = new ProgressRing { Percent = 65 };
            row.Controls.Add(Card(ring,
                Theme.Lbl("حالة الميزانية", 10, false, Theme.Muted).H(26),
                Theme.Lbl("1,950 / 3,000", 11, true).H(32),
                Theme.Lbl("تم استخدام 65% من الميزانية", 9, false, Theme.Muted).H(30)), 2, 0);

            row.Controls.Add(Card(Icon("bell", Theme.Warning, Color.FromArgb(255, 243, 220)),
                Theme.Lbl("التنبيهات", 10, false, Theme.Muted).H(26),
                Theme.Lbl("تنبيه ميزانية", 13, true).H(32),
                Theme.Lbl("تصنيفان قريبان من الحد", 9, false, Theme.Muted).H(30)), 3, 0);
            stats.Controls.Add(row);

            var charts = FinanceChartGrid.Create(false);
            header.Dock = DockStyle.Top;
            stats.Dock = DockStyle.Top;
            charts.Dock = DockStyle.Top;
            charts.Height = 740;
            content.Controls.Add(charts);
            content.Controls.Add(stats);
            content.Controls.Add(header);
            content.Size = new Size(scroll.ClientSize.Width, header.Height + stats.Height + charts.Height);

            scroll.Resize += (sender, args) =>
            {
                content.Width = scroll.ClientSize.Width;
                FinanceChartGrid.ResizeContent(charts);
            };
            content.Width = scroll.ClientSize.Width;
            FinanceChartGrid.ResizeContent(charts);
        }

        // بيجهز شبكة بمقاسات ثابتة للكروت.
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
                t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
            for (int i = 0; i < rows; i++)
                t.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            return t;
        }

        // بيجهز أيقونة جوه دايرة لون.
        private static IconBox Icon(string icon, Color color, Color circle)
        {
            return new IconBox { Icon = icon, IconColor = color, CircleFill = circle };
        }

        /// <summary>بيبني كارت فيه أيقونة وسطور نص.</summary>
        private static RoundedPanel Card(Control iconControl, params Label[] lines)
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(12), Margin = new Padding(6) };
            var stack = new Panel { BackColor = Color.Transparent };
            card.Controls.Add(stack); stack.Dock = DockStyle.Fill;
            card.Controls.Add(iconControl); iconControl.Width = iconControl is ProgressRing ? 72 : 54; iconControl.Dock = DockStyle.Right;
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
        /// <summary>بيحدد ارتفاع النص ويرجع الـLabel عشان يتضاف بسهولة.</summary>
        public static Label H(this Label l, int height) { l.Height = height; return l; }
    }
}
