using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>Bar chart drawn by hand with GDI+ (grid lines, light tracks, green bars).</summary>
    public class BarChart : Control
    {
        public string[] Labels { get; set; } = new string[0];
        public double[] Values { get; set; } = new double[0];
        public Color BarColor { get; set; } = Theme.Primary;

        public BarChart()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
            MinimumSize = new Size(340, 140);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (Values == null || Values.Length == 0) return;

            int left = 56, right = 8, top = 10, bottom = 50; // Increased bottom margin for labels
            var plot = new Rectangle(left, top, Width - left - right, Height - top - bottom);
            if (plot.Width < 40 || plot.Height < 40) return;

            double max = Values.Max();
            double niceMax = Math.Max(1000, Math.Ceiling(max / 1000.0) * 1000);
            int intervals = 4;
            double step = niceMax / intervals;

            int n = Values.Length;
            float slot = plot.Width / (float)n;
            float labelFontSize = 8.5f;
            while (labelFontSize > 5.5f && Labels != null && Labels.Length > 0)
            {
                using (var candidate = Theme.F(labelFontSize))
                {
                    float widestLabel = Labels.Max(label => TextRenderer.MeasureText(label, candidate, Size.Empty,
                        TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width);
                    if (widestLabel <= slot - 4)
                        break;
                }
                labelFontSize -= 0.5f;
            }

            using (var f = Theme.F(8.5f))
            using (var labelFont = Theme.F(labelFontSize))
            using (var gridPen = new Pen(Theme.Border))
            {
                gridPen.DashStyle = DashStyle.Dash;
                for (int i = 0; i <= intervals; i++)
                {
                    float y = plot.Bottom - (float)(plot.Height * i / (double)intervals);
                    g.DrawLine(gridPen, plot.Left, y, plot.Right, y);
                    TextRenderer.DrawText(g, (step * i).ToString("N0"), f,
                        new Rectangle(0, (int)y - 9, left - 8, 18), Theme.Muted,
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                }

                float bw = Math.Min(40f, slot * 0.5f);
                for (int i = 0; i < n; i++)
                {
                    float cx = plot.Left + slot * i + slot / 2f;
                    using (var b = new SolidBrush(Theme.Track))
                        g.FillRectangle(b, cx - bw / 2f, plot.Top, bw, plot.Height);
                    float h = (float)(plot.Height * Values[i] / niceMax);
                    using (var b = new SolidBrush(BarColor))
                        g.FillRectangle(b, cx - bw / 2f, plot.Bottom - h, bw, h);
                    if (Labels != null && i < Labels.Length)
                        TextRenderer.DrawText(g, Labels[i], labelFont,
                            new Rectangle((int)(cx - slot / 2f), plot.Bottom + 8, (int)slot, 40), Theme.Muted,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                }
            }
        }
    }
}
