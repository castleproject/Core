namespace Castle.MonoRail.Framework.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Components.Common.EmailSender;
	using Components.Common.EmailSender.Mock;
	using Components.Validator;
	using Container;
	using Framework.Providers;
	using Framework.Resources;
	using Framework.Services;
	using Framework.Services.AjaxProxyGenerator;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Test;

	[TestFixture]
	public class DefaultMonoRailContainerTestCase
	{
		private DefaultMonoRailContainer _defaultMonoRailContainer;
		private readonly List<Type> _serviceTypes = new List<Type>();

		[TestFixtureSetUp]
		public void Init()
		{
			_defaultMonoRailContainer = new DefaultMonoRailContainer();
			_defaultMonoRailContainer.AddService<IUrlTokenizer>(new DefaultUrlTokenizer());
			_defaultMonoRailContainer.AddService<IUrlBuilder>(new DefaultUrlBuilder());
			_defaultMonoRailContainer.AddService<ICacheProvider>(new DefaultCacheProvider());
			_defaultMonoRailContainer.AddService<IEngineContextFactory>(new DefaultEngineContextFactory());
			_defaultMonoRailContainer.AddService<IControllerFactory>(new DefaultControllerFactory());
			_defaultMonoRailContainer.AddService<IControllerContextFactory>(new DefaultControllerContextFactory());
			_defaultMonoRailContainer.AddService<IControllerTree>(new DefaultControllerTree());
			_defaultMonoRailContainer.AddService<IViewSourceLoader>(new FileAssemblyViewSourceLoader());
			_defaultMonoRailContainer.AddService<IFilterFactory>(new DefaultFilterFactory());
			_defaultMonoRailContainer.AddService<IControllerDescriptorProvider>(new DefaultControllerDescriptorProvider());
			_defaultMonoRailContainer.AddService<IViewEngineManager>(new DefaultViewEngineManager());
			_defaultMonoRailContainer.AddService<IValidatorRegistry>(new CachedValidationRegistry());
			_defaultMonoRailContainer.AddService<IActionSelector>(new DefaultActionSelector());
			_defaultMonoRailContainer.AddService<IScaffoldingSupport>(new StubScaffoldingSupport());
			_defaultMonoRailContainer.AddService<IJSONSerializer>(new NewtonsoftJSONSerializer());
			_defaultMonoRailContainer.AddService<IStaticResourceRegistry>(new DefaultStaticResourceRegistry());
			_defaultMonoRailContainer.AddService<IEmailTemplateService>(new StubEmailTemplateService(new StubEngineContext()));
			_defaultMonoRailContainer.AddService<IEmailSender>(new StubEmailSender());
			_defaultMonoRailContainer.AddService<IResourceFactory>(new DefaultResourceFactory());
			_defaultMonoRailContainer.AddService<ITransformFilterFactory>(new DefaultTransformFilterFactory());
			_defaultMonoRailContainer.AddService<IHelperFactory>(new DefaultHelperFactory());
			_defaultMonoRailContainer.AddService<IServiceInitializer>(new DefaultServiceInitializer());
			_defaultMonoRailContainer.AddService<IDynamicActionProviderFactory>(new DefaultDynamicActionProviderFactory());
			_defaultMonoRailContainer.AddService<IAjaxProxyGenerator>(new PrototypeAjaxProxyGenerator());
            
			_serviceTypes.Add(typeof(IUrlTokenizer));
			_serviceTypes.Add(typeof(IUrlBuilder));
			_serviceTypes.Add(typeof(ICacheProvider));
			_serviceTypes.Add(typeof(IEngineContextFactory));
			_serviceTypes.Add(typeof(IControllerFactory));
			_serviceTypes.Add(typeof(IControllerContextFactory));
			_serviceTypes.Add(typeof(IControllerTree));
			_serviceTypes.Add(typeof(IViewSourceLoader));
			_serviceTypes.Add(typeof(IFilterFactory));
			_serviceTypes.Add(typeof(IControllerDescriptorProvider));
			_serviceTypes.Add(typeof(IViewEngineManager));
			_serviceTypes.Add(typeof(IValidatorRegistry));
			_serviceTypes.Add(typeof(IActionSelector));
			_serviceTypes.Add(typeof(IScaffoldingSupport));
			_serviceTypes.Add(typeof(IJSONSerializer));
			_serviceTypes.Add(typeof(IStaticResourceRegistry));
			_serviceTypes.Add(typeof(IEmailTemplateService));
			_serviceTypes.Add(typeof(IEmailSender));
			_serviceTypes.Add(typeof(IResourceFactory));
			_serviceTypes.Add(typeof(ITransformFilterFactory));
			_serviceTypes.Add(typeof(IHelperFactory));
			_serviceTypes.Add(typeof(IServiceInitializer));
			_serviceTypes.Add(typeof(IDynamicActionProviderFactory));
			_serviceTypes.Add(typeof(IAjaxProxyGenerator));
		}

		[Test]
		public void CacheOverrideTest()
		{
			Type containerType = _defaultMonoRailContainer.GetType();

			foreach(Type serviceType in _serviceTypes)
			{
				// check if it was registered
				Assert.IsTrue(_defaultMonoRailContainer.HasService(serviceType), string.Format("expected registered service of type {0}", serviceType));

				// get the name of the service property by stripping the I
				string propertyName = serviceType.Name.Substring(1);

				// get the service via the property
				PropertyInfo propertyInfo = containerType.GetProperty(propertyName);
				Assert.IsNotNull(propertyInfo, string.Format("Property {0} not found on DefaultMonoRailsContainer", propertyName));

				object serviceInstance = propertyInfo.GetValue(_defaultMonoRailContainer, null);
				Assert.IsNotNull(serviceInstance, string.Format("Expected a registered service of type {0}", serviceType));

				// check if the underlying hastable has the same value as the property
				Assert.AreSame(serviceInstance, _defaultMonoRailContainer.GetService(serviceType));

				// generate a stub and call the setter
				object stub = MockRepository.GenerateStub(serviceType);
				propertyInfo.SetValue(_defaultMonoRailContainer, stub, null);

				// re-get the value
				serviceInstance = propertyInfo.GetValue(_defaultMonoRailContainer, null);

				// compare to the stub to make sure the cache has been overridden
				Assert.AreSame(serviceInstance, _defaultMonoRailContainer.GetService(serviceType), string.Format("Cache bug found for service {0}", serviceType));
			}
		}
	}
}