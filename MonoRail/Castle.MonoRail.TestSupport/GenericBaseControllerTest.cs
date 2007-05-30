#if DOTNET2
namespace Castle.MonoRail.TestSupport
{
	using Castle.MonoRail.Framework;

	public class GenericBaseControllerTest<C> : BaseControllerTest where C : Controller
	{
		protected C controller;
	}
}
#endif