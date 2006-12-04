// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Tests
{
	using Castle.Core;
	using NUnit.Framework;

	[PerThread]
	public class R
	{
	}

	public interface IC
	{
		IN N { get; set; }
	}

	public class C : IC
	{
		private R _r = null;

		public R R
		{
			set { _r = value; }
		}

		private IN _n = null;

		public IN N
		{
			get { return _n; }
			set { _n = value; }
		}
	}

	public interface IN
	{
		IS CS { get; }
	}

	[Transient]
	public class DN : IN
	{
		private IS _s = null;
		private IWM _vm = null;
		private ISP _sp = null;

		public IS CS
		{
			get { return _s; }
		}

		public DN(IWM vm, ISP sp)
		{
			_vm = vm;
			_sp = sp;
			_s = new BS();
		}
	}

	public interface IWM
	{
		void A(IN n);
	}

	public class WM : IWM
	{
		public void A(IN n)
		{
			//...
		}
	}

	public interface IS
	{
		ISP SP { get; set; }
	}

	[Transient]
	public class BS : IS
	{
		private ISP _sp = null;

		public ISP SP
		{
			get { return _sp; }
			set { _sp = value; }
		}
	}

	public interface ISP
	{
		void Save(IS s);
	}

	public class SP : ISP
	{
		public void Save(IS s)
		{
		}
	}
	
	[TestFixture]
	public class ContainerProblem2
	{
		[Test]
		public void CausesStackOverflow()
		{
			IWindsorContainer container = new WindsorContainer();

			container.AddComponent("BS", typeof(IS), typeof(BS));
			container.AddComponent("C", typeof(IC), typeof(C));
			container.AddComponent("WM", typeof(IWM), typeof(WM));
			container.AddComponent("SP", typeof(ISP), typeof(SP));

			// ComponentModel model = new ComponentModel("R", typeof(R), typeof(R));
			// model.LifestyleType = LifestyleType.Custom;
			// model.CustomLifestyle = typeof(PerThreadLifestyleManager);

			// container.Kernel.AddCustomComponent(model);
			// container.Kernel.AddComponent("R", typeof(R), LifestyleType.Thread);
			container.Kernel.AddComponent("R", typeof(R));

			IC c = container["C"] as IC;
			Assert.IsNotNull(c);
		}
	}
}