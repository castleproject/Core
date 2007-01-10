using System;
using System.Collections.Generic;
using Igloo.Clinic.Domain;
using Igloo.Clinic.Services.Interfaces;

namespace Igloo.Clinic.Services.Impl
{
    public class ServiceAuthentification : IServiceAuthentification
    {
        private IDictionary<string, Doctor> doctors = new Dictionary<string, Doctor>();
        
        public ServiceAuthentification()
        {
            Doctor doctor = new Doctor();
            doctor.Name = "NO";
            doctor.Login = doctor.Name.ToLower();
            doctor.Password = "no";
            doctors.Add(doctor.Login, doctor);

            doctor = new Doctor();
            doctor.Name = "JECKIL";
            doctor.Login = doctor.Name.ToLower();
            doctor.Password = "jeckil";
            doctors.Add(doctor.Login, doctor);
        }
        
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
            if (doctors.ContainsKey(login.ToLower()))
            {
                doctor = doctors[login.ToLower()];
            }
            
            return doctor;
        }

        /// <summary>
        /// Registers a user.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="login">The login.</param>
        /// <param name="passwd">The passwd.</param>
        public void Register(string name, string login, string passwd)
        {
            Doctor doctor = new Doctor();
            doctor.Name = name.ToUpper();
            doctor.Login = login.ToLower();
            doctor.Password = passwd;
            doctors.Add(doctor.Login, doctor);
        }

        #endregion
    }
}
