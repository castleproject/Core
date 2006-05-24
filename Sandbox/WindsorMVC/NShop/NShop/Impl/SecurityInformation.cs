using System;
using System.Web;
using NShop.Model;
using NShop.Repositories;
using NShop.Services;

namespace NShop.Impl
{
	public class SecurityInformation : ISecurityInformation
	{
		IRepository<ISecurable> securityRepository;

		public IRepository<ISecurable> SecurityRepository
		{
			get { return securityRepository; }
			set { securityRepository = value; }
		}

		#region ISecurityInformation Members

		public bool HasAuthorizationFor(string action)
		{
			HttpContext current = HttpContext.Current;
			if(current == null)
				return false;
			return current.User.Identity.IsAuthenticated;
		}

		#endregion
	}
}