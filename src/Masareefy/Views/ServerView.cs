using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;

namespace Masareefy.Views
{
    /// <summary>
    /// Screen 6: server control. For now it is only a design (fake data + a timer for the uptime).
    /// LATER (networking step) the Start/Stop button will start a real TCP server.
    /// </summary>
    public class ServerView : BaseView
    {
        private readonly Timer _timer = new Timer();
        private readonly Label _status, _statusValue, _uptimeValue;
        private readonly RoundedButton _toggle;
        private readonly ColorDot _dot = new ColorDot();
        private readonly DataGridView _grid = new DataGridView();
        private bool _running = true;
        private TimeSpan _uptime = new TimeSpan(2, 15, 30);

        public ServerView()
        {
            // ----- header: title on the left, status + stop button on the right -----
            var right = new Panel { Width = 430, BackColor = Color.Transparent, RightToLeft = RightToLeft.No };
            _toggle = new RoundedButton { Text = "إيقاف السيرفر", Size = new Size(176, 44), Location = new Point(238, 8), Fill = Theme.Danger, HoverFill = Color.FromArgb(190, 50, 50) };
            _toggle.Click += (s, e) => Toggle();
            _status = Theme.Lbl("السيرفر يعمل", 11, true, Theme.Success, Align.Start);
            _status.Location = new Point(0, 8); _status.Size = new Size(174, 44);
            _dot.Dot = Theme.Success; _dot.Location = new Point(184, 8); _dot.Size = new Size(28, 44);
            right.Controls.Add(_toggle); right.Controls.Add(_status); right.Controls.Add(_dot);
            var header = Theme.Header("إدارة السيرفر", right);

            // ----- 3 cards (left to right: uptime, devices, server status) -----
            var cards = new Panel { Height = 198, Padding = new Padding(20, 0, 20, 0), BackColor = Color.Transparent };
            var row = new TableLayoutPanel { ColumnCount = 3, RowCount = 1, BackColor = Color.Transparent };
            for (int i = 0; i < 3; i++) row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _uptimeValue = LtrLabel(Format(_uptime), 18, true); _uptimeValue.Height = 40;
            row.Controls.Add(Card("clock", Theme.Primary,
                Theme.Lbl("وقت التشغيل", 10, false, Theme.Muted).Tall(26), _uptimeValue,
                Theme.Lbl("منذ", 9, false, Theme.Muted).Tall(20),
                LtrLabel("16/10/2026 10:30", 9).Tall(22)), 0, 0);

            row.Controls.Add(Card("people", Theme.Primary,
                Theme.Lbl("الأجهزة المتصلة", 10, false, Theme.Muted).Tall(26),
                LtrLabel("3", 20, true).Tall(40),
                Theme.Lbl("عرض الأجهزة", 9.5f, true, Theme.Primary).Tall(26)), 1, 0);

            _statusValue = Theme.Lbl("يعمل", 16, true, Theme.Success); _statusValue.Height = 36;
            row.Controls.Add(Card("wifi", Theme.Primary,
                Theme.Lbl("حالة السيرفر", 10, false, Theme.Muted).Tall(24), _statusValue,
                Theme.Lbl("العنوان", 9, false, Theme.Muted).Tall(18),
                LtrLabel("192.168.1.10", 9).Tall(20),
                Theme.Lbl("المنفذ", 9, false, Theme.Muted).Tall(18),
                LtrLabel("5050", 9).Tall(20)), 2, 0);
            cards.Controls.Add(row); row.Dock = DockStyle.Fill;

            // ----- connected devices table -----
            var body = new Panel { Padding = new Padding(28, 4, 28, 24), BackColor = Color.Transparent };
            body.AutoScroll = true;
            var card = new RoundedPanel { Padding = new Padding(16) };
            body.Controls.Add(card); card.Dock = DockStyle.Fill;
            Theme.StyleGrid(_grid);
            _grid.ColumnHeadersHeight = 50;
            _grid.ColumnHeadersDefaultCellStyle.Font = Theme.F(10.5f, true);
            AddCol("status", "الحالة", 20);
            AddCol("time", "وقت الاتصال", 18);
            AddCol("ip", "IP", 20);
            AddCol("device", "اسم الجهاز", 30);
            AddCol("id", "#", 12);
            _grid.Rows.Add("متصل الآن", "10:32", "192.168.1.12", "Mohamed-PC", 1);
            _grid.Rows.Add("متصل الآن", "10:45", "192.168.1.15", "Malak-PC", 2);
            _grid.Rows.Add("متصل الآن", "11:10", "192.168.1.18", "Jana-PC", 3);
            _grid.CellPainting += Grid_CellPainting;
            card.Controls.Add(_grid); _grid.Dock = DockStyle.Fill;

            Theme.DockStack(this, body, header, cards);

            _timer.Interval = 1000;
            _timer.Tick += (s, e) => { if (_running) { _uptime = _uptime.Add(TimeSpan.FromSeconds(1)); _uptimeValue.Text = Format(_uptime); } };
            _timer.Start();
        }

        private static string Format(TimeSpan t)
        {
            return ((int)t.TotalHours).ToString("00") + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
        }

        private void AddCol(string name, string header, float weight)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = name, HeaderText = header, FillWeight = weight, SortMode = DataGridViewColumnSortMode.NotSortable });
        }

        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string column = _grid.Columns[e.ColumnIndex].Name;
            if (column != "status" && column != "time" && column != "ip" && column != "device" && column != "id") return;
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
            if (column != "status")
            {
                TextRenderer.DrawText(e.Graphics, Convert.ToString(e.Value), e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                e.Handled = true;
                return;
            }
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var b = e.CellBounds;
            Color c = _running ? Theme.Success : Theme.Gray;
            string text = _running ? Convert.ToString(e.Value) : "غير متصل";
            int textWidth = Math.Min(b.Width - 24, TextRenderer.MeasureText(text, e.CellStyle.Font, new Size(b.Width, b.Height), TextFormatFlags.NoPadding).Width);
            int groupWidth = textWidth + 18;
            int groupX = b.X + (b.Width - groupWidth) / 2;
            TextRenderer.DrawText(e.Graphics, text, e.CellStyle.Font, new Rectangle(groupX, b.Y, textWidth, b.Height), Theme.Ink,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.NoPadding);
            using (var br = new SolidBrush(c)) e.Graphics.FillEllipse(br, groupX + textWidth + 8, b.Y + (b.Height - 10) / 2, 10, 10);
            e.Handled = true;
        }

        /// <summary>Fake start/stop (just changes the look). The real server comes in the networking step.</summary>
        private void Toggle()
        {
            _running = !_running;
            _toggle.Text = _running ? "إيقاف السيرفر" : "تشغيل السيرفر";
            _toggle.Fill = _running ? Theme.Danger : Theme.Primary;
            _toggle.HoverFill = _running ? Color.FromArgb(190, 50, 50) : Theme.PrimaryHover;
            _status.Text = _running ? "السيرفر يعمل" : "السيرفر متوقف";
            _status.ForeColor = _running ? Theme.Success : Theme.Muted;
            _dot.Dot = _running ? Theme.Success : Theme.Gray; _dot.Invalidate();
            _statusValue.Text = _running ? "يعمل" : "متوقف";
            _statusValue.ForeColor = _running ? Theme.Success : Theme.Muted;
            if (_running) _uptime = TimeSpan.Zero;
            _uptimeValue.Text = Format(_uptime);
            _grid.Invalidate();
        }

        private static RoundedPanel Card(string icon, Color color, params Label[] lines)
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(16), Margin = new Padding(8) };
            var stack = new Panel { BackColor = Color.Transparent };
            var ic = new IconBox { Icon = icon, IconColor = color, CircleFill = Theme.Soft, Width = 64 };
            card.Controls.Add(stack); stack.Dock = DockStyle.Fill;
            card.Controls.Add(ic); ic.Dock = DockStyle.Right;
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                lines[i].Dock = DockStyle.Top;
                stack.Controls.Add(lines[i]);
            }
            return card;
        }

        private static Label LtrLabel(string text, float size, bool bold = false)
        {
            var label = Theme.Lbl(text, size, bold, Theme.Ink, Align.End);
            label.RightToLeft = RightToLeft.No;
            label.TextAlign = ContentAlignment.MiddleRight;
            return label;
        }
    }

    internal static class LabelTall
    {
        public static Label Tall(this Label l, int height) { l.Height = height; return l; }
    }
}
