namespace Castle.Services.Security.Tests.Model
{
	using System;
	using System.Security.Permissions;

	[CustomPermissionAttribute(SecurityAction.Demand, "can_access_private_info")]
	//[PrincipalPermissionAttribute(SecurityAction.Demand, Role="SuperUser")]
	public class MySecurityClass
	{
		public MySecurityClass()
		{
		}

		[CustomPermissionAttribute(SecurityAction.Demand, "can_do_something_critical")]
		public void DoSomethingCritical()
		{
			
		}
	}
}
