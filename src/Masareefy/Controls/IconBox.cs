using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>بيحط أيقونة جوه كنترول وممكن يضيف وراها دايرة.</summary>
    public class IconBox : Control
    {
        public string Icon { get; set; }
        public Color IconColor { get; set; } = Theme.Primary;
        public Color CircleFill { get; set; } = Color.Transparent;

        // بنجهز الكنترول للرسم الشفاف.
        public IconBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
        }

        // بنظبط حجم الأيقونة على مقاس الكنترول.
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float s = Math.Min(Width, Height);
            if (CircleFill.A > 0)
                using (var b = new SolidBrush(CircleFill))
                    g.FillEllipse(b, (Width - s) / 2f, (Height - s) / 2f, s, s);
            float iconSize = CircleFill.A > 0 ? s * 0.5f : s * 0.7f;
            float scale = iconSize / 22f;
            g.TranslateTransform((Width - iconSize) / 2f, (Height - iconSize) / 2f);
            g.ScaleTransform(scale, scale);
            IconPainter.Draw(g, Icon, new Rectangle(0, 0, 22, 22), IconColor);
            g.ResetTransform();
        }
    }
}
