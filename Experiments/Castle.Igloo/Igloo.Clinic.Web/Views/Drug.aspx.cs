using System;
using System.Web.UI.WebControls;
using Castle.Igloo.UI.Web;
using Igloo.Clinic.Application;

namespace Igloo.Clinic.Web.Views
{
    public partial class Drug : Page
    {
        private DrugController _drugController =null;
        
        public DrugController DrugController
        {
            set { _drugController = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DataSourceControler_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = _drugController;
        }

        protected void ButtonAdd_Click(object sender, EventArgs e)
        {
            _drugController.Create(TextBoxNom.Text, TextBoxDescription.Text);
            GridView1.DataBind();
        }


    }
}
