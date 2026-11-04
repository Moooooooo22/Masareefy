using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Models;

namespace Masareefy.Controls
{
    /// <summary>Donut (ring) chart drawn by hand with GDI+ (FillPie).</summary>
    public class DonutChart : Control
    {
        public List<ChartSlice> Slices { get; set; } = new List<ChartSlice>();
        public float HoleRatio { get; set; } = 0.58f;
        public Color HoleColor { get; set; } = Color.White;

        public DonutChart()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
            MinimumSize = new Size(100, 100); // Ensure minimum visible size
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int size = Math.Min(Width, Height) - 24;
            if (size < 50 || Slices == null || Slices.Count == 0) return; // Increased minimum from 30 to 50
            double total = Slices.Sum(s => s.Value);
            if (total <= 0) return;

            var rect = new Rectangle((Width - size) / 2, (Height - size) / 2, size, size);
            float start = -90f;
            foreach (var s in Slices)
            {
                float sweep = (float)(s.Value / total * 360.0);
                using (var b = new SolidBrush(s.Color)) g.FillPie(b, rect, start, sweep);
                start += sweep;
            }

            // thin white lines between the slices
            var center = new PointF(rect.X + size / 2f, rect.Y + size / 2f);
            start = -90f;
            using (var p = new Pen(HoleColor, 2f))
            {
                foreach (var s in Slices)
                {
                    double rad = start * Math.PI / 180.0;
                    g.DrawLine(p, center, new PointF(center.X + (float)(Math.Cos(rad) * size / 2f),
                                                     center.Y + (float)(Math.Sin(rad) * size / 2f)));
                    start += (float)(s.Value / total * 360.0);
                }
            }

            // the hole in the middle
            int hole = (int)(size * HoleRatio);
            using (var b = new SolidBrush(HoleColor))
                g.FillEllipse(b, rect.X + (size - hole) / 2, rect.Y + (size - hole) / 2, hole, hole);
        }
    }
}
