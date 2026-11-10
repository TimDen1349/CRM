using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CRM.Domain; // Где лежит класс Firm

namespace CRM.Domain.Views
{
    public abstract class Field
    {
        public string Name { get; set; }

        // Получить текстовое значение для отображения
        public abstract string GetValue(Firm firm);

        // Создать копию поля
        public abstract Field Clone();

        // Создать правило фильтрации для этого поля
        public abstract FilterRule CreateRule();
    }

    public class NameField : Field
    {
        public NameField() { Name = "Название"; }
        public override string GetValue(Firm firm) => firm.Name;
        public override Field Clone() => new NameField();
        public override FilterRule CreateRule() => new StrRule(Name, f => f.Name);
    }

    public class TownField : Field
    {
        public TownField() { Name = "Город"; }
        public override string GetValue(Firm firm) => firm.Town;
        public override Field Clone() => new TownField();
        public override FilterRule CreateRule() => new StrRule(Name, f => f.Town);
    }

    public class DateInField : Field
    {
        public DateInField() { Name = "Дата ввода"; }
        public override string GetValue(Firm firm) => firm.DateIn.ToShortDateString();
        public override Field Clone() => new DateInField();
        public override FilterRule CreateRule() => new DateRule(Name, f => f.DateIn);
    }

    public class DateBegField : Field
    {
        public DateBegField() { Name = "Дата начала"; }
        public override string GetValue(Firm firm)
        {
            var contacts = firm.GetMain()?.GetAllContacts();
            if (contacts == null || !contacts.Any()) return "";
            return contacts.Max(c => c.BeginDt).ToShortDateString();
        }
        public override Field Clone() => new DateBegField();
        public override FilterRule CreateRule() => new DateRule(Name, f =>
        {
            var contacts = f.GetMain()?.GetAllContacts();
            if (contacts == null || !contacts.Any()) return DateTime.MinValue;
            return contacts.Max(c => c.BeginDt);
        });
    }

    // Класс, хранящий набор отображаемых полей
    public class FirmVw
    {
        public List<Field> Fields { get; set; } = new List<Field>();

        public FirmVw Clone()
        {
            var copy = new FirmVw();
            foreach (var f in Fields) copy.Fields.Add(f.Clone());
            return copy;
        }
    }
}
