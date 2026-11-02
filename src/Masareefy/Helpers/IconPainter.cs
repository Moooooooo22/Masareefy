using System.Drawing;
using System.Drawing.Drawing2D;

namespace Masareefy.Helpers
{
    /// <summary>
    /// Draws the small icons with GDI+ (no image files needed).
    /// Every icon is drawn inside a 22 x 22 box.
    /// </summary>
    public static class IconPainter
    {
        public static void Draw(Graphics g, string key, Rectangle r, Color c)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(c, 2f))
            using (var br = new SolidBrush(c))
            {
                pen.LineJoin = LineJoin.Round;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int x = r.X, y = r.Y;
                switch (key)
                {
                    case "home":
                        g.DrawLines(pen, new[] { new Point(x + 1, y + 11), new Point(x + 11, y + 2), new Point(x + 21, y + 11) });
                        g.DrawRectangle(pen, x + 4, y + 10, 14, 10);
                        g.FillRectangle(br, x + 9, y + 14, 4, 6);
                        break;
                    case "expenses":
                        g.DrawRectangle(pen, x + 4, y + 1, 14, 20);
                        g.DrawLine(pen, x + 7, y + 7, x + 15, y + 7);
                        g.DrawLine(pen, x + 7, y + 11, x + 15, y + 11);
                        g.DrawLine(pen, x + 7, y + 15, x + 12, y + 15);
                        break;
                    case "categories":
                        g.FillRectangle(br, x + 2, y + 2, 8, 8);
                        g.FillRectangle(br, x + 12, y + 2, 8, 8);
                        g.FillRectangle(br, x + 2, y + 12, 8, 8);
                        g.FillRectangle(br, x + 12, y + 12, 8, 8);
                        break;
                    case "reports":
                        g.FillRectangle(br, x + 3, y + 11, 4, 10);
                        g.FillRectangle(br, x + 9, y + 4, 4, 17);
                        g.FillRectangle(br, x + 15, y + 8, 4, 13);
                        break;
                    case "server":
                        g.DrawRectangle(pen, x + 2, y + 3, 18, 7);
                        g.DrawRectangle(pen, x + 2, y + 12, 18, 7);
                        g.FillEllipse(br, x + 5, y + 5, 3, 3);
                        g.FillEllipse(br, x + 5, y + 14, 3, 3);
                        break;
                    case "wallet":
                        g.DrawRectangle(pen, x + 1, y + 5, 20, 14);
                        g.DrawLine(pen, x + 3, y + 5, x + 15, y + 2);
                        g.FillEllipse(br, x + 15, y + 10, 4, 4);
                        break;
                    case "pie":
                        g.DrawEllipse(pen, x + 2, y + 2, 18, 18);
                        g.FillPie(br, x + 2, y + 2, 18, 18, -90, 110);
                        break;
                    case "bell":
                        g.DrawArc(pen, x + 5, y + 2, 12, 12, 180, 180);
                        g.DrawLine(pen, x + 5, y + 8, x + 3, y + 16);
                        g.DrawLine(pen, x + 17, y + 8, x + 19, y + 16);
                        g.DrawLine(pen, x + 3, y + 16, x + 19, y + 16);
                        g.FillEllipse(br, x + 9, y + 17, 4, 4);
                        break;
                    case "wifi":
                        g.DrawArc(pen, x + 1, y + 3, 20, 20, 225, 90);
                        g.DrawArc(pen, x + 5, y + 8, 12, 12, 225, 90);
                        g.FillEllipse(br, x + 9, y + 15, 4, 4);
                        break;
                    case "people":
                        g.FillEllipse(br, x + 7, y + 2, 8, 8);
                        g.FillPie(br, x + 2, y + 12, 18, 18, 180, 180);
                        break;
                    case "clock":
                        g.DrawEllipse(pen, x + 2, y + 2, 18, 18);
                        g.DrawLines(pen, new[] { new Point(x + 11, y + 6), new Point(x + 11, y + 11), new Point(x + 15, y + 13) });
                        break;
                    case "user":
                        g.FillEllipse(br, x + 6, y + 2, 10, 10);
                        g.FillPie(br, x + 2, y + 13, 18, 18, 180, 180);
                        break;
                    case "edit":
                        g.DrawLine(pen, x + 4, y + 18, x + 16, y + 6);
                        g.DrawLine(pen, x + 4, y + 18, x + 4, y + 14);
                        g.DrawLine(pen, x + 4, y + 18, x + 8, y + 18);
                        break;
                    case "delete":
                        g.DrawRectangle(pen, x + 5, y + 8, 12, 11);
                        g.DrawLine(pen, x + 3, y + 6, x + 19, y + 6);
                        g.DrawLine(pen, x + 9, y + 3, x + 13, y + 3);
                        break;
                    case "wallet-big":
                        g.DrawRectangle(pen, x + 1, y + 5, 20, 14);
                        g.DrawLine(pen, x + 3, y + 5, x + 15, y + 2);
                        g.FillEllipse(br, x + 15, y + 10, 4, 4);
                        g.DrawLine(pen, x + 5, y + 12, x + 11, y + 12);
                        break;
                }
            }
        }
    }
}
