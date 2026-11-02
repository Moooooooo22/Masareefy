using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Helpers;
using Masareefy.Models;

namespace Masareefy.Controls
{
    /// <summary>The list next to a chart: color dot, name, percent.</summary>
    public class LegendList : Panel
    {
        public LegendList()
        {
            BackColor = Color.Transparent;
        }

        public void SetItems(List<ChartSlice> items)
        {
            Controls.Clear();
            double total = items.Sum(i => i.Value);
            // Dock=Top: the LAST control added is the top one, so we add in reverse
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                var row = new Panel { Height = 30, BackColor = Color.Transparent, AutoSize = false };
                var name = Theme.Lbl(item.Label, 9.5f, false, Theme.Ink, Align.End);
                name.AutoSize = false;
                var dot = new ColorDot { Dot = item.Color, Width = 22, Height = 22 };
                var pct = Theme.Lbl((total <= 0 ? 0 : item.Value / total * 100).ToString("0") + "%", 9.5f, false, Theme.Muted, Align.Start);
                pct.AutoSize = false;
                pct.Width = 52;
                row.Controls.Add(name);  name.Dock = DockStyle.Fill;
                row.Controls.Add(dot);   dot.Dock = DockStyle.Left;
                row.Controls.Add(pct);   pct.Dock = DockStyle.Right;
                row.Dock = DockStyle.Top;
                Controls.Add(row);
            }
        }
    }
}
