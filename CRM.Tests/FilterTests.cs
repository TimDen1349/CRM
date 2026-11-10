using CRM.AppLogic;
using CRM.Domain;
using CRM.Domain.Filtering;
using CRM.Domain.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;

namespace CRM.Tests
{
    [TestClass]
    public class FilterTests
    {
        private List<Firm> GetTestFirms()
        {
            // Создаем тестовые фирмы (предполагая, что фабрика из лабы 3 работает)
            var f1 = FirmFactory.Instance.Create("Ромашка", "RU", "МО", "Москва", "", "", DateTime.Now, "", "", new SbFirmType("Главный", true));
            var f2 = FirmFactory.Instance.Create("Лютик", "RU", "ЛО", "СПБ", "", "", DateTime.Now, "", "", new SbFirmType("Главный", true));
            return new List<Firm> { f1, f2 };
        }

        [TestMethod]
        public void StrRule_Contains_WorksCorrectly()
        {
            var rule = new StrRule("Город", f => f.Town) { IsEnabled = true, Operation = LogExpEnum.Contains, FilterText = "Моск" };
            var firm = GetTestFirms()[0]; // Москва

            Assert.IsTrue(rule.FirmRespond(firm));
        }

        [TestMethod]
        public void FilterContr_AppliesMultipleRules()
        {
            var mngr = new FirmMngr(GetTestFirms());
            var contr = new FilterContr(mngr);

            // Правило 1: Город содержит "Москва"
            contr.Rules.Add(new StrRule("Город", f => f.Town) { IsEnabled = true, Operation = LogExpEnum.Contains, FilterText = "Москва" });
            // Правило 2: Название содержит "Ромашка"
            contr.Rules.Add(new StrRule("Название", f => f.Name) { IsEnabled = true, Operation = LogExpEnum.Contains, FilterText = "Ромашка" });

            var result = contr.ApplyFilter();

            Assert.AreEqual(1, result.Firms.Count);
            Assert.AreEqual("Ромашка", result.Firms[0].Name);
        }
    }
}