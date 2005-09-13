namespace Castle.Services.Security
{
    using System.Security.Principal;

    /// <summary>
	/// Summary description for ISecurityManager.
	/// </summary>
	public interface ISecurityManager
	{
		IPolicy Generate(PermissionAttribute permission, IPrincipal principal);
	}
}
