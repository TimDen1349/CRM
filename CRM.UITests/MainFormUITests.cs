using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Automation;
using System.Diagnostics;
using System.Threading;

namespace CRM.UITests
{
    [TestClass]
    public class MainFormUITests
    {
        private Process _app;

        [TestInitialize]
        public void Setup()
        {
            // Запускаем exe-файл нашего приложения (предварительно собрав его)
            _app = Process.Start(@"..\..\..\CRM.UI\bin\Debug\CRM.UI.exe");
            Thread.Sleep(2000); // Ждем загрузки
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
                new Condition(AndCondition(PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit))));

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