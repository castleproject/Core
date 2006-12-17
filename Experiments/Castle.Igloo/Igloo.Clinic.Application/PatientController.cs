using System.Collections.Generic;
using Castle.Igloo.Attributes;
using Castle.Igloo.Contexts;
using Castle.Igloo.Controllers;
using Igloo.Clinic.Domain;
using Igloo.Clinic.Services.Interfaces;

namespace Igloo.Clinic.Application
{
    public class PatientController : BaseController
    {
        private Doctor _doctor = null;
        private IContext _pageContext = null;
        private IList<Patient> _patients =null;
        private IPatientService _patientService = null;
        
        public IContext PageContext
        {
            set { _pageContext = value; }
        }

        [Inject(Name = "doctor")]
        public Doctor Doctor
        {
            set { _doctor = value; }
        }

        [Inject]
        public IList<Patient> Patients
        {
            set { _patients = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientController"/> class.
        /// </summary>
        /// <param name="patientService">The patient service.</param>
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Gets doctor's patient
        /// </summary>
        /// <returns></returns>
        [NoNavigation]
        public virtual IList<Patient> RetrievePatients()
        {
            IList<Patient> patients = _patientService.RetrievePatients(_doctor);
            
            _pageContext.Add("Patients", patients);

            return patients;
        }

        /// <summary>
        /// Create a new patient.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="address">The adress.</param>
        public virtual void InsertPatient(string name, string address)
        {
            Patient patient = new Patient();
            patient.Name = name;
            patient.Address = address;
            _patients.Add(patient);
        }


        /// <summary>
        /// Deletes the patient.
        /// </summary>
        /// <param name="index">The index.</param>
        public virtual void DeletePatient(int index)
        {
            _patients.RemoveAt(index);
        }
    }
}
