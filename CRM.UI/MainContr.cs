using CRM.AppLogic;
using CRM.Domain;
using CRM.Domain.Views;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        /*
        public void ShowColumnSettings()
        {
            // 1. Формируем список ВСЕХ возможных полей для нашей CRM
            // (При добавлении новых полей в систему, вы будете добавлять их сюда)
            var allFields = new List<Field>
            {
                new NameField(),
                new TownField(),
                new RegionField(),
                new CountryField(),
                new DateInField(),
                new LastContactDateField(),
                new WebField()
            };

            // 2. Открываем форму, передавая ей все поля и текущие выбранные
            using (var frm = new frmSelectFields(allFields, Mngr.ViewSettings.Fields))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // 3. Если пользователь нажал "Применить", обновляем настройки
                    Mngr.ViewSettings.Fields = frm.SelectedFields;

                    // 4. Перерисовываем таблицу
                    MainForm.RefreshList();
                }
            }
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
        }*/

        public void ShowAddFirm()
        {
            using (var frm = new frmAddEditFirm(null))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // Забираем созданную фирму из Tag
                    var newFirm = frm.Tag as Firm;
                    if (newFirm != null)
                    {
                        _allFirms.Add(newFirm); // Добавляем в исходный список

                        // Сбрасываем фильтр, чтобы показать всё (или применяем текущий)
                        Mngr = new FirmMngr(_allFirms);
                        Mngr.ViewSettings = frmShowSettingsSafe(Mngr.ViewSettings);
                        MainForm.RefreshList();
                    }
                }
            }
        }

        public void ShowEditFirm(Firm firm)
        {
            if (firm == null) return;

            using (var frm = new frmAddEditFirm(firm))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // Данные в firm обновились автоматически (т.к. это ссылочный тип)
                    // Просто перерисовываем таблицу
                    MainForm.RefreshList();
                }
            }
        }

        // Вспомогательный метод, чтобы не сбрасывать настройки столбцов при добавлении
        private FirmVw frmShowSettingsSafe(FirmVw currentSettings)
        {
            var frm = new frmSelectFields(new List<Field>(), currentSettings.Fields); // Упрощенный трюк
            return currentSettings;
        }

        // Создаем список ВСЕХ возможных полей (вынесите его в отдельный метод или свойство)
        private List<Field> GetAllFields()
        {
            return new List<Field>
            {
                new NameField(), new DateInField(), new CountryField(),
                new PostInxField(), new RegionField(), new TownField(),
                new StreetField(), new WebField(), new LastContactDateField(),
                new CountContField(),
                // Пользовательские поля (используем ключи из вашей FirmFactory)
                new UsrField("Отрасль"), new UsrField("Размер"),
                new UsrField("Приоритет"), new UsrField("Менеджер"), new UsrField("Статус")
            };
        }

        public void ShowColumnSettings()
        {
            using (var frm = new frmSelectFields(GetAllFields(), Mngr.ViewSettings.Fields))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Mngr.ViewSettings.Fields = frm.SelectedFields;
                    MainForm.RefreshList();
                }
            }
        }

        public void ShowFilter()
        {
            var tempMngr = new FirmMngr(_allFirms);
            tempMngr.ViewSettings = Mngr.ViewSettings;

            var filterContr = new FilterContr(tempMngr);

            // ВАЖНО: Для фильтра создаем правила для ВСЕХ полей, а не только для тех, что видны в таблице
            foreach (var field in GetAllFields())
                filterContr.Rules.Add(field.CreateRule());

            using (var frm = new frmFilter(filterContr))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Mngr = filterContr.ApplyFilter();
                    MainForm.RefreshList();
                }
            }
        }
    }
}