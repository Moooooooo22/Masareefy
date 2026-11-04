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
    /// <summary>صفحة المصاريف: بحث وفلاتر وجدول وإضافة وتعديل وحذف.</summary>
    public class ExpensesView : BaseView
    {
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _search = new TextBox();
        private readonly ComboBox _cat = new ComboBox();
        private readonly DateTimePicker _from = new DateTimePicker();
        private readonly DateTimePicker _to = new DateTimePicker();
        private readonly Label _summary;
        private List<Expense> _filtered = new List<Expense>();
        private bool _loading;
        private int _loadedVersion = -1;

        // بنجهز الفلاتر والجدول وأزرار التنقل بين الصفحات.
        public ExpensesView()
        {
            // العنوان وزر إضافة مصروف.
            var add = new RoundedButton { Text = "إضافة مصروف جديد", Size = new Size(210, 44), Location = new Point(0, 8) };
            add.Click += (s, e) => AddExpense();
            var addWrap = new Panel { Width = 210, BackColor = Color.Transparent };
            addWrap.Controls.Add(add);
            var header = Theme.Header("المصاريف", addWrap);

            // الكارت اللي شايل الفلاتر والجدول والملخص.
            var body = new Panel { Padding = new Padding(28, 4, 28, 24), BackColor = Color.Transparent };
            var card = new RoundedPanel { Padding = new Padding(16) };
            body.Controls.Add(card); card.Dock = DockStyle.Fill;

            // فلاتر التاريخ والتصنيف والبحث.
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

            // أسفل الجدول: ملخص النتائج وزر التصدير.
            var footer = new Panel { Height = 58, BackColor = Color.Transparent, Padding = new Padding(0, 10, 0, 0) };
            var export = new RoundedButton { Text = "تصدير إلى CSV", Width = 180 };
            export.Click += (s, e) => MessageBox.Show("تصدير CSV (في الخلفية) هيتعمل في خطوة الـ Threading.", "Masareefy",
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            _summary = Theme.Lbl("", 10, false, Theme.Muted, Align.End);
            footer.Controls.Add(_summary); _summary.Dock = DockStyle.Fill;
            footer.Controls.Add(export); export.Dock = DockStyle.Right;

            BuildGrid();

            card.Controls.Add(_grid); _grid.Dock = DockStyle.Fill;
            card.Controls.Add(footer); footer.Dock = DockStyle.Bottom;
            card.Controls.Add(filters); filters.Dock = DockStyle.Top;

            Theme.DockStack(this, body, header);

            _search.TextChanged += (s, e) => LoadData();
            _cat.SelectedIndexChanged += (s, e) => LoadData();
            _from.ValueChanged += (s, e) => LoadData();
            _to.ValueChanged += (s, e) => LoadData();
            LoadData();
        }

        // بيجهز أعمدة الجدول وأحداث الرسم والضغط.
        private void BuildGrid()
        {
            Theme.StyleGrid(_grid);
            // ترتيب الأعمدة الظاهر للمستخدم.
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

        // بيضيف عمود للجدول.
        private void AddCol(string name, string header, float weight)
        {
            var c = new DataGridViewTextBoxColumn { Name = name, HeaderText = header, FillWeight = weight, SortMode = DataGridViewColumnSortMode.NotSortable };
            _grid.Columns.Add(c);
        }

        // بيرسم خلية التصنيف ونقطة لونه.
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

        // بيعرف الضغط كان على زر التعديل ولا الحذف.
        private string HitAction(DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || _grid.Columns[e.ColumnIndex].Name != "actions") return null;
            var cell = _grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            return ActionCell.HitTest(cell, new Point(cell.X + e.X, cell.Y + e.Y));
        }

        // بيغير شكل المؤشر لما يقف فوق زر إجراء.
        private void Grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            _grid.Cursor = HitAction(e) != null ? Cursors.Hand : Cursors.Default;
        }

        // بينفذ التعديل أو الحذف للصف المختار.
        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string action = HitAction(e);
            if (action == null) return;
            var expense = _grid.Rows[e.RowIndex].Tag as Expense;
            if (expense == null) return;
            if (action == "edit") EditExpense(expense); else DeleteExpense(expense);
        }

        // بيفلتر المصاريف ويعرض الصفحة الحالية.
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

            _grid.Rows.Clear();
            foreach (var x in _filtered)
            {
                int i = _grid.Rows.Add("", x.Amount.ToString("N0"), x.CategoryName, x.Description, x.Date.ToString("dd/MM/yyyy"), x.Id);
                _grid.Rows[i].Tag = x;
            }

            _summary.Text = "عدد النتائج: " + _filtered.Count;
            _loadedVersion = SampleData.ExpensesVersion;
            _loading = false;
        }

        public void RefreshData()
        {
            if (_loadedVersion == SampleData.ExpensesVersion) return;
            LoadData();
        }

        // بيفتح نافذة الإضافة ويضيف المصروف للبيانات التجريبية.
        private void AddExpense()
        {
            using (var dlg = new ExpenseDialog())
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                var r = dlg.Result;
                r.Id = SampleData.NextExpenseId++;
                SampleData.Expenses.Add(r);
                SampleData.ExpensesVersion++;
                LoadData();
            }
        }

        // بيعدل بيانات المصروف الموجود.
        private void EditExpense(Expense x)
        {
            using (var dlg = new ExpenseDialog(x))
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                var r = dlg.Result;
                x.Amount = r.Amount; x.CategoryName = r.CategoryName; x.Date = r.Date;
                x.Description = r.Description; x.Notes = r.Notes;
                SampleData.ExpensesVersion++;
                LoadData();
            }
        }

        // بيطلب تأكيد قبل حذف المصروف.
        private void DeleteExpense(Expense x)
        {
            var answer = MessageBox.Show("متأكد إنك عايز تحذف المصروف ده؟\n" + x.Description, "حذف مصروف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            if (answer != DialogResult.Yes) return;
            SampleData.Expenses.Remove(x);
            SampleData.ExpensesVersion++;
            LoadData();
        }
    }
}
