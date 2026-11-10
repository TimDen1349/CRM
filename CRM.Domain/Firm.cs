using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CRM.Domain
{
    public class Firm
    {
        private string _name;
        private string _country;
        private string _region;
        private string _town;
        private string _street;
        private string _postInx;
        private DateTime _dateIn;
        private string _email;
        private string _web;
        private List<SubFirm> _sbFirms = new List<SubFirm>();
        private Dictionary<string, string> _usrFields = new Dictionary<string, string>();

        public string Name { get => _name; set => _name = value; }
        public string Country { get => _country; set => _country = value; }
        public string Region { get => _region; set => _region = value; }
        public string Town { get => _town; set => _town = value; }
        public string Street { get => _street; set => _street = value; }
        public string PostInx { get => _postInx; set => _postInx = value; }
        public DateTime DateIn { get => _dateIn; set => _dateIn = value; }
        public string Email { get => _email; set => _email = value; }
        public string Web { get => _web; set => _web = value; }

        public int SbFirmsCount => _sbFirms.Count;

        public string this[string key]
        {
            get => _usrFields.ContainsKey(key) ? _usrFields[key] : null;
            set => _usrFields[key] = value;
        }

        public Firm() { }

        public SubFirm GetMain()
        {
            foreach (var sf in _sbFirms)
                if (sf.IsMain) return sf;
            return null;
        }

        public SubFirm GetSubFirmByType(SbFirmType type)
        {
            foreach (var sf in _sbFirms)
            {
                if (sf.IsYourType(type))
                {
                    return sf;
                }
            }
            return null;
        }

        public SubFirm GetSubFirmByName(string name)
        {
            foreach (var sf in _sbFirms)
            {
                if (sf.Name == name)
                {
                    return sf;
                }
            }
            return null;
        }
        public void AddSubFirm(SubFirm new_sf)
        {
            if (new_sf == null || new_sf.Name == "" || new_sf.SbFirmTpy == null)
            {
                throw new ArgumentException("Invalid SbFirm");
            }
            foreach (var sf in _sbFirms)
            {
                if (sf.IsYourType(new_sf.SbFirmTpy))
                {
                    throw new ArgumentException("SbFirm with that type already exist in Firm");
                }
                if (sf.Name == new_sf.Name)
                {
                    throw new ArgumentException("SbFirm with that name already exist in Firm");
                }
            }
            _sbFirms.Add(new_sf);
        }

        public bool AddContactToSbFirm(string name_sf, Contact c)
        {
            Contact temp = c.Clone();
            foreach (var sf in _sbFirms)
                if (sf.Name == name_sf)
                {
                    sf.AddContact(temp);
                    return true;
                }
            return false;
        }

        public void AddContact(Contact c)
        {
            Contact temp = c.Clone();
            var main = GetMain();
            if (main != null) main.AddContact(temp);
        }

        public List<Contact> GetContact(Contact c)
        {
            if (c == null) return null;
            var list = new List<Contact>();
            foreach (var sf in _sbFirms)
            {
                var found = sf.GetContact(c);
                if (found != null) list.Add(found);
            }
            return list;
        }

        public List<Contact> GetContactByType(ContType c)
        {
            if (c == null) return null;
            var list = new List<Contact>();
            foreach (var sf in _sbFirms)
            {
                var found = sf.GetContactByType(c);
                if (found != null) list.Add(found);
            }
            return list;
        }

        public bool ExistContact(Contact c)
        {
            foreach (var sf in _sbFirms)
                if (sf.ExistContact(c)) return true;
            return false;
        }

        public string GetField(string key)
            => _usrFields.ContainsKey(key) ? _usrFields[key] : throw new ArgumentException("Invalid key");

        public void SetField(string key, string value) => _usrFields[key] = value;

        public void AddField(string key, string value)
        {
            if (!_usrFields.ContainsKey(key))
                _usrFields.Add(key, value);
            else
                _usrFields[key] = value;
        }

        public void RenameField(string oldName, string newName)
        {
            if (_usrFields.ContainsKey(oldName))
            {
                var val = _usrFields[oldName];
                _usrFields.Remove(oldName);
                _usrFields[newName] = val;
            }
        }
    }
}