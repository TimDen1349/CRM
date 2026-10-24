using System;
using System.Collections.Generic;

namespace CRM.Domain
{
    public class FirmFactory
    {
        private static FirmFactory _instance;
        private static readonly object _lock = new object();

        public static FirmFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new FirmFactory();
                    }
                }
                return _instance;
            }
        }

        //private List<string> _flds = new List<string>();

        //private SbFirmType _mainType = new SbFirmType("Основное подразделение", true);

        //public string NameMain { get => _nameMain; set => _nameMain = value; }
        //public SbFirmType MainType => _mainType;
        /*
        public void FldNames(params string[] names)
        {
            ;
        }

        //public List<string> GetFldNames() => new List<string>(_flds);
        */

        public string[] fld = ["Отрасль", "Размер", "Приоритет", "Менеджер", "Статус"];

        public FirmFactory() { }

        public Firm Create(string name, string country, string region,
                           string town, string street, string postInx,
                           DateTime dateIn, string email, string web,  SbFirmType mainType)
        {
            Firm f = new Firm();
            f.Name = name;
            f.Country = country;
            f.Region = region;
            f.Town = town;
            f.Street = street;
            f.PostInx = postInx;
            f.DateIn = dateIn;
            f.Email = email;
            f.Web = web;

            foreach (var fldName in fld)
                f.AddField(fldName, "");

            SubFirm main = new SubFirm();
            main.Name = mainType.Name;
            main.SbFirmTpy = mainType;
            f.AddSubFirm(main);

            return f;
        }
        //public void ResetState()
        //{
        //    _flds.Clear();
        //}
    }
}