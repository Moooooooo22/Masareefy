using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Controls;

namespace Masareefy.Helpers
{
    /// <summary>
    /// Simple logo background for views - draws a faint wallet icon
    /// </summary>
    public static class LogoHelper
    {
        public static void AddLogo(Control view)
        {
            view.Paint += (s, e) =>
            {
                DrawLogo(e.Graphics, view.Width, view.Height);
            };
        }

        private static void DrawLogo(Graphics g, int width, int height)
        {
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw wallet icon in the bottom-right corner (very transparent)
                int iconSize = 200;
                int x = width - iconSize - 20;
                int y = height - iconSize - 20;

                // Semi-transparent (alpha ~5%)
                Color logoColor = Color.FromArgb(13, Theme.Primary);

                using (var brush = new SolidBrush(logoColor))
                using (var pen = new Pen(logoColor, 2f))
                {
                    // Draw simple wallet shape
                    int innerX = x + 20;
                    int innerY = y + 40;
                    int cardW = 160;
                    int cardH = 100;

                    // Main wallet body
                    g.FillRectangle(brush, innerX, innerY, cardW, cardH);
                    g.DrawRectangle(pen, innerX, innerY, cardW, cardH);

                    // Card slot
                    g.DrawRectangle(pen, innerX + 10, innerY + 60, 140, 25);
                }
            }
            catch
            {
                // If logo fails, continue anyway
            }
        }
    }
}
