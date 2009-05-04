using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;

namespace Castle.Windsor.Tests.Bugs.FACILITIES_ISSUE_111.Components
{
	public class A_Facilities_Issue_111 : IA_Facilities_Issue_111, IStartable
	{
		public A_Facilities_Issue_111(IB_Facilities_Issue_111[] ibs)
		{
			this.ibs = ibs;
		}

		IB_Facilities_Issue_111[] ibs;

		public void Method()
		{
			Console.WriteLine("A: Method");
		}

		public void Start()
		{
			Console.WriteLine("Started A");
		}

		public void Stop()
		{
			Console.WriteLine("Stopped A");
		}
	}
}
