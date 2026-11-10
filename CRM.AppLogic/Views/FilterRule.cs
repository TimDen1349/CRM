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
            if (!IsEnabled) return true;
            var actual = _getter(firm);
            var exp = LogExpFactory.Create(Operation);
            return exp.Compare(actual, FilterText);
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
            if (!IsEnabled) return true;
            var actual = _getter(firm);
            var exp = LogExpFactory.Create(Operation);
            return exp.Compare(actual, FilterDate);
        }
    }
}