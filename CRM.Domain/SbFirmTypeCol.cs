using System.Collections;
using System.Collections.Generic;

namespace CRM.Domain
{
    public class SbFirmTypeCol : IEnumerable<SbFirmType>
    {
        private List<SbFirmType> _lst = new List<SbFirmType>();

        public int Count => _lst.Count;

        public void Add(SbFirmType t) => _lst.Add(t);

        public IEnumerator<SbFirmType> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}