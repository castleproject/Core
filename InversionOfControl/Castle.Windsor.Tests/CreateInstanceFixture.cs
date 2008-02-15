namespace Castle.Windsor.Tests
{
	using System.Collections;
	using Castle.MicroKernel.Registration;
	using NUnit.Framework;

	[TestFixture]
	public class CreateInstanceFixture
	{
		[Test]
		public void CanCreateInstanceWithNoDependencies()
		{
			IWindsorContainer container = new WindsorContainer();
			NoDeps instance = container.CreateInstance<NoDeps>();
			Assert.IsNotNull(instance);
		}

		[Test]
		public void CanCreateInstanceWithDependenciesFromConatiner()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Register(
				Component.For<ICache<string>>()
					.ImplementedBy<HashTableCache<string>>()
				);
			WithCacheDependency instance = container.CreateInstance<WithCacheDependency>();
			Assert.IsNotNull(instance);
			Assert.IsTrue(instance.Cache is HashTableCache<string>);
		}

		[Test]
		public void CanCreateInstanceWithParametersFromDictionaryAndFromContainer()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Register(
				Component.For<ICache<string>>()
					.ImplementedBy<HashTableCache<string>>()
				);
			Hashtable args = new Hashtable();
			args["name"] = "ayende";
			WithCacheDependency2 instance = container.CreateInstance<WithCacheDependency2>(args);
			Assert.IsNotNull(instance);
			Assert.IsTrue(instance.Cache is HashTableCache<string>);
			Assert.AreEqual("ayende", instance.Name);
		}

		public class WithCacheDependency2
		{
			private readonly ICache<string> cache;
			private readonly string name;

			public ICache<string> Cache
			{
				get { return cache; }
			}

			public string Name
			{
				get { return name; }
			}

			public WithCacheDependency2(ICache<string> cache, string name)
			{
				this.cache = cache;
				this.name = name;
			}
		}

		public class WithCacheDependency
		{
			private readonly ICache<string> cache;

			public ICache<string> Cache
			{
				get { return cache; }
			}

			public WithCacheDependency(ICache<string> cache)
			{
				this.cache = cache;
			}
		}

		public class NoDeps { }
	}
}