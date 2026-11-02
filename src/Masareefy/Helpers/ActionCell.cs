using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Masareefy.Helpers
{
    /// <summary>Draws the two small buttons (edit / delete) inside a grid cell and finds which one was clicked.</summary>
    public static class ActionCell
    {
        private const int Size = 34;
        private const int Gap = 8;

        public static Rectangle EditRect(Rectangle cell)
        {
            return new Rectangle(cell.X + cell.Width / 2 - Size - Gap / 2, cell.Y + (cell.Height - Size) / 2, Size, Size);
        }

        public static Rectangle DeleteRect(Rectangle cell)
        {
            return new Rectangle(cell.X + cell.Width / 2 + Gap / 2, cell.Y + (cell.Height - Size) / 2, Size, Size);
        }

        public static void Paint(DataGridViewCellPaintingEventArgs e)
        {
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var er = EditRect(e.CellBounds);
            var dr = DeleteRect(e.CellBounds);
            using (var p = GfxPath.Round(er, 8)) using (var b = new SolidBrush(ColorTranslator.FromHtml("#FFF3DC"))) g.FillPath(b, p);
            IconPainter.Draw(g, "edit", new Rectangle(er.X + 6, er.Y + 6, 22, 22), ColorTranslator.FromHtml("#D9921A"));
            using (var p = GfxPath.Round(dr, 8)) using (var b = new SolidBrush(ColorTranslator.FromHtml("#FDE8E8"))) g.FillPath(b, p);
            IconPainter.Draw(g, "delete", new Rectangle(dr.X + 6, dr.Y + 6, 22, 22), Theme.Danger);
            e.Handled = true;
        }

        /// <summary>Returns "edit", "delete" or null. Point is in grid coordinates.</summary>
        public static string HitTest(Rectangle cell, Point p)
        {
            if (EditRect(cell).Contains(p)) return "edit";
            if (DeleteRect(cell).Contains(p)) return "delete";
            return null;
        }
    }
}
