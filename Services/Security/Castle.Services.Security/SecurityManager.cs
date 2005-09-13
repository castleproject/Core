// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Services.Security
{
    using System;
    using System.Collections;
    using System.Security.Principal;

    /// <summary>
	/// Summary description for SecurityManager.
	/// </summary>
	public class SecurityManager : ISecurityManager
	{
		public SecurityManager(){}

        public static Authorization Check(PermissionAttribute permission, IPrincipal principal)
        {
            Authorization result = new Authorization(false);
            String[] roles = permission.Roles;

            foreach(String role in roles)
            {
                if(principal.IsInRole(role))
                {
                    result = new Authorization(true);
                    break;
                }
            }
            return result;
        }
        #region ISecurityManager Members

        public IPolicy Generate(PermissionAttribute permission, IPrincipal principal)
        {
            return this.FindPolicy(permission.Roles, principal)[permission.GetType()] as IPolicy;
        }

        #endregion

        private IDictionary FindPolicy(String[] roles, IPrincipal principal)
        {
            IDictionary dict = new Hashtable();
            dict.Add(typeof(DenyAttribute), new DenyPolicy(roles, principal));
            dict.Add(typeof(AllowAttribute), new AllowPolicy(roles, principal));
            return dict;
        }
    }
}
