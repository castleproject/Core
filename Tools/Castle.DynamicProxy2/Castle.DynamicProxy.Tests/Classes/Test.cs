using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.DynamicProxy.Tests.Classes
{
	public class Test : IDisposable, ICloneable
	{
		public void Dispose()
		{
			
		}

		object ICloneable.Clone()
		{
			return new Test();
		}
	}
}
