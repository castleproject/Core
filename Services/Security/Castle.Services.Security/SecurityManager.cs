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
