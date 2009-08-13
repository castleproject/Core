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
	using System.Configuration;
	using System.Threading;
	using System.Windows.Forms;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.Proxy;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using NUnit.Framework;

	[TestFixture]
	public class SynchronizeFacilityTest
	{
		private WindsorContainer container;
		private Exception uncaughtException;

		[SetUp]
		public void SetUp()
		{
			uncaughtException = null;
			container = new WindsorContainer();

			container.AddFacility<SynchronizeFacility>()
				.Register(Component.For<SynchronizationContext>(),
						  Component.For<AsynchronousContext>(),
						  Component.For<DummyForm>().Named("Dummy")
							.Activator<DummyFormActivator>(),
						  Component.For<IDummyForm>().ImplementedBy<DummyForm>(),
						  Component.For<ClassUsingFormInWindowsContext>(),
						  Component.For<ClassUsingFormInAmbientContext>(),
						  Component.For<SyncClassWithoutContext>(),
						  Component.For<SyncClassOverrideContext>(),
						  Component.For(typeof(IClassUsingContext<>)).ImplementedBy(typeof(ClassUsingContext<>)),
						  Component.For<IWorker>().ImplementedBy<SimpleWorker>(),
						  Component.For<IWorkerWithOuts>().ImplementedBy<AsynchronousWorker>(),
						  Component.For<ManualWorker>()
						  );

			MutableConfiguration componentNode = new MutableConfiguration("component");
			componentNode.Attributes[Constants.SynchronizedAttrib] = "true";
			MutableConfiguration synchronizeNode = new MutableConfiguration("synchronize");
			synchronizeNode.Attributes["contextType"] = typeof(WindowsFormsSynchronizationContext).AssemblyQualifiedName;
			MutableConfiguration doWorkMethod = new MutableConfiguration("method");
			doWorkMethod.Attributes["name"] = "DoWork";
			doWorkMethod.Attributes["contextType"] = typeof(WindowsFormsSynchronizationContext).AssemblyQualifiedName;
			synchronizeNode.Children.Add(doWorkMethod);
			componentNode.Children.Add(synchronizeNode);

			container.Kernel.ConfigurationStore.AddComponentConfiguration("class.needing.context", componentNode);
			container.AddComponent("class.needing.context", typeof(ClassUsingForm));
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceSameKey_AreEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference("key1");
			SynchronizeContextReference sync2 = new SynchronizeContextReference("key1");

			Assert.AreEqual(sync1, sync2);
			Assert.AreEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceDifferentKey_AreNotEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference("key1");
			SynchronizeContextReference sync2 = new SynchronizeContextReference("key2");

			Assert.AreNotEqual(sync1, sync2);
			Assert.AreNotEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceSameType_AreEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference(typeof(string));
			SynchronizeContextReference sync2 = new SynchronizeContextReference(typeof(string));

			Assert.AreEqual(sync1, sync2);
			Assert.AreEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceDifferentType_AreNotEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference(typeof(string));
			SynchronizeContextReference sync2 = new SynchronizeContextReference(typeof(float));

			Assert.AreNotEqual(sync1, sync2);
			Assert.AreNotEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void Synchronize_NoContextSpecified_DoesNotUseContext()
		{
			SyncClassWithoutContext sync = container.Resolve<SyncClassWithoutContext>();
			sync.DoWork();
		}

		[Test]
		public void Synchronize_OverrideContext_UsesContext()
		{
			SyncClassOverrideContext sync = container.Resolve<SyncClassOverrideContext>();
			sync.DoWork();
		}

		[Test]
		public void AddControl_DifferentThread_ThrowsException()
		{
			DummyForm form = new DummyForm();
			ExecuteInThread(delegate { form.AddControl(new Button()); });
			Assert.IsNotNull(uncaughtException, "Expected an exception");

			uncaughtException = null;

			ClassUsingFormInWindowsContext classInCtx = new ClassUsingFormInWindowsContext();
			ExecuteInThread(delegate { classInCtx.DoWork(form); });
			Assert.IsNotNull(uncaughtException, "Expected an exception");
		}

		[Test]
		public void AddControl_DifferentThreadUsingClass_WorksFine()
		{
			DummyForm form = container.Resolve<DummyForm>();
			Assert.AreEqual(0, form.Controls.Count);
			ExecuteInThread(delegate { 
				int count = form.AddControl(new Button());
				Assert.AreEqual(1, count);
			});
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void AddControl_DifferentThreadUsingService_WorksFine()
		{
			IDummyForm form = container.Resolve<IDummyForm>();
			ExecuteInThread(delegate
			{
				int count = form.AddControl(new Button());
				Assert.AreEqual(1, count);
			});
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void AddControl_DifferentThreadInContext_WorksFine()
		{
			DummyForm form = new DummyForm();
			ClassUsingFormInWindowsContext client = container.Resolve<ClassUsingFormInWindowsContext>();
			ExecuteInThread(delegate { client.DoWork(form); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void AddControl_DifferentThreadInContextUsingConfiguration_WorksFine()
		{
			DummyForm form = new DummyForm();
			ClassUsingForm client = container.Resolve<ClassUsingForm>();
			ExecuteInThread(delegate { client.DoWork(form); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void DoWorkGeneric_DifferentThreadInContext_WorksFine()
		{
			DummyForm form = new DummyForm();
			IClassUsingContext<DummyForm> client = container.Resolve<IClassUsingContext<DummyForm>>();
			ExecuteInThread(delegate { Assert.AreEqual(form, client.DoWork(form)); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void AddControl_DifferentThreadInContextUsingConfiguration_UsesAmbientContext()
		{
			DummyForm form = new DummyForm();
			ClassUsingFormInAmbientContext client = container.Resolve<ClassUsingFormInAmbientContext>();
			ExecuteInThread(delegate 
			{
				using (var winCtx = new WindowsFormsSynchronizationContext())
				{
					SynchronizationContext.SetSynchronizationContext(winCtx);
					client.DoWork(form); 
				}
			});
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void GetResultOf_UsingSynchronousContext_WorksFine()
		{
			IWorker worker = container.Resolve<IWorker>();
			Result<int> remaining = Result.Of(worker.DoWork(2));
			Assert.AreEqual(4, remaining.End());
		}

		[Test]
		public void GetResultOf_UsingSynchronousContext_CallsCallback()
		{
			bool called = false;
			IWorker worker = container.Resolve<IWorker>();
			Result<int> result = Result.Of(worker.DoWork(2), (Result<int> remaining) =>
			{
				Assert.AreEqual(4, remaining.End());
				Assert.AreSame(worker, remaining.AsyncState);
				Assert.IsTrue(remaining.CompletedSynchronously);
				called = true;
			}, worker);
			Assert.IsTrue(called);
		}

		[Test]
		public void GetResultOf_UsingAsynchronousContext_WorksFine()
		{
			IWorkerWithOuts worker = container.Resolve<IWorkerWithOuts>();
			Result<int> remaining = Result.Of(worker.DoWork(5));
			Assert.AreEqual(10, remaining.End());
			Assert.IsFalse(remaining.CompletedSynchronously);
		}

		[Test]
		public void GetResultOf_WaitingForResult_WorksFine()
		{
			ManualWorker worker = container.Resolve<ManualWorker>();
			Result<int> remaining = Result.Of(worker.DoWork(5));
			ExecuteInThread(delegate
			{
				Thread.Sleep(1000);
				worker.Ready();
			});
			Assert.AreEqual(10, remaining.End());
			Assert.IsFalse(remaining.CompletedSynchronously);
		}

		[Test]
		public void GetResultOf_UsingAsyncSynchronousContextWithOuts_CallsCallback()
		{
			int passed;
			string batch = "bar";
			var called = new ManualResetEvent(false);
			ManualWorker worker = container.Resolve<ManualWorker>();
			Result<int> result = Result.Of(worker.DoWork(20, ref batch, out passed), 
				(Result<int> remaining) =>
			{
				Assert.AreEqual(40, remaining.End(out batch, out passed));
				Assert.AreEqual(10, passed);
				Assert.AreEqual("foo", batch);
				Assert.AreSame(worker, remaining.AsyncState);
				Assert.IsFalse(remaining.CompletedSynchronously);
				called.Set();
			}, worker);
			Assert.IsTrue(called.WaitOne(5000, false));
		}

		[Test]
		public void GetResultOf_UsingAsyncSynchronousContextWithOuts_CanAccessOuts()
		{
			int passed;
			string batch = "bar";
			var called = new ManualResetEvent(false);
			ManualWorker worker = container.Resolve<ManualWorker>();
			Result<int> result = Result.Of(worker.DoWork(20, ref batch, out passed));
			Assert.AreEqual(40, result.End(out batch));
			Assert.AreEqual("foo", batch);
			Assert.AreEqual(2, result.OutValues.Length);
			Assert.AreEqual(1, result.UnboundOutValues.Length);
			Assert.AreEqual("foo", result.GetOutArg<string>(0));
			Assert.AreEqual(10, result.GetOutArg<int>(1));
			Assert.AreEqual(10, result.GetUnboundOutArg<int>(0));
		}

		[Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Bad Bad Bad...")]
		public void GetResultOf_WaitingForResultThrowsException_WorksFine()
		{
			ManualWorker worker = container.Resolve<ManualWorker>();
			Result<int> remaining = Result.Of(worker.DoWork(5));
			ExecuteInThread(delegate
			{
				worker.Failed(new ArgumentException("Bad Bad Bad..."));
			});
			Assert.AreEqual(10, remaining.End());
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void GetResultOf_NotWithAnySynchronizationContext_ThrowsInvalidOperationException()
		{
			IWorker worker = new AsynchronousWorker();
			Result<int> remaining = Result.Of(worker.DoWork(2));
			Assert.AreEqual(4, remaining);
		}

		[Test, ExpectedException(typeof(FacilityException))]
		public void AddContextComponent_WithoutVirtualMethod_ThrowsFacilityException()
		{
			container.AddComponent("class.in.context.bad", typeof(ClassInContextWithoutVirtualMethod));
		}

		[Test, ExpectedException(typeof(HandlerException))]
		public void ResolveContextComponent_WithMissingDependency_ThrowsHandlerException()
		{
			container.AddComponent("class.in.context.bad", typeof(ClassInContextWithMissingDependency));
			container.Resolve("class.in.context.bad");
		}

		[Test]
		public void RegisterFacility_WithControlProxyHook_WorksFine()
		{
			WindsorContainer container2 = new WindsorContainer();

			MutableConfiguration facNode = new MutableConfiguration("facility");
			facNode.Attributes["id"] = "sync.facility";
			facNode.Attributes[Constants.ControlProxyHookAttrib] = typeof(DummyProxyHook).AssemblyQualifiedName;
			container2.Kernel.ConfigurationStore.AddFacilityConfiguration("sync.facility", facNode);
			container2.AddFacility("sync.facility", new SynchronizeFacility());

			container2.AddComponent("dummy.form.class", typeof(DummyForm));
			ComponentModel model = container2.Kernel.GetHandler("dummy.form.class").ComponentModel;
			ProxyOptions options = ProxyUtil.ObtainProxyOptions(model, false);
			Assert.IsNotNull(options, "Proxy options should not be null");
			Assert.IsTrue(options.Hook is DummyProxyHook, "Proxy hook should be a DummyProxyHook");
		}

		[Test, ExpectedException(typeof(ConfigurationErrorsException))]
		public void RegisterFacility_WithBadControlProxyHook_ThrowsConfigurationException()
		{
			WindsorContainer container2 = new WindsorContainer();

			MutableConfiguration facNode = new MutableConfiguration("facility");
			facNode.Attributes["id"] = "sync.facility";
			facNode.Attributes[Constants.ControlProxyHookAttrib] = typeof(string).AssemblyQualifiedName;
			container2.Kernel.ConfigurationStore.AddFacilityConfiguration("sync.facility", facNode);
			container2.AddFacility("sync.facility", new SynchronizeFacility());
		}

		private void ExecuteInThread(ThreadStart run)
		{
			Thread thread = new Thread((ThreadStart) delegate
                         	{
                         		try
                         		{
                         			run();
                         		}
                         		catch(Exception e)
                         		{
                         			uncaughtException = e;
                         		}

                         		Application.DoEvents();
                         		Application.Exit();
                         	});

			Form form = new Form();
			if (form.Handle == IntPtr.Zero)
			{
				throw new InvalidOperationException("Control handle could not be obtained");
			}
			form.BeginInvoke((MethodInvoker)delegate { thread.Start(); });

			Application.Run();
		}
	}
}