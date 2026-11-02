using System;
using System.Drawing;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;

namespace Masareefy.Forms
{
    public class SignUpForm : Form
    {
        private readonly TextBox _userName = new TextBox();
        private readonly TextBox _password = new TextBox();
        private readonly TextBox _confirmPassword = new TextBox();
        private readonly Label _error = new Label();

        public SignUpForm()
        {
            Text = "إنشاء حساب جديد";
            ClientSize = new Size(480, 540);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Bg;
            Font = Theme.F(10);
            RightToLeft = RightToLeft.Yes;

            var title = Theme.Lbl("إنشاء حساب", 20, true, Theme.Ink, Align.Center);
            title.SetBounds(40, 28, 400, 50);
            Controls.Add(title);

            AddField("اسم المستخدم", _userName, 100, false);
            AddField("كلمة المرور (8 أحرف على الأقل)", _password, 190, true);
            AddField("تأكيد كلمة المرور", _confirmPassword, 280, true);

            _error.SetBounds(50, 365, 380, 32);
            _error.ForeColor = Theme.Danger;
            _error.TextAlign = ContentAlignment.MiddleCenter;
            _error.Visible = false;
            Controls.Add(_error);

            var create = new RoundedButton { Text = "إنشاء الحساب" };
            create.SetBounds(50, 415, 380, 46);
            create.Click += (sender, args) => CreateAccount();
            Controls.Add(create);
            AcceptButton = create;
        }

        private void AddField(string labelText, TextBox input, int top, bool password)
        {
            var label = Theme.Lbl(labelText, 10, true);
            label.SetBounds(50, top, 380, 24);
            Controls.Add(label);
            input.SetBounds(50, top + 28, password ? 330 : 380, 34);
            input.Font = Theme.F(11);
            input.RightToLeft = RightToLeft.Yes;
            input.UseSystemPasswordChar = password;
            Controls.Add(input);
            if (password)
            {
                var revealPassword = new PasswordVisibilityToggle(input);
                revealPassword.SetBounds(390, top + 28, 40, 34);
                Controls.Add(revealPassword);
            }
        }

        private void CreateAccount()
        {
            if (_password.Text != _confirmPassword.Text)
            {
                ShowError("كلمتا المرور غير متطابقتين.");
                return;
            }

            try
            {
                string code;
                string error;
                if (!UserAccountStore.Register(_userName.Text, _password.Text, out code, out error))
                {
                    ShowError(error);
                    return;
                }

                ShowRecoveryCode(code);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception)
            {
                ShowError("تعذّر إنشاء الحساب في قاعدة البيانات.");
            }
        }

        private void ShowRecoveryCode(string code)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "كود استعادة الحساب";
                dialog.ClientSize = new Size(440, 220);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.BackColor = Theme.Bg;
                dialog.RightToLeft = RightToLeft.Yes;

                var message = Theme.Lbl("احتفظ بهذا الكود؛ سيُطلب عند نسيان كلمة المرور.", 10, true, Theme.Ink, Align.Center);
                message.SetBounds(20, 20, 400, 42);
                dialog.Controls.Add(message);

                var codeBox = new TextBox { Text = code, ReadOnly = true, TextAlign = HorizontalAlignment.Center, Font = Theme.F(14, true) };
                codeBox.SetBounds(30, 75, 380, 38);
                codeBox.RightToLeft = RightToLeft.No;
                dialog.Controls.Add(codeBox);

                var copy = new RoundedButton { Text = "نسخ الكود", Fill = Theme.Soft, HoverFill = Theme.Border, TextColor = Theme.Primary };
                copy.SetBounds(30, 135, 175, 42);
                copy.Click += (sender, args) =>
                {
                    try
                    {
                        Clipboard.SetText(code);
                        MessageBox.Show(dialog, "تم نسخ كود الاستعادة.", dialog.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(dialog, "تعذر النسخ. احتفظ بالكود الظاهر.", dialog.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
                dialog.Controls.Add(copy);

                var done = new RoundedButton { Text = "حفظت الكود" };
                done.SetBounds(235, 135, 175, 42);
                done.Click += (sender, args) => { dialog.DialogResult = DialogResult.OK; dialog.Close(); };
                dialog.Controls.Add(done);
                dialog.AcceptButton = done;
                dialog.ShowDialog(this);
            }
        }

        private void ShowError(string message)
        {
            _error.Text = message;
            _error.Visible = true;
        }
    }
}
