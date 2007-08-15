namespace !NAMESPACE!.Controllers
{
	using Castle.MonoRail.Framework;

	public class LoginController : BaseController
	{
		public void Index()
		{
		}

		[AccessibleThrough(Verb.Post)]
		public void Authenticate(string username, string password, bool autoLogin)
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
