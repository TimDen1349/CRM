using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CRM.UI;

[assembly: DoNotParallelize] // КРИТИЧЕСКИ ВАЖНО: Запрещаем параллельный запуск тестов

namespace CRM.UITests
{
    [TestClass]
    public class MainFormUITests
    {
        private Thread _uiThread;
        private frmMain _mainForm;
        private MainContr _mainContr; // Сохраняем ссылку на контроллер для проверки данных

        [TestInitialize]
        public void Setup()
        {
            var ready = new ManualResetEventSlim(false);

            _uiThread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                _mainContr = new MainContr();
                _mainForm = _mainContr.MainForm;

                _mainForm.Shown += (s, e) => ready.Set();
                Application.Run(_mainForm);
            });

            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();

            Assert.IsTrue(ready.Wait(5000), "Не удалось запустить главную форму");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_mainForm != null && !_mainForm.IsDisposed)
            {
                try
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        // Жестко закрываем все открытые окна
                        foreach (Form f in Application.OpenForms.Cast<Form>().ToList())
                        {
                            if (f != _mainForm && !f.IsDisposed) f.Close();
                        }
                        _mainForm.Close();
                        Application.ExitThread();
                    }));
                }
                catch { }
            }
            _uiThread?.Join(5000);
        }

        // ==========================================
        // ТЕСТ 1: Фильтрация по городу
        // ==========================================
        [TestMethod]
        public void UITest_FilterByCity_UpdatesListView()
        {
            var btnFilter = WaitForControl<Button>(_mainForm, "btnFilter");
            btnFilter.BeginInvoke(new Action(() => btnFilter.PerformClick()));

            var filterForm = WaitForDialog("Настройка фильтра");
            Assert.IsNotNull(filterForm, "Окно фильтра не открылось");

            DisableRule(filterForm, 0); // Отключаем Название
            DisableRule(filterForm, 2); // Отключаем Дату ввода
            DisableRule(filterForm, 3); // Отключаем Дату контакта

            var txtTown = WaitForControl<TextBox>(filterForm, "txtField_1");
            Assert.IsNotNull(txtTown, "Поле ввода города не найдено");
            txtTown.Invoke(new Action(() => txtTown.Text = "Москва"));

            var btnApply = WaitForControl<Button>(filterForm, "btnApply");
            btnApply.Invoke(new Action(() => btnApply.PerformClick()));

            // Ждем, пока модель данных и UI обновятся
            Assert.IsTrue(WaitForListCount(1), "Фильтр по городу не сработал (ожидалась 1 фирма)");
        }

        // ==========================================
        // ТЕСТ 2: Фильтрация по Дате посл. контакта
        // ==========================================
        [TestMethod]
        public void UITest_FilterByContactDate_UpdatesListView()
        {
            var btnFilter = WaitForControl<Button>(_mainForm, "btnFilter");
            btnFilter.BeginInvoke(new Action(() => btnFilter.PerformClick()));

            var filterForm = WaitForDialog("Настройка фильтра");
            Assert.IsNotNull(filterForm, "Окно фильтра не открылось");

            DisableRule(filterForm, 0); // Название
            DisableRule(filterForm, 1); // Город
            DisableRule(filterForm, 2); // Дата ввода

            var dtpDate = WaitForControl<DateTimePicker>(filterForm, "dtpField_3");
            Assert.IsNotNull(dtpDate, "Поле даты не найдено");
            dtpDate.Invoke(new Action(() => dtpDate.Value = new DateTime(2025, 1, 1)));

            var rbGT = WaitForControl<RadioButton>(filterForm, "rbGT_3");
            Assert.IsNotNull(rbGT, "Радиокнопка '>' не найдена");
            rbGT.Invoke(new Action(() => rbGT.Checked = true));

            var btnApply = WaitForControl<Button>(filterForm, "btnApply");
            btnApply.Invoke(new Action(() => btnApply.PerformClick()));

            Assert.IsTrue(WaitForListCount(1), "Фильтр по дате не сработал (ожидалась 1 фирма)");
        }

        // ==========================================
        // ТЕСТ 3: Добавление новой фирмы
        // ==========================================
        [TestMethod]
        public void UITest_AddNewFirm_AppearsInListView()
        {
            var listView = WaitForControl<ListView>(_mainForm, "lvFirms");
            int initialCount = listView.Items.Count;

            var btnAdd = WaitForControl<Button>(_mainForm, "btnAdd");
            btnAdd.BeginInvoke(new Action(() => btnAdd.PerformClick()));

            var addWindow = WaitForDialog("Добавление фирмы");
            Assert.IsNotNull(addWindow, "Окно добавления не открылось");

            var txtName = WaitForControl<TextBox>(addWindow, "txtName");
            var txtTown = WaitForControl<TextBox>(addWindow, "txtTown");

            txtName.Invoke(new Action(() => txtName.Text = "Тестовая Фирма"));
            txtTown.Invoke(new Action(() => txtTown.Text = "Тестовый Город"));

            var btnSave = WaitForControl<Button>(addWindow, "btnSave");
            btnSave.Invoke(new Action(() => btnSave.PerformClick()));

            Assert.IsTrue(WaitForListCount(initialCount + 1), "Новая фирма не появилась в таблице");
        }


        // ==========================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ==========================================

        private void DisableRule(Form filterForm, int index)
        {
            var chk = WaitForControl<CheckBox>(filterForm, "chkEnable_" + index);
            if (chk != null)
            {
                chk.Invoke(new Action(() => chk.Checked = true));
            }
        }

        // Надежное ожидание: проверяет и модель данных, и UI
        private bool WaitForListCount(int expectedCount, int timeoutMs = 5000)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                int modelCount = _mainContr.Mngr.Firms.Count; // Проверяем саму модель

                if (modelCount == expectedCount)
                {
                    // Если модель верна, даем UI время перерисоваться и проверяем ListView
                    var listView = WaitForControl<ListView>(_mainForm, "lvFirms", 2000);
                    if (listView != null)
                    {
                        int uiCount = -1;
                        try { listView.Invoke(new Action(() => uiCount = listView.Items.Count)); } catch { }
                        if (uiCount == expectedCount) return true;
                    }
                }
                Thread.Sleep(100);
            }
            return false;
        }

        private T WaitForControl<T>(Control parent, string name, int timeoutMs = 5000) where T : Control
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                var control = FindControl<T>(parent, name);
                if (control != null && !control.IsDisposed) return control;
                Thread.Sleep(100);
            }
            return null;
        }

        private T FindControl<T>(Control parent, string name) where T : Control
        {
            if (parent.Name == name && parent is T t) return t;
            foreach (Control child in parent.Controls)
            {
                if (child.IsDisposed) continue;
                var found = FindControl<T>(child, name);
                if (found != null) return found;
            }
            return null;
        }

        private Form WaitForDialog(string title, int timeoutMs = 5000)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                // ДЕЛАЕМ СНИМОК КОЛЛЕКЦИИ, чтобы избежать ошибки "Collection was modified"
                var openForms = Application.OpenForms.Cast<Form>().ToList();

                foreach (Form f in openForms)
                {
                    if (f.Text == title && f.Visible && !f.IsDisposed)
                    {
                        return f;
                    }
                }
                Thread.Sleep(100);
            }
            return null;
        }
    }
}