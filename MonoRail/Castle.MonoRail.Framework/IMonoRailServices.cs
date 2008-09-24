// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Castle.Core;
	using Castle.MonoRail.Framework.Services;
	using Resources;
	using Services.AjaxProxyGenerator;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IMonoRailServices : IServiceProviderEx
	{
		/// <summary>
		/// Gets or sets the URL tokenizer.
		/// </summary>
		/// <value>The URL tokenizer.</value>
		IUrlTokenizer UrlTokenizer { get; set; }

		/// <summary>
		/// Gets or sets the URL builder.
		/// </summary>
		/// <value>The URL builder.</value>
		IUrlBuilder UrlBuilder { get; set; }

		/// <summary>
		/// Gets or sets the cache provider.
		/// </summary>
		/// <value>The cache provider.</value>
		ICacheProvider CacheProvider { get; set; }

		/// <summary>
		/// Gets or sets the engine context factory.
		/// </summary>
		/// <value>The engine context factory.</value>
		IEngineContextFactory EngineContextFactory { get; set; }

		/// <summary>
		/// Gets or sets the controller factory.
		/// </summary>
		/// <value>The controller factory.</value>
		IControllerFactory ControllerFactory { get; set; }

		/// <summary>
		/// Gets or sets the controller context factory.
		/// </summary>
		/// <value>The controller context factory.</value>
		IControllerContextFactory ControllerContextFactory { get; set; }

		/// <summary>
		/// Gets or sets the controller tree.
		/// </summary>
		/// <value>The controller tree.</value>
		IControllerTree ControllerTree { get; set; }

		/// <summary>
		/// Gets or sets the view source loader.
		/// </summary>
		/// <value>The view source loader.</value>
		IViewSourceLoader ViewSourceLoader { get; set; }

		/// <summary>
		/// Gets or sets the filter factory.
		/// </summary>
		/// <value>The filter factory.</value>
		IFilterFactory FilterFactory { get; set; }

		/// <summary>
		/// Gets or sets the controller descriptor provider.
		/// </summary>
		/// <value>The controller descriptor provider.</value>
		IControllerDescriptorProvider ControllerDescriptorProvider { get; set; }

		/// <summary>
		/// Gets or sets the view engine manager.
		/// </summary>
		/// <value>The view engine manager.</value>
		IViewEngineManager ViewEngineManager { get; set; }

		/// <summary>
		/// Gets or sets the validator registry.
		/// </summary>
		/// <value>The validator registry.</value>
		IValidatorRegistry ValidatorRegistry { get; set; }

		/// <summary>
		/// Gets or sets the action selector.
		/// </summary>
		/// <value>The action selector.</value>
		IActionSelector ActionSelector { get; set; }

		/// <summary>
		/// Gets or sets the scaffold support.
		/// </summary>
		/// <value>The scaffold support.</value>
		IScaffoldingSupport ScaffoldingSupport { get; set; }

		/// <summary>
		/// Gets or sets the JSON serializer.
		/// </summary>
		/// <value>The JSON serializer.</value>
		IJSONSerializer JSONSerializer { get; set; }

		/// <summary>
		/// Gets or sets the static resource registry service.
		/// </summary>
		/// <value>The static resource registry.</value>
		IStaticResourceRegistry StaticResourceRegistry { get; set; }

		/// <summary>
		/// Gets or sets the email template service.
		/// </summary>
		/// <value>The email template service.</value>
		IEmailTemplateService EmailTemplateService { get; set; }

		/// <summary>
		/// Gets or sets the email sender.
		/// </summary>
		/// <value>The email sender.</value>
		IEmailSender EmailSender { get; set; }

		/// <summary>
		/// Gets or sets the resource factory.
		/// </summary>
		/// <value>The resource factory.</value>
		IResourceFactory ResourceFactory { get; set; }

		/// <summary>
		/// Gets or sets the transformfilter factory.
		/// </summary>
		/// <value>The resource factory.</value>
		ITransformFilterFactory TransformFilterFactory { get; set; }

		/// <summary>
		/// Gets or sets the service initializer.
		/// </summary>
		/// <value>The service initializer.</value>
		IServiceInitializer ServiceInitializer { get; set; }

		/// <summary>
		/// Gets or sets the helper factory.
		/// </summary>
		/// <value>The helper factory.</value>
		IHelperFactory HelperFactory { get; set; }

		/// <summary>
		/// Gets or sets the extension manager.
		/// </summary>
		/// <value>The extension manager.</value>
		ExtensionManager ExtensionManager { get; set; }

		/// <summary>
		/// Gets or sets the dynamic action provider factory.
		/// </summary>
		/// <value>The dynamic action provider factory.</value>
		IDynamicActionProviderFactory DynamicActionProviderFactory { get; set; }

		/// <summary>
		/// Gets or sets the ajax proxy generator.
		/// </summary>
		/// <value>The ajax proxy generator.</value>
		IAjaxProxyGenerator AjaxProxyGenerator { get; set; }
	}
}
