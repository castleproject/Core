namespace Castle.Services.Security
{
	using System;
	using System.Security.Principal;

	[Serializable]
	public class GenericExtendedPrincipal : GenericPrincipal, IExtendedPrincipal
	{
		private readonly string[] permissions;

		public GenericExtendedPrincipal(IIdentity identity, string[] roles, string[] permissions) : base(identity, roles)
		{
			this.permissions = permissions;
		}

		public bool HasPermission(String permissionName)
		{
			return Array.IndexOf(permissions, permissionName) != -1;
		}
	}
}
