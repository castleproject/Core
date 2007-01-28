
namespace Igloo.Clinic.Domain
{
    public class Drug
    {
        private string _name;
        private string _description;
        private long _id = int.MinValue;

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Drug(long id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
        }


    }
}
