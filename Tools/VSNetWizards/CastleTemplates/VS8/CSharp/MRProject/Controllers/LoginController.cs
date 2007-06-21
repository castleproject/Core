namespace !NAMESPACE!.Controllers
{
	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public class LoginController : SmartDispatcherController
	{
		public void Index()
		{
		
		}

		public void ValidateLogin(string username, string password, bool autoLogin)
		{
			// In a real situation, you would like to authenticate the user here. 
			// As a matter of example, we're just getting the parameters sent 
			// and displaying it back to the user
			PropertyBag["username"] = username;
			PropertyBag["password"] = password;
			PropertyBag["autoLogin"] = autoLogin;
		}
	}
}
