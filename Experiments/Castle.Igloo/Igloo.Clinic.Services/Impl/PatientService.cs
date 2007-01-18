
using System.Collections;
using System.Collections.Generic;
using Castle.Igloo;
using Castle.Igloo.Attributes;
using Igloo.Clinic.Domain;
using Igloo.Clinic.Services.Interfaces;

namespace Igloo.Clinic.Services.Impl
{
    /// <summary>
    /// This component is a singleton
    /// The component cart is session scope so a proxy will be injected
    /// </summary>
    [Scope(Scope = ScopeType.Application)]
    public class PatientService : IPatientService
    {
        private ICart _cart = null;

        public PatientService(ICart cart)
        {
            _cart = cart;
        }


        public IList<Patient> RetrievePatients(Doctor doctor)
        {
            IList<Patient> patients = new List<Patient>();

            int i = _cart.Count;

            if (doctor.Name == "NO")
            {
                Patient patient = new Patient();
                patient.Name = "James Bond";
                patient.Address = "Jamaica";
                patients.Add(patient);

                patient = new Patient();
                patient.Name = "Felix Leiter";
                patient.Address = "New York";
                patients.Add(patient);

                patient = new Patient();
                patient.Name = "Miss Moneypenny";
                patient.Address = "London";
                patients.Add(patient);
            }

            return patients;
        }
    }
}
