// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MicroKernel.Test.Concerns
{
	using System;

	using NUnit.Framework;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Model.Default;
	using Castle.MicroKernel.Concerns;

	/// <summary>
	/// Summary description for AbstractConcernTestCase.
	/// </summary>
	public abstract class AbstractConcernTestCase : Assertion
	{
		protected IKernel m_kernel = new DefaultAvalonKernel();

		public abstract IConcern Create();

		public abstract void AssertComponent( IComponentModel model, DummyComponent component );

		[Test]
		public void TestApply()
		{
			IConcern concern = Create();
			AssertNotNull( concern );
			concern.Init( m_kernel );
			
			IComponentModel model = 
				new DefaultComponentModelBuilder(m_kernel).BuildModel( 
					"a", typeof(IMyService), typeof(DummyComponent) );

			DummyComponent component = new DummyComponent();

			concern.Apply( model, component );

			AssertComponent( model, component );
		}
	}

	public interface IMyService
	{
	}

	public class DummyComponent : IMyService, 
		ILogEnabled, IConfigurable, IInitializable, 
		IStartable, IContextualizable, IDisposable, ILookupEnabled
	{
		public bool logEnabled;
		public bool configure;
		public bool context;
		public bool initialize;
		public bool start;
		public bool stop;
		public bool dispose;
		public bool lookup;

		#region ILogEnabled Members

		public void EnableLogging(ILogger logger)
		{
			logEnabled = true;
		}

		#endregion

		#region IConfigurable Members

		public void Configure(IConfiguration config)
		{
			configure = true;
		}

		#endregion

		#region IInitializable Members

		public void Initialize()
		{
			initialize = true;
		}

		#endregion

		#region IStartable Members

		public void Start()
		{
			start = true;
		}

		public void Stop()
		{
			stop = true;
		}

		#endregion

		#region IContextualizable Members

		public void Contextualize(IContext arg)
		{
			context = true;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			dispose = true;
		}

		#endregion

		#region ILookupEnabled Members

		public void EnableLookups(ILookupManager manager)
		{
			lookup = true;
		}

		#endregion
	}
}
