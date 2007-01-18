using System.Collections;
using System.Collections.Generic;
using Igloo.Clinic.Domain;

namespace Igloo.Clinic.Services.Interfaces
{
    public interface IPatientService
    {
        IList<Patient> RetrievePatients(Doctor doctor);
    }
}
