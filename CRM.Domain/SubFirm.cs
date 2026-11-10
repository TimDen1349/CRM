using System.Collections.Generic;

namespace CRM.Domain
{
    public class SubFirm
    {
        private string _name;
        private string _bossName;
        private string _ofcBossName;
        private string _tel;
        private string _email;
        private SbFirmType _tpy;
        private List<Contact> _conts = new List<Contact>();

        public string Name { get => _name; set => _name = value; }
        public string BossName { get => _bossName; set => _bossName = value; }
        public string OfcBossName { get => _ofcBossName; set => _ofcBossName = value; }
        public string Tel { get => _tel; set => _tel = value; }
        public string Email { get => _email; set => _email = value; }
        public SbFirmType SbFirmTpy { get => _tpy; set => _tpy = value; }

        public int ContactCount => _conts.Count;

        public bool IsMain => _tpy != null && _tpy.IsMain;

        public SubFirm() { }

        public void AddContact(Contact c) => _conts.Add(c);

        public bool ExistContact(Contact c) => _conts.Contains(c);

        public List<Contact> GetAllContacts()
        {
            return _conts;
        }

        public Contact GetContact(Contact c)
        {
            foreach (var contact in _conts)
            {
                if (contact == c) return contact;
            }
            return null;
        }

        public Contact GetContactByType(ContType c)
        {
            foreach (var contact in _conts)
            {
                if (contact.CntType == c) return contact;
            }
            return null;
        }

        public bool IsYourType(SbFirmType t)
        {
            if (t == null || _tpy == null) return false;
            return _tpy.Name == t.Name;
        }
    }
}