using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Domain.Filtering
{
    public enum LogExpEnum { EQ, NoEQ, GT, LT, GE, LE, Contains, NoContains }

    public interface ILogExp
    {
        bool Compare(object actualValue, object filterValue);
    }

    public class ExpEQ : ILogExp
    {
        public bool Compare(object a, object b) => a != null && a.Equals(b);
    }

    public class ExpNoEQ : ILogExp
    {
        public bool Compare(object a, object b) => a != null && !a.Equals(b);
    }

    public class ExpGT : ILogExp
    {
        public bool Compare(object a, object b) => a is IComparable ca && b is IComparable && ca.CompareTo(b) > 0;
    }

    public class ExpLT : ILogExp
    {
        public bool Compare(object a, object b) => a is IComparable ca && b is IComparable && ca.CompareTo(b) < 0;
    }

    public class ExpContains : ILogExp
    {
        public bool Compare(object a, object b)
        {
            if (a == null || b == null) return false;
            return a.ToString().IndexOf(b.ToString(), StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public class ExpNoContains : ILogExp
    {
        public bool Compare(object a, object b) => !new ExpContains().Compare(a, b);
    }

    // Фабрика выражений
    public static class LogExpFactory
    {
        public static ILogExp Create(LogExpEnum type)
        {
            switch (type)
            {
                case LogExpEnum.EQ: return new ExpEQ();
                case LogExpEnum.NoEQ: return new ExpNoEQ();
                case LogExpEnum.GT: return new ExpGT();
                case LogExpEnum.LT: return new ExpLT();
                case LogExpEnum.Contains: return new ExpContains();
                case LogExpEnum.NoContains: return new ExpNoContains();
                default: throw new ArgumentException("Неизвестный тип выражения");
            }
        }
    }
}
