using System;
using Igloo.Clinic.Domain;
using Igloo.Clinic.Services.Interfaces;

namespace Igloo.Clinic.Services.Impl
{
    public class ServiceAuthentification : IServiceAuthentification
    {
        #region IServiceAuthentification Members

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="passwd">The passwd.</param>
        /// <returns></returns>
        public Doctor Validate(string login, string passwd)
        {
            // In real project, must call a service to do authentification
            Doctor doctor =null;
            if (login.ToLower() == "no")
            {
                doctor = new Doctor();
                doctor.Name = login.ToLower();
            }
            else if (login.ToLower() == "jeckil")
            {
                doctor = new Doctor();
                doctor.Name = login.ToLower();
            }
            
            return doctor;
        }

        #endregion
    }
}
