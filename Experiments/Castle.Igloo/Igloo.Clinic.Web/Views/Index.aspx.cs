using System;
using System.Collections.Generic;
using Castle.Igloo.Attributes;
using Igloo.Clinic.Domain;
using Igloo.Clinic.Application;
using Castle.Igloo.UI.Web;

namespace Igloo.Clinic.Web.Views
{
    public partial class Index : Page
    {
        private PatientController _patientController = null;
        private IList<Patient> _patients = null;

        [Inject]
        public IList<Patient> Patients
        {
            set { _patients = value; }
        }
        
        public PatientController PatientController
        {
            set { _patientController = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GridViewBlog.DataSource = _patientController.RetrievePatients();
                GridViewBlog.DataBind();
            }
            else
            {
                GridViewBlog.DataSource = _patients;
                GridViewBlog.DataBind();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
