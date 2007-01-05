using System;
using Castle.MonoRail.TestSupport;

namespace <%= ClassName %>.Tests.Controllers
{
	public class ControllerTestCase : AbstractMRTestCase
	{
		protected override String GetPhysicalDir()
		{
			return "../../public";
		}    
	}
}