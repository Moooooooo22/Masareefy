using System;
using System.Drawing;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;

namespace Masareefy.Forms
{
    /// <summary>شاشة الدخول ومنها بنوصل للتسجيل واستعادة كلمة المرور.</summary>
    public class LoginForm : Form
    {
        // حقول الدخول ورسالة الخطأ.
        private readonly TextBox _userName = new TextBox();
        private readonly TextBox _password = new TextBox();
        private readonly Label _error = new Label();

        // بنجهز شاشة الدخول وأزرار التنقل للشاشات التانية.
        public LoginForm()
        {
            Text = "Masareefy - تسجيل الدخول";
            ClientSize = new Size(480, 480);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            Font = Theme.F(10);
            RightToLeft = RightToLeft.Yes;

            var title = Theme.Lbl("أهلًا بك في مصاريفي", 20, true, Theme.Ink, Align.Center);
            title.SetBounds(40, 35, 400, 55);
            Controls.Add(title);
            var subtitle = Theme.Lbl("سجّل الدخول لمتابعة مصاريفك", 10, false, Theme.Muted, Align.Center);
            subtitle.SetBounds(40, 88, 400, 32);
            Controls.Add(subtitle);

            var userLabel = Theme.Lbl("اسم المستخدم", 10, true);
            userLabel.SetBounds(60, 140, 360, 25);
            Controls.Add(userLabel);
            _userName.SetBounds(60, 168, 360, 34);
            _userName.Font = Theme.F(11);
            _userName.RightToLeft = RightToLeft.Yes;
            Controls.Add(_userName);

            var passwordLabel = Theme.Lbl("كلمة المرور", 10, true);
            passwordLabel.SetBounds(60, 218, 360, 25);
            Controls.Add(passwordLabel);
            _password.SetBounds(60, 246, 320, 34);
            _password.Font = Theme.F(11);
            _password.UseSystemPasswordChar = true;
            _password.RightToLeft = RightToLeft.Yes;
            Controls.Add(_password);
            var revealPassword = new PasswordVisibilityToggle(_password);
            revealPassword.SetBounds(385, 246, 35, 34);
            Controls.Add(revealPassword);

            _error.SetBounds(60, 286, 360, 28);
            _error.ForeColor = Theme.Danger;
            _error.TextAlign = ContentAlignment.MiddleCenter;
            _error.Visible = false;
            Controls.Add(_error);

            var login = new RoundedButton
            {
                Text = "تسجيل الدخول",
                Fill = Color.Transparent,
                HoverFill = Color.Transparent,
                TextColor = Theme.Primary,
                Stroke = Color.Transparent
            };
            login.SetBounds(60, 330, 360, 46);
            login.Click += (sender, args) => Login();
            Controls.Add(login);

            var signUp = new RoundedButton
            {
                Text = "إنشاء حساب",
                Fill = Color.Transparent,
                HoverFill = Color.Transparent,
                TextColor = Theme.Primary,
                Stroke = Color.Transparent
            };
            signUp.SetBounds(60, 390, 170, 42);
            signUp.Click += (sender, args) =>
            {
                using (var form = new SignUpForm())
                    form.ShowDialog(this);
            };
            Controls.Add(signUp);

            var forgotPassword = new RoundedButton
            {
                Text = "نسيت كلمة المرور؟",
                Fill = Color.Transparent,
                HoverFill = Color.Transparent,
                TextColor = Theme.Primary,
                Stroke = Color.Transparent
            };
            forgotPassword.SetBounds(250, 390, 170, 42);
            forgotPassword.Click += (sender, args) =>
            {
                using (var form = new ForgotPasswordForm())
                    form.ShowDialog(this);
            };
            Controls.Add(forgotPassword);
            AcceptButton = login;
        }

        // بيتأكد من الحساب ويحفظ اسمه في الجلسة.
        private void Login()
        {
            var name = _userName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(_password.Text))
            {
                _error.Text = "أدخل اسم المستخدم وكلمة المرور.";
                _error.Visible = true;
                return;
            }

            try
            {
                if (!UserAccountStore.VerifyCredentials(name, _password.Text))
                {
                    _error.Text = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                    _error.Visible = true;
                    return;
                }

                Session.UserName = name;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception)
            {
                _error.Text = "تعذّر الوصول إلى قاعدة الحسابات.";
                _error.Visible = true;
            }
        }
    }
}
