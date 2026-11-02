using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;
using Masareefy.Models;

namespace Masareefy.Forms
{
    /// <summary>Small window to add or edit a category (name, color, monthly budget).</summary>
    public class CategoryDialog : Form
    {
        public Category Result { get; private set; }

        private readonly Category _editing;
        private readonly ErrorProvider _err = new ErrorProvider();
        private TextBox _txtName, _txtBudget;
        private Panel _colorPreview;
        private Color _color = Theme.Success;

        public CategoryDialog(Category existing = null)
        {
            _editing = existing;
            Text = existing == null ? "إضافة تصنيف جديد" : "تعديل التصنيف";
            ClientSize = new Size(440, 400);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = Theme.F(10);
            _err.RightToLeft = true;
            _err.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            var l1 = Theme.Lbl("اسم التصنيف", 10, true, Theme.Ink, Align.Start); l1.Location = new Point(24, 18); l1.Size = new Size(392, 24);
            _txtName = new TextBox { RightToLeft = RightToLeft.Yes, Font = Theme.F(11), Location = new Point(24, 44), Size = new Size(392, 28) };

            var l2 = Theme.Lbl("الميزانية الشهرية (EGP)", 10, true, Theme.Ink, Align.Start); l2.Location = new Point(24, 92); l2.Size = new Size(392, 24);
            _txtBudget = new TextBox { RightToLeft = RightToLeft.Yes, Font = Theme.F(11), Location = new Point(24, 118), Size = new Size(392, 28) };

            var l3 = Theme.Lbl("اللون", 10, true, Theme.Ink, Align.Start); l3.Location = new Point(24, 166); l3.Size = new Size(392, 24);
            _colorPreview = new Panel { Location = new Point(372, 194), Size = new Size(44, 34), BackColor = _color, BorderStyle = BorderStyle.FixedSingle };
            var pick = new RoundedButton { Text = "اختيار لون", Location = new Point(24, 192), Size = new Size(160, 38), Fill = Color.White, HoverFill = Theme.Bg, TextColor = Theme.Ink, Stroke = Theme.Border };
            pick.Click += (s, e) =>
            {
                using (var cd = new ColorDialog { Color = _color, FullOpen = true })
                    if (cd.ShowDialog(this) == DialogResult.OK) { _color = cd.Color; _colorPreview.BackColor = _color; }
            };

            var save = new RoundedButton { Text = "حفظ", Location = new Point(236, 330), Size = new Size(180, 46) };
            var cancel = new RoundedButton { Text = "إلغاء", Location = new Point(24, 330), Size = new Size(190, 46), Fill = Color.White, HoverFill = Theme.Bg, TextColor = Theme.Ink, Stroke = Theme.Border };
            save.Click += (s, e) => Save();
            cancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { l1, _txtName, l2, _txtBudget, l3, _colorPreview, pick, save, cancel });
            AcceptButton = save; CancelButton = cancel;

            if (existing != null)
            {
                _txtName.Text = existing.Name;
                _txtBudget.Text = existing.MonthlyBudget.ToString();
                _color = existing.ColorValue;
                _colorPreview.BackColor = _color;
            }
        }

        private void Save()
        {
            _err.Clear();
            bool ok = true;
            string name = _txtName.Text.Trim();
            decimal budget;
            if (name.Length == 0) { _err.SetError(_txtName, "اكتب اسم التصنيف"); ok = false; }
            else if (SampleData.Categories.Any(c => c.Name == name && (_editing == null || c.Id != _editing.Id)))
            { _err.SetError(_txtName, "الاسم ده موجود قبل كده"); ok = false; }
            if (!decimal.TryParse(_txtBudget.Text, out budget) || budget < 0) { _err.SetError(_txtBudget, "الميزانية لازم تكون رقم (صفر أو أكتر)"); ok = false; }
            if (!ok) return;

            Result = new Category
            {
                Id = _editing != null ? _editing.Id : 0,
                Name = name,
                MonthlyBudget = budget,
                ColorValue = _color,
                Spent = _editing != null ? _editing.Spent : 0
            };
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
