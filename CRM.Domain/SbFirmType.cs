namespace CRM.Domain
{
    public class SbFirmType
    {
        private string _name;
        private bool _isMain;   // является ли этот тип "основным подразделением"

        public string Name { get => _name; set => _name = value; }
        public bool IsMain { get => _isMain; set => _isMain = value; }

        public SbFirmType() { }

        public SbFirmType(string name, bool isMain)
        {
            _name = name;
            _isMain = isMain;
        }

        public override string ToString() => _name;
    }
}