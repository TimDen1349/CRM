using System;

namespace CRM.Domain
{
    public class Contact
    {
        private DateTime _beginDt;
        private DateTime _endDt;
        private string _descr;
        private string _dataInfo;
        private ContType _tpy;

        public DateTime BeginDt { get => _beginDt; set => _beginDt = value; }
        public DateTime EndDt { get => _endDt; set => _endDt = value; }
        public string Descr { get => _descr; set => _descr = value; }
        public string DataInfo { get => _dataInfo; set => _dataInfo = value; }
        public ContType CntType { get => _tpy; set => _tpy = value; }

        public Contact() { }

        public Contact(DateTime beginDt, DateTime endDt, string descr, string dataInfo, ContType tpy)
        {
            _beginDt = beginDt;
            _endDt = endDt;
            _descr = descr;
            _dataInfo = dataInfo;
            _tpy = tpy;
        }

        public Contact Clone()
        {
            return new Contact(_beginDt, _endDt, _descr, _dataInfo, _tpy);
        }

        public static bool operator ==(Contact a, Contact b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a._beginDt == b._beginDt
                && a._endDt == b._endDt
                && a._descr == b._descr
                && a._dataInfo == b._dataInfo
                && Equals(a._tpy, b._tpy);
        }

        public static bool operator !=(Contact a, Contact b) => !(a == b);

        public override bool Equals(object obj) => obj is Contact c && this == c;
        public override int GetHashCode() =>
            HashCode.Combine(_beginDt, _endDt, _descr, _dataInfo, _tpy);
    }
}