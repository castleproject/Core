// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using Castle.MicroKernel.Proxy;

	public interface IDummyForm
	{
		void AddControl(Control control);
	}

	public class DummyForm : Form, IDummyForm
	{
		public DummyForm()
		{
			IntPtr handle = Handle;
		}

		public virtual void AddControl(Control control)
		{
			Controls.Add(control);
		}
	}

	public class ClassUsingForm
	{
		[Synchronize]
		public virtual void DoWork(Form form)
		{
			form.Controls.Add(new Button());
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
	public class ClassUsingFormInContext : ClassUsingForm
	{
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
		void DoWork(T work);
	}

	[Synchronize]
	public class ClassUsingContext<T> : IClassUsingContext<T> where T : Control
	{
		[Synchronize(typeof(WindowsFormsSynchronizationContext))]
		public void DoWork(T work)
		{
			work.Controls.Add(new Button());
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