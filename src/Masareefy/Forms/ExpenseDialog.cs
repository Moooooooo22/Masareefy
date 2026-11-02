using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;
using Masareefy.Models;

namespace Masareefy.Forms
{
    /// <summary>Small window to add a new expense or edit an existing one.</summary>
    public class ExpenseDialog : Form
    {
        public Expense Result { get; private set; }

        private readonly Expense _editing;
        private readonly ErrorProvider _err = new ErrorProvider();
        private TextBox _txtAmount, _txtDesc, _txtNotes;
        private ComboBox _cmbCategory;
        private DateTimePicker _dtp;

        public ExpenseDialog(Expense existing = null)
        {
            _editing = existing;
            Text = existing == null ? "إضافة مصروف جديد" : "تعديل المصروف";
            ClientSize = new Size(440, 560);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = Theme.F(10);
            _err.RightToLeft = true;
            _err.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            int y = 18;
            _txtAmount = new TextBox();
            AddField("المبلغ (EGP)", _txtAmount, ref y, 28);

            _cmbCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in SampleData.Categories) _cmbCategory.Items.Add(c.Name);
            AddField("التصنيف", _cmbCategory, ref y, 28);

            _dtp = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Value = DateTime.Today };
            AddField("التاريخ", _dtp, ref y, 28);

            _txtDesc = new TextBox();
            AddField("الوصف", _txtDesc, ref y, 28);

            _txtNotes = new TextBox { Multiline = true };
            AddField("ملاحظات (اختياري)", _txtNotes, ref y, 70);

            var save = new RoundedButton { Text = "حفظ", Location = new Point(236, 488), Size = new Size(180, 46) };
            var cancel = new RoundedButton { Text = "إلغاء", Location = new Point(24, 488), Size = new Size(190, 46), Fill = Color.White, HoverFill = Theme.Bg, TextColor = Theme.Ink, Stroke = Theme.Border };
            save.Click += (s, e) => Save();
            cancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(save); Controls.Add(cancel);
            AcceptButton = save; CancelButton = cancel;

            if (existing != null)
            {
                _txtAmount.Text = existing.Amount.ToString();
                _cmbCategory.SelectedItem = existing.CategoryName;
                _dtp.Value = existing.Date;
                _txtDesc.Text = existing.Description;
                _txtNotes.Text = existing.Notes;
            }
            else _cmbCategory.SelectedIndex = 0;
        }

        private void AddField(string label, Control input, ref int y, int height)
        {
            var l = Theme.Lbl(label, 10, true, Theme.Ink, Align.Start);
            l.Location = new Point(24, y); l.Size = new Size(392, 24);
            input.RightToLeft = input is DateTimePicker ? RightToLeft.No : RightToLeft.Yes;
            input.Font = Theme.F(11);
            input.Location = new Point(24, y + 26);
            input.Size = new Size(392, height);
            Controls.Add(l); Controls.Add(input);
            y += 26 + height + 22;
        }

        private void Save()
        {
            _err.Clear();
            bool ok = true;
            decimal amount;
            if (!decimal.TryParse(_txtAmount.Text, out amount) || amount <= 0) { _err.SetError(_txtAmount, "المبلغ لازم يكون رقم أكبر من صفر"); ok = false; }
            if (_cmbCategory.SelectedIndex < 0) { _err.SetError(_cmbCategory, "اختار التصنيف"); ok = false; }
            if (string.IsNullOrWhiteSpace(_txtDesc.Text)) { _err.SetError(_txtDesc, "اكتب وصف للمصروف"); ok = false; }
            if (!ok) return;

            Result = new Expense
            {
                Id = _editing != null ? _editing.Id : 0,
                Amount = amount,
                CategoryName = (string)_cmbCategory.SelectedItem,
                Date = _dtp.Value.Date,
                Description = _txtDesc.Text.Trim(),
                Notes = _txtNotes.Text.Trim()
            };
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
