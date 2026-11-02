using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Views
{
    public class BaseView : UserControl
    {
        private const int TileSpacing = 170;
        private Bitmap _backgroundCache;

        public BaseView()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            BackColor = Theme.Bg;
            Dock = DockStyle.Fill;
            RightToLeft = RightToLeft.Yes;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RebuildBackground();
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0)
                return;
            if (_backgroundCache == null || _backgroundCache.Width != Width || _backgroundCache.Height != Height)
                RebuildBackground();
            if (_backgroundCache != null)
                e.Graphics.DrawImageUnscaled(_backgroundCache, 0, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_backgroundCache != null)
                {
                    _backgroundCache.Dispose();
                    _backgroundCache = null;
                }
            }
            base.Dispose(disposing);
        }

        private void RebuildBackground()
        {
            if (Width <= 0 || Height <= 0)
                return;
            if (_backgroundCache != null)
                _backgroundCache.Dispose();
            _backgroundCache = new Bitmap(Width, Height);
            using (var graphics = Graphics.FromImage(_backgroundCache))
            {
                graphics.Clear(Theme.Bg);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                var pattern = Color.FromArgb(12, Theme.Primary);
                var random = new Random(Width * 397 ^ Height * 7919);
                int row = 0;
                for (int y = 55; y < Height; y += TileSpacing, row++)
                {
                    int offset = row % 2 == 0 ? 0 : TileSpacing / 2;
                    for (int x = 70 - offset; x < Width; x += TileSpacing)
                        DrawPatternIcon(graphics, (int)random.Next(5), x, y, random.Next(-9, 10), pattern);
                }
                DrawWatermark(graphics);
            }
        }

        private static void DrawPatternIcon(Graphics graphics, int type, float centerX, float centerY, float angle, Color color)
        {
            var state = graphics.Save();
            graphics.TranslateTransform(centerX, centerY);
            graphics.RotateTransform(angle);
            using (var pen = new Pen(color, 2f))
            using (var brush = new SolidBrush(color))
            {
                pen.LineJoin = LineJoin.Round;
                switch (type)
                {
                    case 0:
                        graphics.DrawEllipse(pen, -18, -18, 36, 36);
                        graphics.DrawEllipse(pen, -11, -11, 22, 22);
                        DrawCurrency(graphics, new RectangleF(-14, -8, 28, 16), color, 7f);
                        break;
                    case 1:
                        using (var path = GfxPath.Round(new Rectangle(-30, -19, 60, 38), 6)) graphics.DrawPath(pen, path);
                        graphics.DrawEllipse(pen, -7, -7, 14, 14);
                        graphics.DrawLine(pen, -25, -12, -18, -12);
                        graphics.DrawLine(pen, 18, 12, 25, 12);
                        break;
                    case 2:
                        graphics.DrawEllipse(pen, -20, -18, 36, 18);
                        graphics.DrawArc(pen, -20, -9, 36, 18, 0, 180);
                        graphics.DrawEllipse(pen, -20, -2, 36, 18);
                        graphics.DrawArc(pen, -20, 7, 36, 18, 0, 180);
                        break;
                    case 3:
                        using (var path = GfxPath.Round(new Rectangle(-29, -18, 58, 36), 7)) graphics.DrawPath(pen, path);
                        graphics.DrawLine(pen, -23, -8, 17, -8);
                        graphics.DrawEllipse(pen, 13, -3, 8, 8);
                        break;
                    default:
                        DrawCurrency(graphics, new RectangleF(-32, -15, 64, 30), color, 15f);
                        break;
                }
            }
            graphics.Restore(state);
        }

        private static void DrawCurrency(Graphics graphics, RectangleF bounds, Color color, float size)
        {
            using (var font = Theme.F(size, true))
            using (var brush = new SolidBrush(color))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.DirectionRightToLeft })
                graphics.DrawString("ج.م", font, brush, bounds, format);
        }

        private void DrawWatermark(Graphics graphics)
        {
            float x = Width - 282;
            float y = Height - 195;
            var fill = Color.FromArgb(5, Theme.Primary);
            var line = Color.FromArgb(15, Theme.Primary);
            using (var pen = new Pen(line, 5f))
            using (var brush = new SolidBrush(fill))
            {
                pen.LineJoin = LineJoin.Round;
                using (var wallet = GfxPath.Round(new Rectangle((int)x, (int)y + 36, 238, 128), 26))
                {
                    graphics.FillPath(brush, wallet);
                    graphics.DrawPath(pen, wallet);
                }
                graphics.DrawLine(pen, x + 24, y + 62, x + 165, y + 36);
                using (var flap = GfxPath.Round(new Rectangle((int)x + 132, (int)y + 78, 114, 55), 14))
                {
                    graphics.FillPath(brush, flap);
                    graphics.DrawPath(pen, flap);
                }
                graphics.DrawEllipse(pen, x + 201, y + 96, 13, 13);
                DrawLeaf(graphics, x + 73, y + 8, line, fill, true);
                DrawLeaf(graphics, x + 113, y + 2, line, fill, false);
            }
        }

        private static void DrawLeaf(Graphics graphics, float x, float y, Color line, Color fill, bool left)
        {
            using (var path = new GraphicsPath())
            using (var pen = new Pen(line, 4f))
            using (var brush = new SolidBrush(fill))
            {
                if (left)
                {
                    path.AddBezier(x + 24, y + 32, x + 3, y + 26, x + 2, y + 5, x + 5, y);
                    path.AddBezier(x + 5, y, x + 27, y + 2, x + 38, y + 13, x + 24, y + 32);
                }
                else
                {
                    path.AddBezier(x + 2, y + 28, x + 5, y + 8, x + 24, y + 1, x + 35, y + 4);
                    path.AddBezier(x + 35, y + 4, x + 33, y + 24, x + 19, y + 34, x + 2, y + 28);
                }
                graphics.FillPath(brush, path);
                graphics.DrawPath(pen, path);
            }
        }
    }
}
