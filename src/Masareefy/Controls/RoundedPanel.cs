using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>A white card with rounded corners and a thin border.</summary>
    public class RoundedPanel : Panel
    {
        public int Radius { get; set; } = 16;
        public Color Fill { get; set; } = Color.White;
        public Color Stroke { get; set; } = Theme.Border;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var background = Theme.Backdrop(this);
            using (var brush = new SolidBrush(background))
                g.FillRectangle(brush, ClientRectangle);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = GfxPath.Round(new Rectangle(0, 0, Width - 1, Height - 1), Radius))
            using (var b = new SolidBrush(Fill))
            using (var p = new Pen(Stroke))
            {
                g.FillPath(b, path);
                g.DrawPath(p, path);
            }
        }
    }
}
