
namespace Igloo.Clinic.Domain
{
    public class Doctor
    {
        private string _name = string.Empty;
        private string _login= string.Empty;
        private string _password = string.Empty;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
