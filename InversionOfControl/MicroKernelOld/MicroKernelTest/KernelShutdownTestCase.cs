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

namespace Castle.MicroKernel.Test
{
	using System;

	using NUnit.Framework;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for KernelShutdownTestCase.
	/// </summary>
	[TestFixture]
	public class KernelShutdownTestCase : Assertion
	{
		private IKernel m_kernel;

		[SetUp]
		public void Init()
		{
			m_kernel = new DefaultAvalonKernel();
		}

		[Test]
		public void ShutdownOrder()
		{
			m_kernel.AddComponent("parent", typeof (IParent), typeof (ParentImpl));
			m_kernel.AddComponent("child", typeof (IChild), typeof (ChildImpl));

			IHandler handler = m_kernel[ "parent" ];
			IParent parent = handler.Resolve() as IParent;

			ContainerUtil.Dispose(m_kernel);
			
			Assert( parent.Terminated );
		}

		internal interface IParent
		{
			bool Terminated { get; }
		}

		internal interface IChild
		{
			void Terminate();
		}

		[AvalonComponent("Parent", Framework.Lifestyle.Singleton)]
		[AvalonService(typeof (IParent))]
		internal class ParentImpl : IParent, IDisposable
		{
			private IChild m_child;
			private bool m_terminated = false;

			public ParentImpl(IChild child)
			{
				m_child = child;
			}

			public bool Terminated
			{
				get { return m_terminated; }
			}

			#region IDisposable Members

			public void Dispose()
			{
				m_child.Terminate();
				m_terminated = true;
			}

			#endregion
		}

		[AvalonComponent("Child", Framework.Lifestyle.Singleton)]
		[AvalonService(typeof (IChild))]
		internal class ChildImpl : IChild, IDisposable
		{
			private bool m_terminated = false;

			public void Terminate()
			{
				if (m_terminated)
				{
					throw new ApplicationException("Already disposed.");
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				m_terminated = true;
			}

			#endregion
		}
	}
}