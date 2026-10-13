using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM.Domain;

namespace CRM.Tests
{
    [TestClass]
    public class FirmTests
    {
        [TestMethod]
        public void Factory_CreatesFirmWithMainSubFirm()
        {
            var factory = FirmFactory.Instance;
            factory.FldNames("Отрасль", "Размер", "Приоритет", "Менеджер", "Статус");

            var firm = factory.Create("Ромашка", "RU", "МО", "Москва",
                                      "Ленина 1", "101000", DateTime.Now,
                                      "a@b.ru", "romashka.ru");

            Assert.AreEqual("Ромашка", firm.Name);
            Assert.AreEqual(1, firm.SbFirmsCount);
            Assert.IsNotNull(firm.GetMain());
            Assert.IsTrue(firm.GetMain().IsMain);
            Assert.AreEqual("", firm["Отрасль"]);
        }

        [TestMethod]
        public void AddContact_GoesToMainSubFirm()
        {
            var factory = FirmFactory.Instance;
            factory.FldNames("A", "B", "C", "D", "E");
            var firm = factory.Create("X", "RU", "", "", "", "", DateTime.Now, "", "");

            var type = new ContType("Звонок", "Исходящий");
            var contact = new Contact(DateTime.Now, DateTime.Now.AddMinutes(5),
                                      "Позвонили", "Звонок клиенту", type);

            firm.AddContact(contact);

            Assert.AreEqual(1, firm.GetMain().ContactCount);
            Assert.IsTrue(firm.ExistContact(contact));
        }

        [TestMethod]
        public void Contact_Clone_EqualsOriginal()
        {
            var type = new ContType("Письмо", "");
            var c1 = new Contact(new DateTime(2026, 1, 1), new DateTime(2026, 1, 2),
                                 "desc", "info", type);
            var c2 = c1.Clone();

            Assert.AreEqual(c1, c2);
            Assert.IsTrue(c1 == c2);
            Assert.IsFalse(ReferenceEquals(c1, c2));
        }

        [TestMethod]
        public void ExistContact_ReturnsFalse_ForUnknownContact()
        {
            var factory = FirmFactory.Instance;
            factory.FldNames("A", "B", "C", "D", "E");
            var firm = factory.Create("X", "RU", "", "", "", "", DateTime.Now, "", "");

            var c = new Contact(DateTime.Now, DateTime.Now, "d", "i", new ContType("t", ""));
            Assert.IsFalse(firm.ExistContact(c));
        }

        [TestMethod]
        public void RenameField_PreservesValue()
        {
            var factory = FirmFactory.Instance;
            factory.FldNames("Отрасль", "B", "C", "D", "E");
            var firm = factory.Create("X", "RU", "", "", "", "", DateTime.Now, "", "");

            firm.SetField("Отрасль", "IT");
            firm.RenameField("Отрасль", "Сфера");

            Assert.AreEqual("IT", firm["Сфера"]);
            Assert.IsNull(firm["Отрасль"]);
        }

        [TestMethod]
        public void GetMain_ReturnsNull_IfNoMainExists()
        {
            var firm = new Firm();
            Assert.IsNull(firm.GetMain());
        }
    }
}