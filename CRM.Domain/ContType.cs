namespace CRM.Domain
{
    public class ContType
    {
        private string _name;   // название типа (например, "Звонок")
        private string _note;   // примечание

        public string Name { get => _name; set => _name = value; }
        public string Note { get => _note; set => _note = value; }

        public ContType() { }

        public ContType(string name, string note)
        {
            _name = name;
            _note = note;
        }

        public override string ToString() => _name;
    }
}