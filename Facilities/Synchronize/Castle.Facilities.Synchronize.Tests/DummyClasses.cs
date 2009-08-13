// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.Synchronize.Tests
{
	using System;
	using System.Reflection;
	using System.Threading;
	using System.Windows.Forms;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Proxy;

	public interface IDummyForm
	{
		int AddControl(Control control);
	}

	public class DummyForm : Form, IDummyForm
	{
		public DummyForm()
		{
			IntPtr handle = Handle;
		}

		public virtual int AddControl(Control control)
		{
			Controls.Add(control);
			return Controls.Count;
		}
	}

	public interface IWorker
	{
		[Synchronize]
		int DoWork(int work);
	}

	public interface IWorkerWithOuts : IWorker
	{
		[Synchronize]
		int DoWork(int work, ref string batch, out int passed);
	}

	public class AsynchronousContext : SynchronizationContext
	{
		public override void Send(SendOrPostCallback d, object state)
		{
			Post(d, state);
		}
	}

	[Synchronize(typeof(SynchronizationContext))]
	public class SimpleWorker : IWorker
	{
		[Synchronize]
		public virtual int DoWork(int work)
		{
			return work * 2;
		}
	}

	[Synchronize(typeof(AsynchronousContext))]
	public class AsynchronousWorker : SimpleWorker, IWorkerWithOuts
	{
		[Synchronize]
		public virtual int DoWork(int work, ref string batch, out int passed)
		{
			batch = "foo";
			passed = work / 2;
			return work * 2;
		}
	}

	public class ManualWorker : AsynchronousWorker, IWorkerWithOuts
	{
		private Exception exception;
		private ManualResetEvent ready = new ManualResetEvent(false);

		[Synchronize]
		public override int DoWork(int work)
		{
			int remaining = base.DoWork(work);
			if (ready.WaitOne(5000, false))
			{
				if (exception != null)
				{
					throw exception;
				}
				return remaining;
			}
			return work * 2;
		}

		public void Ready()
		{
			ready.Set();
		}

		public void Failed(Exception ex)
		{
			exception = ex;
			Ready();
		}
	}

	public class ClassUsingForm
	{
		[Synchronize]
		public virtual int DoWork(Form form)
		{
			form.Controls.Add(new Button());
			return form.Controls.Count;
		}
	}

	[Synchronize]
	public class SyncClassWithoutContext
	{
		[Synchronize]
		public virtual void DoWork()
		{
			
		}
	}

	[Synchronize(typeof(SynchronizationContext))]
	public class SyncClassOverrideContext : SyncClassWithoutContext
	{
		
	}

	[Synchronize(typeof(WindowsFormsSynchronizationContext))]
	public class ClassUsingFormInWindowsContext : ClassUsingForm
	{
	}

	[Synchronize]
	public class ClassUsingFormInAmbientContext
	{
		[Synchronize(UseAmbientContext = true)]
		public virtual int DoWork(Form form)
		{
			form.Controls.Add(new Button());
			return form.Controls.Count;
		}
	}

	[Synchronize(typeof(WindowsFormsSynchronizationContext))]
	public class ClassInContextWithoutVirtualMethod
	{
		[Synchronize]
		public void DoWork(Form form)
		{
			form.Controls.Add(new Button());
		}
	}

	[Synchronize(typeof(SynchronizationContext))]
	public class ClassInContextWithMissingDependency
	{
		[Synchronize("foo")]
		public virtual void DoWork(Form form)
		{
			form.Controls.Add(new Button());
		}
	}

	public interface IClassUsingContext<T> where T : Control
	{
		T DoWork(T work);
	}

	[Synchronize]
	public class ClassUsingContext<T> : IClassUsingContext<T> where T : Control
	{
		[Synchronize(typeof(WindowsFormsSynchronizationContext))]
		public T DoWork(T work)
		{
			work.Controls.Add(new Button());
			return work;
		}
	}

	public class DummyFormActivator : DefaultComponentActivator
	{
		public DummyFormActivator(ComponentModel model, IKernel kernel,
		                          ComponentInstanceDelegate onCreation,
		                          ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object Instantiate(CreationContext context)
		{
			return base.Instantiate(context);
		}
	}

	public class DummyProxyHook : IProxyHook
	{
		public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			return false;
		}

		public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
		{
		}

		public void MethodsInspected()
		{
		}
	}
}