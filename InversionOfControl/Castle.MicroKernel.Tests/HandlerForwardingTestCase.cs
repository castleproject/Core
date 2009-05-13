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

using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Castle.MicroKernel.Tests
{
	[TestFixture]
	public class HandlerForwardingTestCase
	{
		[Test]
		public void Can_register_handler_forwarding()
		{
			IKernel kernel = new DefaultKernel();
			kernel.Register(
				Component.For<IUserRepository, IRepository>()
					.ImplementedBy<MyRepository>()
				);

			Assert.AreSame(
				kernel[typeof(IRepository)],
				kernel[typeof(IUserRepository)]
				);
		}

		[Test]
		public void Can_register_handler_forwarding_using_generics()
		{
			IKernel kernel = new DefaultKernel();
			kernel.Register(
				Component.For<IUserRepository, IRepository<User>>()
					.ImplementedBy<MyRepository>()
				);
			Assert.AreSame(
				kernel[typeof(IRepository<User>)],
				kernel[typeof(IUserRepository)]
				);
		}

		[Test]
		public void Can_register_several_handler_forwarding()
		{
			IKernel kernel = new DefaultKernel();
			kernel.Register(
				Component.For<IUserRepository>()
					.Forward<IRepository, IRepository<User>>()
						.ImplementedBy<MyRepository>()
				);

			Assert.AreSame(
				kernel[typeof(IRepository<User>)],
				kernel[typeof(IUserRepository)]
				);
			Assert.AreSame(
				kernel[typeof(IRepository)],
				kernel[typeof(IUserRepository)]
				);
		}

		[Test]
		public void Can_register_handler_forwarding_with_dependencies()
		{
			IKernel kernel = new DefaultKernel();
			kernel.Register(
				Component.For<IUserRepository, IRepository>()
					.ImplementedBy<MyRepository2>(),
				Component.For<ServiceUsingRepository>()
				);

			kernel.Register(Component.For<User>());

			ServiceUsingRepository service =
				kernel.Resolve<ServiceUsingRepository>();
		}

		[Test]
		public void ResolveAll_Will_Only_Resolve_Unique_Handlers()
		{
			IKernel kernel = new DefaultKernel();
			kernel.Register(
				Component.For<IUserRepository, IRepository>()
					.ImplementedBy<MyRepository>()
				);

			IRepository[] repos = kernel.ResolveAll<IRepository>();
			Assert.AreEqual(1, repos.Length);
		}

		public interface IRepository
		{

		}

		public interface IRepository<T> : IRepository
		{

		}

		public class User { }

		public interface IUserRepository : IRepository<User>
		{

		}

		public class MyRepository : IUserRepository
		{

		}

		public class MyRepository2 : IUserRepository
		{
			public MyRepository2(User user)
			{
			}
		}

		public class ServiceUsingRepository
		{
			public ServiceUsingRepository(IRepository repos)
			{
			}
		}
	}
}
