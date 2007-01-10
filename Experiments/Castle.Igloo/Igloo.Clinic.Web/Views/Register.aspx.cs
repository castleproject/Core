using System;
using Igloo.Clinic.Application;
using Castle.Igloo.UI.Web;

namespace Igloo.Clinic.Web.Views
{
    public partial class Register : Page
    {
        private LoginController _loginController = null;

        public LoginController LoginController
        {
            set { _loginController = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonRegister_Click(object sender, EventArgs e)
        {
            if (!_loginController.Register(TextBoxName.Text,
                                           TextBoxLogin.Text, 
                                           TextBoxPassword.Text))
            {
                LiteralMessage.Text = FlashMessages["ALREADY"];
            }
        }
    }
}
