using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Masareefy.Controls;
using Masareefy.Helpers;
using Masareefy.Views;

namespace Masareefy.Forms
{
    /// <summary>النافذة الأساسية: قائمة جانبية ومحتوى الشاشة المختارة.</summary>
    public class MainForm : Form
    {
        // مساحة الصفحات، وقاموس الشاشات وأزرار القائمة.
        private readonly Panel _content = new Panel();
        private readonly Dictionary<string, UserControl> _views = new Dictionary<string, UserControl>();
        private readonly List<NavButton> _navButtons = new List<NavButton>();

        // بنجهز القائمة ونفتح شاشة الهوم.
        public MainForm()
        {
            Text = "Masareefy - مصاريفي";
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1280, 760);
            MinimumSize = new Size(1024, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            Font = Theme.F(10);
            RightToLeft = RightToLeft.Yes; // الواجهة كلها ماشية من اليمين للشمال.

            BuildSidebar();

            _content.BackColor = Theme.Bg;
            Controls.Add(_content);          // مساحة المحتوى تتمدد قبل القائمة.
            _content.Dock = DockStyle.Fill;
            Controls.Add(_sidebar);
            _sidebar.Dock = DockStyle.Left;

            Navigate("dashboard");
            WindowState = FormWindowState.Maximized;
        }

        private Panel _sidebar;

        private void BuildSidebar()
        {
            _sidebar = new Panel { Width = 232, BackColor = Theme.Sidebar };

            // شعار التطبيق في أعلى القائمة.
            var logo = new IconBox { Icon = "wallet-big", IconColor = Color.White, CircleFill = Theme.SidebarActive, Size = new Size(46, 46), Location = new Point(16, 26) };
            var brand = Theme.Lbl("Masareefy", 15, true, Color.White, Align.End);
            brand.RightToLeft = RightToLeft.No;
            brand.TextAlign = ContentAlignment.MiddleLeft;
            brand.Location = new Point(70, 26);
            brand.Size = new Size(150, 46);
            _sidebar.Controls.Add(logo);
            _sidebar.Controls.Add(brand);

            AddNav("dashboard", "الصفحة الرئيسية", "home", 0);
            AddNav("expenses", "المصاريف", "expenses", 1);
            AddNav("categories", "التصنيفات", "categories", 2);
            AddNav("reports", "التقارير", "reports", 3);
            AddNav("server", "السيرفر", "server", 4);
        }

        // بيضيف زر للشاشة المطلوبة.
        private void AddNav(string key, string text, string icon, int index)
        {
            var b = new NavButton
            {
                Text = text,
                Icon = icon,
                Tag = key,
                Size = new Size(204, 46),
                Location = new Point(14, 110 + index * 54),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            b.Click += (s, e) => Navigate((string)((Control)s).Tag);
            _navButtons.Add(b);
            _sidebar.Controls.Add(b);
        }

        /// <summary>بيبدل الشاشة اللي ظاهرة في مساحة المحتوى.</summary>
        public void Navigate(string key)
        {
            UserControl view;
            if (!_views.TryGetValue(key, out view))
            {
                switch (key)
                {
                    case "dashboard": view = new DashboardView(); break;
                    case "expenses":   view = new ExpensesView(); break;
                    case "categories": view = new CategoriesView(); break;
                    case "reports":    view = new ReportsView(); break;
                    case "server":     view = new ServerView(); break;
                    default:            return;
                }
                _views[key] = view;
            }

            foreach (UserControl cachedView in _views.Values)
                cachedView.Visible = cachedView == view;

            if (view.Parent != _content)
            {
                view.Dock = DockStyle.Fill;
                _content.Controls.Add(view);
            }

            if (view is ExpensesView expensesView)
                expensesView.RefreshData();
            else if (view is CategoriesView categoriesView)
                categoriesView.RefreshData();

            foreach (var b in _navButtons)
            {
                b.Selected = (string)b.Tag == key;
                b.Invalidate();
            }
        }
    }
}
