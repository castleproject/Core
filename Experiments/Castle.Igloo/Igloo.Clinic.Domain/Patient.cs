
namespace Igloo.Clinic.Domain
{
    public class Patient
    {
        private string _name = string.Empty;
        private string _address = string.Empty;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
