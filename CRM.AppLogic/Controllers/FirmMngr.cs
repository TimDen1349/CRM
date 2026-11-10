using System;
using System.Collections.Generic;
using System.Text;
using CRM.Domain;
using CRM.Domain.Views;

namespace CRM.AppLogic
{
    // Менеджер хранит список фирм и текущую настройку отображения
    public class FirmMngr
    {
        public List<Firm> Firms { get; private set; }
        public FirmVw ViewSettings { get; set; }

        public FirmMngr(List<Firm> initialFirms)
        {
            Firms = initialFirms ?? new List<Firm>();
            ViewSettings = new FirmVw();
            // Настраиваем поля по умолчанию
            ViewSettings.Fields.Add(new NameField());
            ViewSettings.Fields.Add(new TownField());
            ViewSettings.Fields.Add(new DateInField());
            ViewSettings.Fields.Add(new LastContactDateField());
        }
    }

    // Контроллер фильтрации
    public class FilterContr
    {
        private FirmMngr _mngr;
        public List<FilterRule> Rules { get; set; } = new List<FilterRule>();

        public FilterContr(FirmMngr mngr)
        {
            _mngr = mngr;
        }

        // Выполняет фильтрацию и возвращает НОВЫЙ менеджер с отфильтрованными фирмами
        public FirmMngr ApplyFilter()
        {
            var filteredFirms = new List<Firm>();
            foreach (var firm in _mngr.Firms)
            {
                bool pass = true;
                foreach (var rule in Rules)
                {
                    if (!rule.FirmRespond(firm))
                    {
                        pass = false;
                        break;
                    }
                }
                if (pass) filteredFirms.Add(firm);
            }

            var newMngr = new FirmMngr(filteredFirms);
            newMngr.ViewSettings = _mngr.ViewSettings; // Сохраняем настройки столбцов
            return newMngr;
        }
    }
}