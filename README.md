# Masareefy - مصاريفي

برنامج تتبّع المصاريف (CS312 – أدوات التطوير الحديثة، جامعة مصر للعلوم والتكنولوجيا).
**C# + Windows Forms (.NET 8)**

## تشغيل البرنامج
1. نزّل **Visual Studio 2022 Community**، واختار workload اسمه **.NET desktop development**.
2. افتح الملف `Masareefy.sln`.
3. اضغط **F5**.
4. أنشئ حسابًا باسم مستخدم وكلمة مرور (8 أحرف على الأقل)، واحفظ كود الاستعادة الذي يظهر مرة واحدة.
5. سجّل الدخول ببيانات الحساب. تُحفظ الحسابات محليًا في SQLite تحت مجلد بيانات المستخدم.

## وصلنا لحد فين
- [x] الـ GUI للـ 6 شاشات بداتا تجريبية (`Helpers/SampleData.cs`)
- [x] قاعدة بيانات SQLite للحسابات (Users)
- [x] إنشاء الحسابات وتسجيل الدخول بكلمات مرور مملّحة ومشفّرة، واستعادة الحساب بكود الاستعادة
- [ ] قاعدة البيانات (SQLite): Categories وExpenses
- [ ] المصاريف والتصنيفات (CRUD) من قاعدة البيانات
- [ ] التحقق من البيانات + ملف Log للأخطاء
- [ ] التقارير من قاعدة البيانات
- [ ] تصدير CSV في الخلفية (async)
- [ ] السيرفر والكلاينتس (Networking)
- [ ] الاختبار + الدوكيومنتيشن + العرض

## الشاشات
| الشاشة | الملف |
|---|---|
| تسجيل الدخول / إنشاء حساب | `Forms/LoginForm.cs` |
| النافذة الرئيسية + القايمة الجانبية | `Forms/MainForm.cs` |
| الصفحة الرئيسية (Dashboard) | `Views/DashboardView.cs` |
| المصاريف | `Views/ExpensesView.cs` (+ `Forms/ExpenseDialog.cs`) |
| التصنيفات | `Views/CategoriesView.cs` (+ `Forms/CategoryDialog.cs`) |
| التقارير | `Views/ReportsView.cs` |
| السيرفر | `Views/ServerView.cs` |

## شكل الفولدرات
```
src/Masareefy/
  Forms/      النوافذ (Login, Main, الـ Dialogs)
  Views/      الشاشات اللي بتتفتح جوه النافذة الرئيسية
  Controls/   عناصر مرسومة بـ GDI+ (الرسومات، الأزرار، الكروت)
  Helpers/    Theme (الألوان)، SampleData (داتا تجريبية)، Session
  Models/     كلاسات البيانات (Expense, Category)
docs/         ملفات المشروع (الخطة، الدوكيومنتيشن، تصميم الشاشات)
```
كل شاشة في ملف لوحدها، فاحنا مش هنتعارض كتير على نفس الملف.

## إزاي نرفع شغلنا على GitHub
- `main` = النسخة النهائية، و`develop` = اللي بنجمّع فيه شغلنا. **محدش يشتغل على main.**

أول مرة:
```
git clone <رابط الريبو>
cd Masareefy
git checkout develop
```

كل مرة تشتغل:
```
git checkout develop
git pull
git checkout -b feature/اسم-المهمة      (مثال: feature/database)
... اشتغل ...
git add .
git commit -m "add: وصف قصير لللي عملته"
git push -u origin feature/اسم-المهمة
```
بعدين افتح **Pull Request** على GitHub من فرعك على `develop`، وواحد من التيم يراجعه ويعمل Merge.

قواعد بسيطة:
- اعمل `git pull` قبل ما تبدأ، وقول في الجروب إنت شغال على أنهي شاشة.
- Commit كتير ورسالة قصيرة.
- لو حصل Conflict متحلهوش لوحدك، نحلّه سوا.
