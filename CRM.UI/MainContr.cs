using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CRM.Domain;
using CRM.AppLogic;

namespace CRM.UI
{
    public class MainContr
    {
        // 1. Храним ИСХОДНЫЙ список всех фирм (источник правды)
        private List<Firm> _allFirms;

        public FirmMngr Mngr { get; set; } = null!;
        public frmMain MainForm { get; set; } = null!;

        public MainContr()
        {
            var mainType = new SbFirmType("Главный офис", true);

            var firm1 = FirmFactory.Instance.Create("Ромашка", "RU", "МО", "Москва", "Ленина 1", "101000", DateTime.Now, "a@b.ru", "romashka.ru", mainType);
            var firm2 = FirmFactory.Instance.Create("Лютик", "RU", "ЛО", "Санкт-Петербург", "Невский", "190000", DateTime.Now, "l@b.ru", "lutik.ru", mainType);
            var firm3 = FirmFactory.Instance.Create("Одуванчик", "RU", "МО", "Химки", "Мира", "141400", DateTime.Now, "o@b.ru", "oduvan.ru", mainType);

            var contactType = new ContType("Звонок", "Исходящий");
            firm1.AddContact(new Contact(new DateTime(2026, 5, 20), DateTime.Now, "Тест", "Инфо", contactType));
            firm2.AddContact(new Contact(new DateTime(2024, 1, 15), DateTime.Now, "Тест2", "Инфо", contactType));

            // 2. Сохраняем исходный список
            _allFirms = new List<Firm> { firm1, firm2, firm3 };

            // 3. Инициализируем менеджер на основе исходного списка
            Mngr = new FirmMngr(_allFirms);
            MainForm = new frmMain(this);
        }

        public void ShowFilter()
        {
            // 4. Создаем ВРЕМЕННЫЙ менеджер из ПОЛНОГО списка фирм для фильтрации
            var tempMngr = new FirmMngr(_allFirms);
            // Копируем текущие настройки отображения столбцов
            tempMngr.ViewSettings = Mngr.ViewSettings;

            var filterContr = new FilterContr(tempMngr);

            // Автоматически создаем правила на основе текущих столбцов
            foreach (var field in Mngr.ViewSettings.Fields)
                filterContr.Rules.Add(field.CreateRule());

            using (var frm = new frmFilter(filterContr))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // 5. Заменяем текущий Mngr на отфильтрованный
                    Mngr = filterContr.ApplyFilter();
                    MainForm.RefreshList();
                }
            }
        }
    }
}