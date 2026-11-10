using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using System.IO; // Не забудьте добавить это в using


namespace CRM.UITests
{
    [TestClass]
    public class MainFormUITests
    {
        private Process _app;


    [TestInitialize]
        public void Setup()
        {
            // 1. Получаем папку, где лежит сам тест (CRM.UITests\bin\Debug\net10.0-windows)
            string testDir = AppDomain.CurrentDomain.BaseDirectory;

            // 2. Формируем путь к exe-файлу относительно папки теста
            // Выходим на 4 уровня вверх до папки решения (CRM), а затем идем в CRM.UI
            string exePath = Path.GetFullPath(Path.Combine(testDir, @"..\..\..\CRM.UI\bin\Debug\net10.0-windows\CRM.UI.exe"));

            // Проверка: если файл не найден, мы выведем понятную ошибку
            if (!File.Exists(exePath))
            {
                throw new FileNotFoundException($"Не удалось найти приложение по пути: {exePath}. Убедитесь, что проект CRM.UI успешно собран.");
            }

            // Запускаем
            _app = Process.Start(exePath);
            Thread.Sleep(4000);
        }

    [TestCleanup]
        public void Cleanup()
        {
            if (_app != null && !_app.HasExited) _app.Kill();
        }

        [TestMethod]
        public void UITest_FilterByCity_UpdatesListView()
        {
            // 1. Находим главное окно приложения
            var mainWindow = AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, "CRM Система"));

            Assert.IsNotNull(mainWindow, "Главное окно не найдено");

            // 2. Находим кнопку "Фильтр" и нажимаем
            var btnFilter = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "Фильтр"));

            ((InvokePattern)btnFilter.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
            Thread.Sleep(1000);

            // 3. Находим открывшееся окно фильтра
            var filterWindow = AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, "Настройка фильтра"));

            Assert.IsNotNull(filterWindow, "Окно фильтра не открылось");

            // 4. Находим поле ввода для города (можно искать по типу ControlType.Edit)
            var txtCity = filterWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "txtField_1"));

            // Вводим "Москва"
            ((ValuePattern)txtCity.GetCurrentPattern(ValuePattern.Pattern)).SetValue("Москва");

            // 5. Нажимаем "Применить"
            var btnApply = filterWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "Применить"));
            ((InvokePattern)btnApply.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
            Thread.Sleep(1000);

            // 6. Возвращаемся на главное окно и проверяем ListView
            var listView = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List));

            // Смотрим, сколько элементов в списке после фильтрации
            var listItems = listView.FindAll(TreeScope.Children,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));

            // Ожидаем, что останется только 1 фирма (Ромашка из Москвы)
            Assert.AreEqual(1, listItems.Count, "Фильтрация по городу не сработала");
        }
    }
}