using System;
using System.Collections.Generic;
using Castle.Core;

namespace Castle.MicroKernel.Tests.Bugs.Ioc113
{
	public class StartableDisposableAndInitializableComponent : IInitializable, IDisposable, IStartable
	{
		public IList<SdiComponentMethods> calledMethods;

		public StartableDisposableAndInitializableComponent()
		{
			calledMethods = new List<SdiComponentMethods>();
		}

		public void Initialize()
		{
			calledMethods.Add(SdiComponentMethods.Initialize);
		}

		public void Start()
		{
			calledMethods.Add(SdiComponentMethods.Start);
		}

		public void DoSomething()
		{
			calledMethods.Add(SdiComponentMethods.DoSomething);
		}

		public void Stop()
		{
			calledMethods.Add(SdiComponentMethods.Stop);
		}

		public void Dispose()
		{
			calledMethods.Add(SdiComponentMethods.Dispose);
		}
	}
}
