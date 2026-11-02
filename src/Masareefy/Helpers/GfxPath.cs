using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Masareefy.Helpers
{
    /// <summary>بيجهز مسار مستطيل بحواف دائرية للرسم.</summary>
    public static class GfxPath
    {
        // بيرجع مسار مستطيل عادي أو مدور حسب قيمة radius.
        public static GraphicsPath Round(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            if (radius <= 0) { p.AddRectangle(r); return p; }
            int d = radius * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
