using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM.Domain;

namespace CRM.Tests
{
    [TestClass]
    public class FirmTests
    {
        SbFirmType mainType = new SbFirmType("Главный офис", true);

        //  (Проверено) тест для создания Фабрики фирм
        [TestMethod]
        public void Factory_CreatesFirmWithMainSubFirm()
        {
            var Factory = new FirmFactory();
            var firm = FirmFactory.Instance.Create("Ромашка", "RU", "МО", "Москва",
                                                   "Ленина 1", "101000", DateTime.Now,
                                                   "a@b.ru", "romashka.ru", mainType);
            Assert.AreEqual("Ромашка", firm.Name);
            Assert.AreEqual(1, firm.SbFirmsCount);
            var Main_firm = firm.GetMain();
            Assert.IsNotNull(Main_firm);
            Assert.AreEqual(mainType, Main_firm.SbFirmTpy);
            Assert.IsTrue(Main_firm.IsMain);
            Assert.AreEqual("", firm.GetField(Factory.fld[0]));
            Assert.AreEqual("", firm.GetField(Factory.fld[1]));
            Assert.AreEqual("", firm.GetField(Factory.fld[2]));
            Assert.AreEqual("", firm.GetField(Factory.fld[3]));
            Assert.AreEqual("", firm.GetField(Factory.fld[4]));
        }

        //  (Проверено) тест для проверки существования контакта
        [TestMethod]
        public void ExistContact_ReturnsFalse_ForUnknownContact()
        {
            var firm = FirmFactory.Instance.Create("X", "RU", "", "", "", "",
                                                   DateTime.Now, "", "", mainType);
            var c = new Contact(DateTime.Now, DateTime.Now, "d", "i",
                                new ContType("t", ""));
            Assert.IsFalse(firm.ExistContact(c));
        }

        //  (Проверено) тест для проверки клонирования контакта изолированно
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

        //  (Проверено) Тест переименовывания полей в фирме и обращения к несуществующему полю
        [TestMethod]
        public void RenameField_PreservesValue()
        {
            var Factory = new FirmFactory();
            var firm = FirmFactory.Instance.Create("X", "RU", "", "", "", "", DateTime.Now, "", "", mainType);

            firm.SetField(Factory.fld[0], "_IT");
            firm.RenameField(Factory.fld[0], "_Сфера");

            firm.SetField(Factory.fld[4], "_Активно");
            firm.RenameField(Factory.fld[4], "_Состояние");

            Assert.AreEqual("_IT", firm["_Сфера"]);
            Assert.Throws<ArgumentException>(() => firm.GetField(Factory.fld[0]));

            Assert.AreEqual("_Активно", firm["_Состояние"]);
            Assert.Throws<ArgumentException>(() => firm.GetField(Factory.fld[4]));

            firm.RenameField("Несуществующее поле", "Новое имя");
            Assert.IsNull(firm["Новое имя"]);
        }

        //  (Проверено) тест создания фирмы не через FirmFactory и существует ли там main подразделение
        [TestMethod]
        public void GetMain_ReturnsNull_IfNoMainExists()
        {
            var firm = new Firm();
            Assert.IsNull(firm.GetMain());
        }

        //  (Новый) тест для добавления филиала
        [TestMethod]
        public void AddSubFirm_IncreasesCount_AndStoresCorrectly()
        {
            var firm = FirmFactory.Instance.Create("X", "RU", "", "", "", "", DateTime.Now, "", "", mainType);

            var branchType = new SbFirmType("Филиал", false);
            var newSubFirm = new SubFirm
            {
                Name = "Филиал в СПб",
                SbFirmTpy = branchType
            };

            firm.AddSubFirm(newSubFirm);

            Assert.AreEqual(2, firm.SbFirmsCount);
            Assert.IsFalse(newSubFirm.IsMain);
        }

        //  (Новый) тест для добавления контактов в главное подраздение и филиал
        [TestMethod]
        public void AddContacts_ToMainAndSubFirm_DistributesCorrectly()
        {
            var firm = FirmFactory.Instance.Create("X", "RU", "", "", "", "", DateTime.Now, "", "", mainType);

            var branchType = new SbFirmType("Филиал", false);
            var branch = new SubFirm { Name = "Филиал", SbFirmTpy = branchType };
            firm.AddSubFirm(branch);

            var type = new ContType("Звонок", "");
            var contactMain = new Contact(DateTime.Now, DateTime.Now, "Основной", "Инфо1", type);
            var contactBranch = new Contact(DateTime.Now, DateTime.Now, "Филиальный", "Инфо2", type);

            firm.AddContact(contactMain);
            firm.AddContactToSbFirm(1, contactBranch);

            Assert.AreEqual(1, firm.GetMain().ContactCount);
            Assert.AreEqual(1, firm.GetSubFirm(1).ContactCount);

            Assert.IsTrue(firm.ExistContact(contactMain));
            Assert.IsTrue(firm.ExistContact(contactBranch));

            Assert.IsTrue(firm.GetMain().ExistContact(contactMain));
            Assert.IsFalse(firm.GetMain().ExistContact(contactBranch));

            Assert.IsTrue(firm.GetSubFirm(1).ExistContact(contactBranch));
            Assert.IsFalse(firm.GetSubFirm(1).ExistContact(contactMain));
        }

        //  (Новый) тест для добавление контакта в главное подразделение и проверки клон ли это
        [TestMethod]
        public void AddContact_GoesToMainSubFirm_AndIsCloned()
        {
            var firm = FirmFactory.Instance.Create("X", "RU", "", "", "", "", DateTime.Now, "", "", mainType);

            var type = new ContType("Звонок", "Исходящий");
            var templateContact = new Contact(DateTime.Now, DateTime.Now.AddMinutes(5), "Позвонили", "Звонок клиенту", type);

            firm.AddContact(templateContact);

            Assert.AreEqual(1, firm.GetMain().ContactCount);

            var contactInFirm = firm.GetMain().GetContact(0);

            Assert.AreEqual(templateContact.CntType.Name, contactInFirm.CntType.Name);
            Assert.IsFalse(ReferenceEquals(templateContact, contactInFirm));
            Assert.AreNotSame(templateContact, contactInFirm);
        }

        //  (Новый) Тест для проверки добавления и обращения к коллекции типов контактов
        [TestMethod]
        public void ContTypeCol_AddAndRetrieve_WorksCorrectly()
        {
            var col = new ContTypeCol();
            var t1 = new ContType("Звонок", "Исходящий");
            var t2 = new ContType("Письмо", "Входящее");

            col.Add(t1);
            col.Add(t2);

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual("Звонок", col[0].Name);
            Assert.AreEqual("Письмо", col[1].Name);
        }

        //  (Новый) Тест для проверки возможности перебирать коллекцию типов подразделений через foreach
        [TestMethod]
        public void SbFirmTypeCol_SupportsForeachIteration()
        {
            var col = new SbFirmTypeCol();
            col.Add(new SbFirmType("Главный офис", true));
            col.Add(new SbFirmType("Филиал", false));

            int count = 0;
            foreach (var type in col)
            {
                count++;
            }

            Assert.AreEqual(2, count);
        }

        //  (Новый) Тест для проверки функции IsYourType в субфирме
        [TestMethod]
        public void SubFirm_IsYourType_ComparesTypesCorrectly()
        {
            var mainType = new SbFirmType("Главный офис", true);
            var branchType = new SbFirmType("Филиал", false);

            var subFirm = new SubFirm { SbFirmTpy = mainType };

            Assert.IsTrue(subFirm.IsYourType(mainType));
            Assert.IsFalse(subFirm.IsYourType(branchType));
        }

        //  (Новый) Тест для перезаписи значения поля с помощью команда AddField
        [TestMethod]
        public void AddField_OverwritesValue_IfKeyAlreadyExists()
        {
            var firm = FirmFactory.Instance.Create("X", "RU", "", "", "", "", DateTime.Now, "", "", mainType);

            firm.AddField("Отрасль", "IT");
            firm.AddField("Отрасль", "Финансы");

            Assert.AreEqual("Финансы", firm.GetField("Отрасль"));
        }

        //  (Новый) Тест для проверки сравнения типов контакта и перегрузки ==
        [TestMethod]
        public void Contact_Equality_ReturnsFalse_ForDifferentFields()
        {
            var type = new ContType("Звонок", "");
            var c1 = new Contact(DateTime.Now, DateTime.Now, "Описание 1", "Инфо", type);
            var c2 = new Contact(DateTime.Now, DateTime.Now, "Описание 2", "Инфо", type);

            Assert.IsTrue(c1 != c2);
            Assert.IsFalse(c1.Equals(c2));
        }
    }
}