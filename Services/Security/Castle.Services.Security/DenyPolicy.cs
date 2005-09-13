namespace Castle.Services.Security
{
    using System;
    using System.Security.Principal;

    /// <summary>
	/// Summary description for DenyPolicy.
	/// </summary>
	public class DenyPolicy : IPolicy
	{
        String[] _roles;
        IPrincipal _principal;

        public DenyPolicy(String[] roles, IPrincipal principal)
        {
            this._roles = roles;
            this._principal = principal;
        }

        #region IPolicy Members

        public bool Evaluate()
        {
            bool result = true;
            foreach(String role in this._roles)
            {
                if(this._principal.IsInRole(role))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion
    }
}
