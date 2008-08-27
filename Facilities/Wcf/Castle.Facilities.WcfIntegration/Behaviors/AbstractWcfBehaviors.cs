using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel;
using System.Threading;
using System.Reflection;

namespace Castle.Facilities.WcfIntegration
{
	internal abstract class AbstractWcfBehaviors : IWcfBehavior
	{
		public abstract void Accept(IWcfBehaviorVisitor visitor);

		public abstract ICollection<IHandler> GetHandlers(IKernel kernel);
	}
}
