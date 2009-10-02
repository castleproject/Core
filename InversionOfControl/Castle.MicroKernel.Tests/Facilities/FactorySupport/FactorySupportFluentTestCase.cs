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

namespace Castle.MicroKernel.Tests.Facilities.FactorySupport
{
	using Castle.Facilities.FactorySupport;
	using Castle.MicroKernel.Registration;
	using NUnit.Framework;

	class User
	{
		public FiscalStability FiscalStability { get; set; }
	}

	enum FiscalStability { DirtFarmer, MrMoneyBags };
	interface ICarProvider { }
	class FerrariProvider : ICarProvider { }
	class HondaProvider : ICarProvider { }
	class AbstractCarProviderFactory
	{
		public ICarProvider Create(User currentUser)
		{
			if (currentUser.FiscalStability == FiscalStability.MrMoneyBags)
				return new FerrariProvider();
			else
				return new HondaProvider();
		}
	}

	[TestFixture]
	public class When_resolving_car_providers_from_an_abstract_factory
	{
		IKernel kernel;
		[SetUp]
		public void SetUp()
		{
			kernel = new DefaultKernel();
			kernel.AddFacility<FactorySupportFacility>();
		}

		[Test]
		public void register_ferrari_implementation_get_ferrari_instance()
		{
			RegisterComponentsImplemtedByFerrari(new User() { FiscalStability = FiscalStability.MrMoneyBags });
			Assert.IsInstanceOf(typeof(FerrariProvider), kernel.Resolve<ICarProvider>());
		}

		[Test]
		public void register_ferrari_implementation_get_honda_instance()
		{
			RegisterComponentsImplemtedByFerrari(new User() { FiscalStability = FiscalStability.DirtFarmer });
			Assert.IsInstanceOf(typeof(HondaProvider), kernel.Resolve<ICarProvider>());
		}

		[Test]
		public void can_register_without_providing_an_implementation()
		{
			var user = new User() { FiscalStability = FiscalStability.DirtFarmer };
			kernel.Register(
				Component.For<User>().Named("currentUser").Instance(user),
				Component.For<AbstractCarProviderFactory>().Named("AbstractCarProviderFactory"),
				Component.For<ICarProvider>()
					.Attribute("factoryId").Eq("AbstractCarProviderFactory")
					.Attribute("factoryCreate").Eq("Create")
				);
			Assert.IsInstanceOf(typeof(HondaProvider), kernel.Resolve<ICarProvider>());
		}

		private void RegisterComponentsImplemtedByFerrari(User user)
		{
			kernel.Register(
				Component.For<User>().Named("currentUser").Instance(user),
				Component.For<AbstractCarProviderFactory>().Named("AbstractCarProviderFactory"),
				Component.For<ICarProvider>()
					.ImplementedBy<FerrariProvider>()
					.Attribute("factoryId").Eq("AbstractCarProviderFactory")
					.Attribute("factoryCreate").Eq("Create")
				);
		}

        [Test]
        public void RegisterWithFluentFactory() 
        {
            var user = new User {FiscalStability = FiscalStability.DirtFarmer};
            kernel.Register(
                Component.For<User>().Instance(user),
                Component.For<AbstractCarProviderFactory>(),
                Component.For<ICarProvider>()
                    .UsingFactory((AbstractCarProviderFactory f) => f.Create(kernel.Resolve<User>()))
                );
            Assert.IsInstanceOf(typeof(HondaProvider), kernel.Resolve<ICarProvider>());
        }

        [Test]
        public void RegisterWithFactoryMethod() 
        {
            var user = new User { FiscalStability = FiscalStability.DirtFarmer };
            kernel.Register(
                Component.For<AbstractCarProviderFactory>(),
                Component.For<ICarProvider>()
                    .UsingFactoryMethod(() => new AbstractCarProviderFactory().Create(user))
                );
            Assert.IsInstanceOf(typeof(HondaProvider), kernel.Resolve<ICarProvider>());
        }

        [Test]
        public void RegisterWithFactoryMethodAndKernel() 
        {
            var user = new User { FiscalStability = FiscalStability.MrMoneyBags };
            kernel.Register(
                Component.For<User>().Instance(user),
                Component.For<AbstractCarProviderFactory>(),
                Component.For<ICarProvider>()
                    .UsingFactoryMethod(k => new AbstractCarProviderFactory().Create(k.Resolve<User>()))
                );
            Assert.IsInstanceOf(typeof(FerrariProvider), kernel.Resolve<ICarProvider>());
        }

        [Test]
        public void RegisterWithFactoryMethodNamed()
        {
            kernel.Register(
                Component.For<ICarProvider>()
                    .UsingFactoryMethod(() => new AbstractCarProviderFactory().Create(new User { FiscalStability = FiscalStability.MrMoneyBags }))
                    .Named("ferrariProvider"),
                Component.For<ICarProvider>()
                    .UsingFactoryMethod(() => new AbstractCarProviderFactory().Create(new User { FiscalStability = FiscalStability.DirtFarmer }))
                    .Named("hondaProvider")
                );

            Assert.IsInstanceOf(typeof(HondaProvider), kernel.Resolve<ICarProvider>("hondaProvider"));
            Assert.IsInstanceOf(typeof(FerrariProvider), kernel.Resolve<ICarProvider>("ferrariProvider"));
        }

        [Test]
        public void RegisterWithFactoryMethodAndKernelNamed()
        {
            kernel.Register(
                Component.For<ICarProvider>()
                    .UsingFactoryMethod(k => new AbstractCarProviderFactory().Create(new User { FiscalStability = FiscalStability.MrMoneyBags }))
                    .Named("ferrariProvider"),
                Component.For<ICarProvider>()
                    .UsingFactoryMethod(k => new AbstractCarProviderFactory().Create(new User { FiscalStability = FiscalStability.DirtFarmer }))
                    .Named("hondaProvider")
                );

            Assert.IsInstanceOf(typeof(HondaProvider), kernel.Resolve<ICarProvider>("hondaProvider"));
            Assert.IsInstanceOf(typeof(FerrariProvider), kernel.Resolve<ICarProvider>("ferrariProvider"));
        }

		[Test]
		public void RegisterWithFactoryMethodKernelAndContext()
		{
			kernel.Register(
				Component.For<User>().LifeStyle.Transient,
				Component.For<AbstractCarProviderFactory>(),
				Component.For<ICarProvider>()
					.UsingFactoryMethod((k, ctx) => new AbstractCarProviderFactory().Create(
						k.Resolve<User>(ctx.AdditionalParameters)))
				);
			Assert.IsInstanceOf(typeof(FerrariProvider), kernel.Resolve<ICarProvider>(
				new { FiscalStability = FiscalStability.MrMoneyBags }));
		}
	}
}
