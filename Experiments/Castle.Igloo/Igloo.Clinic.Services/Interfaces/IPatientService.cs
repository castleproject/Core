using System;
using System.Collections.Generic;
using System.Text;
using Igloo.Clinic.Domain;

namespace Igloo.Clinic.Services.Interfaces
{
    public interface IPatientService
    {
        IList<Patient> RetrievePatients(Doctor doctor);
    }
}
