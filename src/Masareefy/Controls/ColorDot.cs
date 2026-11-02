using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Masareefy.Controls
{
    /// <summary>A small colored circle (used in legends).</summary>
    public class ColorDot : Control
    {
        public Color Dot { get; set; } = Color.Green;

        public ColorDot()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Size = new Size(26, 26); // Set default size
            MinimumSize = new Size(14, 14); // Ensure minimum visible size
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int s = Math.Min(14, Math.Min(Width, Height));
            using (var b = new SolidBrush(Dot))
                g.FillEllipse(b, (Width - s) / 2, (Height - s) / 2, s, s);
        }
    }
}
