using System;
using System.Collections.Generic;
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
    /// <summary>Screen 3: expenses table with search, filters, paging, add / edit / delete.</summary>
    public class ExpensesView : BaseView
    {
        private const int PageSize = 5;

        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _search = new TextBox();
        private readonly ComboBox _cat = new ComboBox();
        private readonly DateTimePicker _from = new DateTimePicker();
        private readonly DateTimePicker _to = new DateTimePicker();
        private readonly Label _summary;
        private readonly Panel _pager = new Panel();
        private List<Expense> _filtered = new List<Expense>();
        private int _page = 1;
        private bool _loading;

        public ExpensesView()
        {
            // ----- header -----
            var add = new RoundedButton { Text = "إضافة مصروف جديد", Size = new Size(210, 44), Location = new Point(0, 8) };
            add.Click += (s, e) => AddExpense();
            var addWrap = new Panel { Width = 210, BackColor = Color.Transparent };
            addWrap.Controls.Add(add);
            var header = Theme.Header("المصاريف", addWrap);

            // ----- card with filters, table, footer -----
            var body = new Panel { Padding = new Padding(28, 4, 28, 24), BackColor = Color.Transparent };
            var card = new RoundedPanel { Padding = new Padding(16) };
            body.Controls.Add(card); card.Dock = DockStyle.Fill;

            // filters (visual order left to right: from, to, category, search)
            var filters = new Panel { Height = 62, BackColor = Color.Transparent };
            var flow = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.Transparent };
            _search.PlaceholderText = "بحث في الوصف...";
            _search.RightToLeft = RightToLeft.Yes;
            _search.Font = Theme.F(11); _search.Width = 280; _search.Margin = new Padding(6, 12, 6, 0);
            _cat.DropDownStyle = ComboBoxStyle.DropDownList;
            _cat.RightToLeft = RightToLeft.Yes; _cat.Font = Theme.F(11); _cat.Width = 170; _cat.Margin = new Padding(6, 12, 6, 0);
            _cat.Items.Add("كل التصنيفات");
            foreach (var c in SampleData.Categories) _cat.Items.Add(c.Name);
            _cat.SelectedIndex = 0;
            foreach (var dtp in new[] { _from, _to })
            {
                dtp.Format = DateTimePickerFormat.Custom; dtp.CustomFormat = "dd/MM/yyyy";
                dtp.Font = Theme.F(11); dtp.Width = 140; dtp.Margin = new Padding(6, 12, 6, 0);
            }
            _from.Value = new DateTime(2026, 1, 1);
            _to.Value = new DateTime(2026, 12, 31);
            var lblFrom = Theme.Lbl("من", 10, false, Theme.Muted, Align.Center); lblFrom.Size = new Size(34, 36); lblFrom.Margin = new Padding(6, 10, 0, 0);
            var lblTo = Theme.Lbl("إلى", 10, false, Theme.Muted, Align.Center); lblTo.Size = new Size(34, 36); lblTo.Margin = new Padding(6, 10, 0, 0);
            flow.Controls.Add(_search); flow.Controls.Add(_cat);
            flow.Controls.Add(lblFrom); flow.Controls.Add(_from);
            flow.Controls.Add(lblTo); flow.Controls.Add(_to);
            filters.Controls.Add(flow); flow.Dock = DockStyle.Fill;

            // footer: pager (left), summary (middle), export button (right)
            var footer = new Panel { Height = 58, BackColor = Color.Transparent, Padding = new Padding(0, 10, 0, 0) };
            var export = new RoundedButton { Text = "تصدير إلى CSV", Width = 180 };
            export.Click += (s, e) => MessageBox.Show("تصدير CSV (في الخلفية) هيتعمل في خطوة الـ Threading.", "Masareefy",
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            _summary = Theme.Lbl("", 10, false, Theme.Muted, Align.End);
            _pager.Width = 230; _pager.BackColor = Color.Transparent;
            footer.Controls.Add(_summary); _summary.Dock = DockStyle.Fill;
            footer.Controls.Add(export); export.Dock = DockStyle.Right;
            footer.Controls.Add(_pager); _pager.Dock = DockStyle.Left;

            BuildGrid();

            card.Controls.Add(_grid); _grid.Dock = DockStyle.Fill;
            card.Controls.Add(footer); footer.Dock = DockStyle.Bottom;
            card.Controls.Add(filters); filters.Dock = DockStyle.Top;

            Theme.DockStack(this, body, header);

            _search.TextChanged += (s, e) => { _page = 1; LoadData(); };
            _cat.SelectedIndexChanged += (s, e) => { _page = 1; LoadData(); };
            _from.ValueChanged += (s, e) => { _page = 1; LoadData(); };
            _to.ValueChanged += (s, e) => { _page = 1; LoadData(); };
            LoadData();
        }

        // ---------------------------------------------------------------- grid
        private void BuildGrid()
        {
            Theme.StyleGrid(_grid);
            // columns in the same order as the design (left to right)
            AddCol("actions", "إجراءات", 14);
            AddCol("amount", "المبلغ (EGP)", 14);
            AddCol("category", "التصنيف", 20);
            AddCol("desc", "الوصف", 30);
            AddCol("date", "التاريخ", 14);
            AddCol("id", "#", 6);
            _grid.CellPainting += Grid_CellPainting;
            _grid.CellMouseClick += Grid_CellMouseClick;
            _grid.CellMouseMove += Grid_CellMouseMove;
            _grid.CellMouseLeave += (s, e) => _grid.Cursor = Cursors.Default;
        }

        private void AddCol(string name, string header, float weight)
        {
            var c = new DataGridViewTextBoxColumn { Name = name, HeaderText = header, FillWeight = weight, SortMode = DataGridViewColumnSortMode.NotSortable };
            _grid.Columns.Add(c);
        }

        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string col = _grid.Columns[e.ColumnIndex].Name;
            if (col == "actions") { ActionCell.Paint(e); return; }
            if (col == "category")
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                string name = Convert.ToString(e.Value);
                var b = e.CellBounds;
                int dotSize = 12;
                int gap = 8;
                int maxTextWidth = Math.Max(0, b.Width - dotSize - gap - 12);
                int textWidth = Math.Min(maxTextWidth, TextRenderer.MeasureText(name, e.CellStyle.Font, new Size(maxTextWidth, b.Height),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width);
                int groupWidth = textWidth + gap + dotSize;
                int groupX = b.X + (b.Width - groupWidth) / 2;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var br = new SolidBrush(SampleData.ColorOf(name)))
                    e.Graphics.FillEllipse(br, groupX + textWidth + gap, b.Y + (b.Height - dotSize) / 2, dotSize, dotSize);
                TextRenderer.DrawText(e.Graphics, name, e.CellStyle.Font, new Rectangle(groupX, b.Y, textWidth, b.Height), Theme.Ink,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.NoPadding);
                e.Handled = true;
            }
        }

        private string HitAction(DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || _grid.Columns[e.ColumnIndex].Name != "actions") return null;
            var cell = _grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            return ActionCell.HitTest(cell, new Point(cell.X + e.X, cell.Y + e.Y));
        }

        private void Grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            _grid.Cursor = HitAction(e) != null ? Cursors.Hand : Cursors.Default;
        }

        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string action = HitAction(e);
            if (action == null) return;
            var expense = _grid.Rows[e.RowIndex].Tag as Expense;
            if (expense == null) return;
            if (action == "edit") EditExpense(expense); else DeleteExpense(expense);
        }

        // ---------------------------------------------------------------- data
        private void LoadData()
        {
            if (_loading) return;
            _loading = true;

            _filtered = SampleData.Expenses
                .Where(x => x.Date.Date >= _from.Value.Date && x.Date.Date <= _to.Value.Date)
                .Where(x => _cat.SelectedIndex <= 0 || x.CategoryName == (string)_cat.SelectedItem)
                .Where(x => string.IsNullOrWhiteSpace(_search.Text) || x.Description.Contains(_search.Text.Trim()))
                .OrderByDescending(x => x.Date).ThenByDescending(x => x.Id)
                .ToList();

            int pages = Math.Max(1, (int)Math.Ceiling(_filtered.Count / (double)PageSize));
            if (_page > pages) _page = pages;

            _grid.Rows.Clear();
            foreach (var x in _filtered.Skip((_page - 1) * PageSize).Take(PageSize))
            {
                int i = _grid.Rows.Add("", x.Amount.ToString("N0"), x.CategoryName, x.Description, x.Date.ToString("dd/MM/yyyy"), x.Id);
                _grid.Rows[i].Tag = x;
            }

            int first = _filtered.Count == 0 ? 0 : (_page - 1) * PageSize + 1;
            int last = Math.Min(_page * PageSize, _filtered.Count);
            _summary.Text = "عرض " + first + "-" + last + " من " + _filtered.Count + " نتيجة";
            BuildPager(pages);
            _loading = false;
        }

        private void BuildPager(int pages)
        {
            _pager.Controls.Clear();
            int x = 0;
            _pager.Controls.Add(PageButton("‹", x, _page > 1, () => { _page--; LoadData(); })); x += 40;
            for (int p = 1; p <= pages; p++)
            {
                int page = p;
                _pager.Controls.Add(PageButton(p.ToString(), x, true, () => { _page = page; LoadData(); }, p == _page)); x += 40;
            }
            _pager.Controls.Add(PageButton("›", x, _page < pages, () => { _page++; LoadData(); }));
        }

        private RoundedButton PageButton(string text, int x, bool enabled, Action click, bool active = false)
        {
            var b = new RoundedButton
            {
                Text = text, Location = new Point(x, 4), Size = new Size(34, 34), Radius = 8,
                Font = Theme.F(10, true), Enabled = enabled,
                Fill = active ? Theme.Primary : Color.White,
                HoverFill = active ? Theme.PrimaryHover : Theme.Bg,
                TextColor = active ? Color.White : (enabled ? Theme.Ink : Theme.Gray),
                Stroke = active ? Color.Transparent : Theme.Border
            };
            b.Click += (s, e) => click();
            return b;
        }

        // ---------------------------------------------------------------- actions
        private void AddExpense()
        {
            using (var dlg = new ExpenseDialog())
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                var r = dlg.Result;
                r.Id = SampleData.NextExpenseId++;
                SampleData.Expenses.Add(r);
                _page = 1;
                LoadData();
            }
        }

        private void EditExpense(Expense x)
        {
            using (var dlg = new ExpenseDialog(x))
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                var r = dlg.Result;
                x.Amount = r.Amount; x.CategoryName = r.CategoryName; x.Date = r.Date;
                x.Description = r.Description; x.Notes = r.Notes;
                LoadData();
            }
        }

        private void DeleteExpense(Expense x)
        {
            var answer = MessageBox.Show("متأكد إنك عايز تحذف المصروف ده؟\n" + x.Description, "حذف مصروف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            if (answer != DialogResult.Yes) return;
            SampleData.Expenses.Remove(x);
            LoadData();
        }
    }
}
