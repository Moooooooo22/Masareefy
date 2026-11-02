using System;
using System.Windows.Forms;
using Masareefy.Helpers;

namespace Masareefy.Controls
{
    /// <summary>زر العين اللي بيظهر أو يخفي كلمة المرور.</summary>
    public class PasswordVisibilityToggle : Button
    {
        // مربع النص اللي الزر بيتحكم فيه.
        private readonly TextBox _passwordBox;

        // بنربط العين بمربع كلمة المرور.
        public PasswordVisibilityToggle(TextBox passwordBox)
        {
            _passwordBox = passwordBox ?? throw new ArgumentNullException(nameof(passwordBox));
            Text = "👁";
            AccessibleName = "إظهار كلمة المرور";
            AccessibleDescription = "إظهار أو إخفاء نص كلمة المرور";
            Width = 40;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Theme.Border;
            BackColor = Theme.Soft;
            ForeColor = Theme.Primary;
            Font = Theme.F(11);
            Cursor = Cursors.Hand;
            RightToLeft = RightToLeft.No;
            TabStop = true;
        }

        // بنبدل الإخفاء والظهور ونرجع المؤشر مكانه.
        protected override void OnClick(EventArgs e)
        {
            int selectionStart = _passwordBox.SelectionStart;
            int selectionLength = _passwordBox.SelectionLength;
            _passwordBox.UseSystemPasswordChar = !_passwordBox.UseSystemPasswordChar;
            AccessibleName = _passwordBox.UseSystemPasswordChar ? "إظهار كلمة المرور" : "إخفاء كلمة المرور";
            _passwordBox.Focus();
            _passwordBox.SelectionStart = Math.Min(selectionStart, _passwordBox.TextLength);
            _passwordBox.SelectionLength = Math.Min(selectionLength, _passwordBox.TextLength - _passwordBox.SelectionStart);
            base.OnClick(e);
        }
    }
}
