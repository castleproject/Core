using System;
using Castle.Igloo;
using Castle.Igloo.Attributes;
using Igloo.Clinic.Domain;
using Igloo.Clinic.Application;
using Castle.Igloo.UI.Web;

namespace Igloo.Clinic.Web.Views
{
    public partial class Site : MasterPage
    {
        private Doctor _doctor = null;
        private LoginController _loginController = null;

        [Inject(Name = "doctor", Scope = ScopeType.Session)]
        public Doctor Doctor
        {
            set { _doctor = value; }
        }

        public LoginController LoginController
        {
            set { _loginController = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            LiteralName.Text += _doctor.Name;
        }

        protected void LinkButtonLogout_Click(object sender, EventArgs e)
        {
            _loginController.LogOut();
        }



    }
}
