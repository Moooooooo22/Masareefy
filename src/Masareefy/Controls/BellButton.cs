using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>The notification bell with a red number badge.</summary>
    public class BellButton : Control
    {
        public int Count { get; set; } = 0;

        public BellButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Size = new Size(52, 52);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var body = new Rectangle(2, 6, 44, 44);
            using (var b = new SolidBrush(Color.White)) g.FillEllipse(b, body);
            using (var p = new Pen(Theme.Border)) g.DrawEllipse(p, body);
            IconPainter.Draw(g, "bell", new Rectangle(body.X + 11, body.Y + 11, 22, 22), Theme.Primary);
            if (Count > 0)
            {
                var badge = new Rectangle(Width - 22, 0, 20, 20);
                using (var b = new SolidBrush(Theme.Danger)) g.FillEllipse(b, badge);
                TextRenderer.DrawText(g, Count.ToString(), Theme.F(8.5f, true), badge, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }
    }
}
