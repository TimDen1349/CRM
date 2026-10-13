using System.Collections.Generic;

namespace CRM.Domain
{
    public class ContTypeCol
    {
        private List<ContType> _lst = new List<ContType>();

        public int Count => _lst.Count;

        public ContType this[int i]
        {
            get => _lst[i];
            set => _lst[i] = value;
        }

        public void Add(ContType t) => _lst.Add(t);
        public void Clear() => _lst.Clear();
    }
}