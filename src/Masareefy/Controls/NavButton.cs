using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>A button in the sidebar: icon on the left, Arabic text next to it.</summary>
    public class NavButton : Button
    {
        public string Icon { get; set; }
        public bool Selected { get; set; }
        private bool _hover;

        public NavButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            RightToLeft = RightToLeft.Yes;
            Font = Theme.F(11f);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.Clear(Theme.Sidebar);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (Selected || _hover)
            {
                using (var path = GfxPath.Round(new Rectangle(0, 0, Width - 1, Height - 1), 10))
                using (var b = new SolidBrush(Selected ? Theme.SidebarActive : Theme.SidebarHover))
                    g.FillPath(b, path);
            }
            IconPainter.Draw(g, Icon, new Rectangle(16, (Height - 22) / 2, 22, 22), Color.White);
            using (var f = Theme.F(11f, Selected))
                TextRenderer.DrawText(g, Text, f, new Rectangle(50, 0, Width - 56, Height), Color.White,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.WordBreak);
        }
    }
}
