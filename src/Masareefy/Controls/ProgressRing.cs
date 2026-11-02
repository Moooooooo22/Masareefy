using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>حلقة بتوضح نسبة استخدام الميزانية.</summary>
    public class ProgressRing : Control
    {
        public int Percent { get; set; } = 0;

        // بنجهز الكنترول عشان يرسم النسبة بنفسه.
        public ProgressRing()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        // بنرسم الحلقة ونكتب النسبة في النص.
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int size = Math.Min(Width, Height) - 14;
            if (size < 20) return;
            var rect = new Rectangle((Width - size) / 2, (Height - size) / 2, size, size);
            using (var track = new Pen(Theme.Track, 10f))
                g.DrawEllipse(track, rect);
            Color c = Percent >= 100 ? Theme.Danger : (Percent >= 80 ? Theme.Warning : Theme.Sidebar);
            using (var arc = new Pen(c, 10f))
            {
                arc.StartCap = LineCap.Round;
                arc.EndCap = LineCap.Round;
                g.DrawArc(arc, rect, -90, 360f * Math.Min(Percent, 100) / 100f);
            }
            using (var f = Theme.F(13f, true))
                TextRenderer.DrawText(g, Percent + "%", f, ClientRectangle, Theme.Ink,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
