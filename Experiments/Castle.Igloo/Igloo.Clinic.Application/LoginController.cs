
#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using Castle.Igloo.Contexts;
using Castle.Igloo.Controllers;
using Igloo.Clinic.Domain;

using Igloo.Clinic.Services.Interfaces;

namespace Igloo.Clinic.Application
{
    public class LoginController : BaseController
	{
        private IContext _sessionContext = null;
        private IServiceAuthentification _serviceAuthentification = null;

        public IContext SessionContext
        {
            set { _sessionContext = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="serviceAuthentification">The service authentification.</param>
        public LoginController(IServiceAuthentification serviceAuthentification)
        {
            _serviceAuthentification = serviceAuthentification;
        }
        
        public virtual bool Validate(string login, string passwd)
		{
            Doctor doctor = _serviceAuthentification.Validate(login, passwd);
            if (doctor!= null)
            {
                // Add an object in the session context under the name "doctor"
                // this object will be inject later
                _sessionContext.Add("doctor", doctor);               
            }
            else
            {
               navigationContext.Action = Castle.Igloo.Navigation.NavigationContext.NO_NAVIGATION;
               messages.Add("unknown", "Unknown login or bad password");
            }
            return (doctor != null);
		}
        
        public virtual void LogOut()
        {
            _sessionContext.Abandon();
        }
	}
}
