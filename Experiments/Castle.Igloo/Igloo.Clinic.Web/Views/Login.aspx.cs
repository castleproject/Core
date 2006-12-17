using System;
using Igloo.Clinic.Application;
using Castle.Igloo.UI.Web;


namespace Igloo.Clinic.Web.Views
{
    public partial class Login : Page
    {
        private LoginController _loginController = null;

        public LoginController LoginController
        {
            set { _loginController = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {}

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            if (!_loginController.Validate(login.Text, password.Text))
            {
                LiteralMessage.Text = Messages["unknown"];
            }
        }
    }
}
