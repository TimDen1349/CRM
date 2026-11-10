using System;
using System.Collections.Generic;
using System.Text;
using System;
using CRM.Domain;
using CRM.Domain.Filtering;

namespace CRM.Domain.Views
{
    public abstract class FilterRule
    {
        public string FieldName { get; set; }
        public bool IsEnabled { get; set; }
        public abstract bool FirmRespond(Firm firm);
    }

    // Правило для строковых полей (Содержит, Не содержит, ==, !=)
    public class StrRule : FilterRule
    {
        public string FilterText { get; set; }
        public LogExpEnum Operation { get; set; }
        private Func<Firm, string> _getter;

        public StrRule(string fieldName, Func<Firm, string> getter)
        {
            FieldName = fieldName;
            _getter = getter;
        }

        public override bool FirmRespond(Firm firm)
        {
            if (!IsEnabled || string.IsNullOrWhiteSpace(FilterText)) return true; // <--- Игнорируем пустоту
            var actual = _getter(firm);
            var exp = LogExpFactory.Create(Operation);
            return exp.Compare(actual, FilterText.Trim()); // Добавили Trim на случай лишних пробелов
        }
    }

    // Правило для дат (>, <, ==, !=)
    public class DateRule : FilterRule
    {
        public DateTime FilterDate { get; set; }
        public LogExpEnum Operation { get; set; }
        private Func<Firm, DateTime> _getter;

        public DateRule(string fieldName, Func<Firm, DateTime> getter)
        {
            FieldName = fieldName;
            _getter = getter;
        }

        public override bool FirmRespond(Firm firm)
        {
            if (!IsEnabled || FilterDate == DateTime.MinValue) return true; // <--- Игнорируем пустые даты
            var actual = _getter(firm);
            var exp = LogExpFactory.Create(Operation);
            return exp.Compare(actual, FilterDate);
        }
    }

    // Правило для целых чисел (Число контактов)
    public class IntRule : FilterRule
    {
        public int FilterValue { get; set; }
        public LogExpEnum Operation { get; set; }
        private Func<Firm, int> _getter;

        public IntRule(string fieldName, Func<Firm, int> getter)
        {
            FieldName = fieldName;
            _getter = getter;
        }

        public override bool FirmRespond(Firm firm)
        {
            if (!IsEnabled) return true;
            var actual = _getter(firm);
            var exp = LogExpFactory.Create(Operation);
            return exp.Compare(actual, FilterValue);
        }
    }
}