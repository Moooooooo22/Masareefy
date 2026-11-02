using System;
using System.Drawing;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;

namespace Masareefy.Forms
{
    /// <summary>شاشة تغيير كلمة المرور باستخدام كود الاستعادة.</summary>
    public class ForgotPasswordForm : Form
    {
        // بيانات الحساب والكود وكلمة المرور الجديدة.
        private readonly TextBox _userName = new TextBox();
        private readonly TextBox _recoveryCode = new TextBox();
        private readonly TextBox _newPassword = new TextBox();
        private readonly TextBox _confirmPassword = new TextBox();
        private readonly Label _error = new Label();

        // بنجهز حقول الاستعادة والتأكيد.
        public ForgotPasswordForm()
        {
            Text = "استعادة كلمة المرور";
            ClientSize = new Size(480, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Bg;
            Font = Theme.F(10);
            RightToLeft = RightToLeft.Yes;

            var title = Theme.Lbl("استعادة كلمة المرور", 20, true, Theme.Ink, Align.Center);
            title.SetBounds(40, 25, 400, 50);
            Controls.Add(title);

            AddField("اسم المستخدم", _userName, 95, false);
            AddField("كود الاستعادة", _recoveryCode, 180, false);
            AddField("كلمة المرور الجديدة", _newPassword, 265, true);
            AddField("تأكيد كلمة المرور الجديدة", _confirmPassword, 350, true);

            _error.SetBounds(50, 435, 380, 32);
            _error.ForeColor = Theme.Danger;
            _error.TextAlign = ContentAlignment.MiddleCenter;
            _error.Visible = false;
            Controls.Add(_error);

            var reset = new RoundedButton { Text = "تغيير كلمة المرور" };
            reset.SetBounds(50, 485, 380, 46);
            reset.Click += (sender, args) => ResetPassword();
            Controls.Add(reset);
            AcceptButton = reset;
        }

        // بيضيف حقل، ومعاه زر عين لو الحقل كلمة مرور.
        private void AddField(string labelText, TextBox input, int top, bool password)
        {
            var label = Theme.Lbl(labelText, 10, true);
            label.SetBounds(50, top, 380, 24);
            Controls.Add(label);
            input.SetBounds(50, top + 28, password ? 330 : 380, 34);
            input.Font = Theme.F(11);
            input.RightToLeft = password ? RightToLeft.Yes : RightToLeft.No;
            input.UseSystemPasswordChar = password;
            Controls.Add(input);
            if (password)
            {
                var revealPassword = new PasswordVisibilityToggle(input);
                revealPassword.SetBounds(390, top + 28, 40, 34);
                Controls.Add(revealPassword);
            }
        }

        // بيتأكد من تطابق التأكيد وبعدين يطلب تغيير كلمة المرور.
        private void ResetPassword()
        {
            if (_newPassword.Text != _confirmPassword.Text)
            {
                ShowError("كلمتا المرور غير متطابقتين.");
                return;
            }

            try
            {
                string error;
                if (!UserAccountStore.ResetPassword(_userName.Text, _recoveryCode.Text, _newPassword.Text, out error))
                {
                    ShowError(error);
                    return;
                }

                MessageBox.Show(this, "تم تغيير كلمة المرور. يمكنك تسجيل الدخول الآن.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception)
            {
                ShowError("تعذّر تحديث كلمة المرور في قاعدة البيانات.");
            }
        }

        // بيعرض رسالة الخطأ تحت الحقول.
        private void ShowError(string message)
        {
            _error.Text = message;
            _error.Visible = true;
        }
    }
}
