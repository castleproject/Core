namespace Castle.Services.Security
{
    using System;
    using System.Security.Principal;

    /// <summary>
	/// Summary description for DenyPolicy.
	/// </summary>
	public class AllowPolicy : IPolicy
	{
        String[] _roles;
        IPrincipal _principal;

		public AllowPolicy(String[] roles, IPrincipal principal)
		{
			this._roles = roles;
            this._principal = principal;
        }

        #region IPolicy Members

        public bool Evaluate()
        {
            bool result = false;
            foreach(String role in this._roles)
            {
                if(this._principal.IsInRole(role))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        #endregion
    }
}
