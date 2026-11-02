using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Forms;
using Masareefy.Helpers;
using Masareefy.Models;

namespace Masareefy.Views
{
    /// <summary>صفحة التصنيفات: الاسم واللون والميزانية ونسبة الصرف.</summary>
    public class CategoriesView : BaseView
    {
        // الجدول اللي بيعرض بيانات التصنيفات.
        private readonly DataGridView _grid = new DataGridView();

        // بنجهز الجدول وزر الإضافة وأحداث الخلايا.
        public CategoriesView()
        {
            var add = new RoundedButton { Text = "إضافة تصنيف جديد", Size = new Size(210, 44), Location = new Point(0, 8) };
            add.Click += (s, e) => AddCategory();
            var addWrap = new Panel { Width = 210, BackColor = Color.Transparent };
            addWrap.Controls.Add(add);
            var header = Theme.Header("إدارة التصنيفات", addWrap);

            var body = new Panel { Padding = new Padding(28, 4, 28, 24), BackColor = Color.Transparent };
            var card = new RoundedPanel { Padding = new Padding(16) };
            body.Controls.Add(card); card.Dock = DockStyle.Fill;

            Theme.StyleGrid(_grid);
            AddCol("actions", "إجراءات", 14);
            AddCol("pct", "النسبة", 22);
            AddCol("spent", "المصروف الحالي", 13);
            AddCol("budget", "الميزانية الشهرية (EGP)", 17);
            AddCol("color", "اللون", 9);
            AddCol("name", "اسم التصنيف", 19);
            AddCol("id", "#", 6);
            _grid.CellPainting += Grid_CellPainting;
            _grid.CellMouseClick += Grid_CellMouseClick;
            _grid.CellMouseMove += (s, e) => _grid.Cursor = HitAction(e) != null ? Cursors.Hand : Cursors.Default;
            _grid.CellMouseLeave += (s, e) => _grid.Cursor = Cursors.Default;

            card.Controls.Add(_grid); _grid.Dock = DockStyle.Fill;
            Theme.DockStack(this, body, header);
            LoadData();
        }

        // بيضيف عمود بالعنوان والمساحة المناسبة.
        private void AddCol(string name, string header, float weight)
        {
            var c = new DataGridViewTextBoxColumn { Name = name, HeaderText = header, FillWeight = weight, SortMode = DataGridViewColumnSortMode.NotSortable };
            _grid.Columns.Add(c);
        }

        // بيحمل التصنيفات الحالية في الجدول.
        private void LoadData()
        {
            _grid.Rows.Clear();
            foreach (var c in SampleData.Categories)
            {
                int i = _grid.Rows.Add("", c.Percent, c.Spent.ToString("N0"), c.MonthlyBudget.ToString("N0"), "", c.Name, c.Id);
                _grid.Rows[i].Tag = c;
            }
        }

        public void RefreshData()
        {
            LoadData();
        }

        // بيرسم لون التصنيف وشريط نسبة الميزانية.
        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string col = _grid.Columns[e.ColumnIndex].Name;
            var cat = _grid.Rows[e.RowIndex].Tag as Category;
            if (col == "actions") { ActionCell.Paint(e); return; }

            if (col == "color" && cat != null)
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var b = e.CellBounds;
                using (var br = new SolidBrush(cat.ColorValue))
                    e.Graphics.FillEllipse(br, b.X + (b.Width - 18) / 2, b.Y + (b.Height - 18) / 2, 18, 18);
                e.Handled = true;
            }
            else if (col == "pct" && cat != null)
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var b = e.CellBounds;
                int pct = cat.Percent;
                Color c = pct >= 100 ? Theme.Danger : (pct >= 75 ? Theme.Warning : Theme.Success);
                const int pctWidth = 48;
                const int gap = 12;
                int trackW = Math.Min(112, Math.Max(0, b.Width - pctWidth - gap - 24));
                int groupWidth = trackW + gap + pctWidth;
                int groupX = b.X + (b.Width - groupWidth) / 2;
                TextRenderer.DrawText(g, pct + "%", e.CellStyle.Font,
                    new Rectangle(groupX + trackW + gap, b.Y, pctWidth, b.Height), Theme.Ink,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                if (trackW > 10)
                {
                    var track = new Rectangle(groupX, b.Y + (b.Height - 8) / 2, trackW, 8);
                    using (var p = GfxPath.Round(track, 4)) using (var br = new SolidBrush(Theme.Track)) g.FillPath(br, p);
                    int fillW = Math.Max(8, trackW * Math.Min(pct, 100) / 100);
                    var fill = new Rectangle(track.X, track.Y, fillW, 8);
                    using (var p = GfxPath.Round(fill, 4)) using (var br = new SolidBrush(c)) g.FillPath(br, p);
                }
                e.Handled = true;
            }
        }

        // بيعرف إذا الضغط كان على زر تعديل أو حذف.
        private string HitAction(DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || _grid.Columns[e.ColumnIndex].Name != "actions") return null;
            var cell = _grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            return ActionCell.HitTest(cell, new Point(cell.X + e.X, cell.Y + e.Y));
        }

        // بينفذ الزر اللي اتضغط عليه في الصف.
        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string action = HitAction(e);
            if (action == null) return;
            var cat = _grid.Rows[e.RowIndex].Tag as Category;
            if (cat == null) return;
            if (action == "edit") EditCategory(cat); else DeleteCategory(cat);
        }

        // بيضيف تصنيف جديد للبيانات التجريبية.
        private void AddCategory()
        {
            using (var dlg = new CategoryDialog())
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                var r = dlg.Result;
                r.Id = SampleData.NextCategoryId++;
                SampleData.Categories.Add(r);
                LoadData();
            }
        }

        // بيعدل التصنيف وبيحدّث اسمه في المصاريف المرتبطة.
        private void EditCategory(Category c)
        {
            using (var dlg = new CategoryDialog(c))
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                var r = dlg.Result;
                string oldName = c.Name;
                c.Name = r.Name; c.MonthlyBudget = r.MonthlyBudget; c.ColorValue = r.ColorValue;
                // بنخلي المصاريف تفضل مرتبطة بالتصنيف بعد تغيير اسمه.
                foreach (var x in SampleData.Expenses.Where(x => x.CategoryName == oldName)) x.CategoryName = c.Name;
                LoadData();
            }
        }

        // بيمنع حذف تصنيف مستخدم، وبعدها يطلب تأكيد الحذف.
        private void DeleteCategory(Category c)
        {
            // ماينفعش نحذف تصنيف لسه مربوط بمصاريف.
            if (SampleData.Expenses.Any(x => x.CategoryName == c.Name))
            {
                MessageBox.Show("مينفعش تحذف تصنيف عليه مصاريف.", "Masareefy", MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                return;
            }
            var answer = MessageBox.Show("متأكد إنك عايز تحذف التصنيف ده؟\n" + c.Name, "حذف تصنيف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            if (answer != DialogResult.Yes) return;
            SampleData.Categories.Remove(c);
            LoadData();
        }
    }
}
