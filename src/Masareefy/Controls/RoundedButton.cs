using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>A flat button with rounded corners and a hover color.</summary>
    public class RoundedButton : Button
    {
        public Color Fill { get; set; } = Theme.Primary;
        public Color HoverFill { get; set; } = Theme.PrimaryHover;
        public Color TextColor { get; set; } = Color.White;
        public Color Stroke { get; set; } = Color.Transparent;
        public int Radius { get; set; } = 10;
        private bool _hover;

        public RoundedButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            RightToLeft = RightToLeft.Yes;
            Font = Theme.F(10.5f, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = GfxPath.Round(new Rectangle(0, 0, Width - 1, Height - 1), Radius))
            using (var b = new SolidBrush(_hover ? HoverFill : Fill))
            {
                g.FillPath(b, path);
                if (Stroke.A > 0)
                    using (var p = new Pen(Stroke)) g.DrawPath(p, path);
            }
            // Fix text rendering: remove EndEllipsis and add WordBreak for proper display
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, TextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.RightToLeft | TextFormatFlags.WordBreak);
        }
    }
}
