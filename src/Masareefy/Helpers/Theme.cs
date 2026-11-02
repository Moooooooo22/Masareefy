using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Masareefy.Helpers
{
    /// <summary>Where the text sits. Start = right side (Arabic), End = left side.</summary>
    public enum Align { Start, Center, End }

    /// <summary>All colors, fonts and small helpers so every screen looks the same.</summary>
    public static class Theme
    {
        // ---- Colors (from the design) ----
        public static readonly Color Sidebar = ColorTranslator.FromHtml("#0B4F2F");
        public static readonly Color SidebarActive = ColorTranslator.FromHtml("#1C7A49");
        public static readonly Color SidebarHover = ColorTranslator.FromHtml("#146B3A");
        public static readonly Color Primary = ColorTranslator.FromHtml("#146B3A");
        public static readonly Color PrimaryHover = ColorTranslator.FromHtml("#0F5A30");
        public static readonly Color Bg = ColorTranslator.FromHtml("#EEF5F0");
        public static readonly Color Border = ColorTranslator.FromHtml("#DCE6DF");
        public static readonly Color Soft = ColorTranslator.FromHtml("#E3F1E8");
        public static readonly Color Track = ColorTranslator.FromHtml("#E6EEE8");
        public static readonly Color Ink = ColorTranslator.FromHtml("#1F2A24");
        public static readonly Color Muted = ColorTranslator.FromHtml("#6B7B72");
        public static readonly Color Danger = ColorTranslator.FromHtml("#D64545");
        public static readonly Color Warning = ColorTranslator.FromHtml("#F2A93B");
        public static readonly Color Success = ColorTranslator.FromHtml("#22A559");
        public static readonly Color Blue = ColorTranslator.FromHtml("#3B82F6");
        public static readonly Color Purple = ColorTranslator.FromHtml("#8B5CF6");
        public static readonly Color Pink = ColorTranslator.FromHtml("#EC4899");
        public static readonly Color Gray = ColorTranslator.FromHtml("#9CA3AF");

        public const string FontName = "Segoe UI";

        public static Font F(float size, bool bold = false)
        {
            return new Font(FontName, size, bold ? FontStyle.Bold : FontStyle.Regular);
        }

        /// <summary>
        /// Arabic label. With RightToLeft = Yes:
        /// - Align.Start shows text on the RIGHT (المحاذاة الطبيعية للعربية)
        /// - Align.End shows text on the LEFT
        /// - Align.Center centers the text
        /// </summary>
        public static Label Lbl(string text, float size = 10f, bool bold = false, Color? color = null, Align align = Align.Start)
        {
            var l = new Label();
            l.Text = text;
            l.Font = F(size, bold);
            l.ForeColor = color ?? Ink;
            l.BackColor = Color.Transparent;
            l.RightToLeft = RightToLeft.Yes;
            l.AutoSize = false;
            // With RTL: Start = Right, End = Left, Center = Center
            if (align == Align.Center) l.TextAlign = ContentAlignment.MiddleCenter;
            else if (align == Align.End) l.TextAlign = ContentAlignment.MiddleLeft;  // End = Left with RTL
            else l.TextAlign = ContentAlignment.MiddleRight;  // Start = Right with RTL
            return l;
        }

        /// <summary>
        /// Docking rule in WinForms: add the Fill control FIRST, then the Top controls
        /// from bottom to top (the last one added ends up at the very top).
        /// </summary>
        public static void DockStack(Control parent, Control fill, params Control[] topToBottom)
        {
            parent.Controls.Add(fill);
            fill.Dock = DockStyle.Fill;
            for (int i = topToBottom.Length - 1; i >= 0; i--)
            {
                topToBottom[i].Dock = DockStyle.Top;
                parent.Controls.Add(topToBottom[i]);
            }
        }

        /// <summary>The page title bar (like the design): title on the LEFT, optional control on the RIGHT.</summary>
        public static Panel Header(string title, Control rightSide = null)
        {
            var h = new Panel();
            h.Height = 80;
            h.Padding = new Padding(28, 14, 28, 6);
            h.BackColor = Color.Transparent;
            var t = Lbl(title, 20, true, Ink, Align.End);
            h.Controls.Add(t);
            t.Dock = DockStyle.Fill;
            if (rightSide != null)
            {
                h.Controls.Add(rightSide);
                rightSide.Dock = DockStyle.Right;
            }
            return h;
        }

        /// <summary>Same look for every table.</summary>
        public static void StyleGrid(DataGridView g)
        {
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Color.White;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F3F8F5");
            g.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
            g.ColumnHeadersDefaultCellStyle.Font = F(10.5f, true);
            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F3F8F5");
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 50;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.AllowUserToResizeColumns = false;
            g.ReadOnly = true;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.RowTemplate.Height = 56;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = Border;
            g.DefaultCellStyle.Font = F(11);
            g.DefaultCellStyle.ForeColor = Ink;
            g.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EAF5EE");
            g.DefaultCellStyle.SelectionForeColor = Ink;
            g.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // smoother painting (property is protected, so we use reflection)
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, g, new object[] { true });
        }

        /// <summary>First parent that has a solid color (used to paint rounded corners).</summary>
        public static Color Backdrop(Control c)
        {
            Control p = c.Parent;
            while (p != null)
            {
                if (p.BackColor.A == 255) return p.BackColor;
                p = p.Parent;
            }
            return Bg;
        }
    }
}
