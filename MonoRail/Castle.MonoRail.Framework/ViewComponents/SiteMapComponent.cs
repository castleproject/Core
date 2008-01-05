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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using System.Web;

	/// <summary>
	/// Provides access to sitemap content and renders it either using nested sections or a custom 
	/// specified view.
	/// </summary>
	/// 
	/// <example>
	/// The following example uses nvelocity view engine syntax.
	/// <code>
	/// <![CDATA[
	/// #blockcomponent(SiteMapComponent with "providerName=support")
	/// 
	/// #startparentnode
	///   $node.Name
	///   <ul>
	/// #end
	/// #endparentnode
	///   </ul>
	/// #end
	/// 
	/// #startnode
	///   <li> $node.Name
	///   <ul>
	/// #end
	/// #endnode
	///   </li> </ul>
	/// #end
	/// 
	/// #leafnode
	///   <a href="$node.url"> $node.Name </a>
	/// #end
	/// 
	/// #end
	/// ]]>
	/// </code>
	/// <para>
	/// Using a custom view:
	/// </para>
	/// <code>
	/// <![CDATA[
	/// #component(SiteMapComponent with "providerName=support" "customview=directoryview")
	/// ]]>
	/// </code>
	/// </example>
	/// 
	/// <remarks>
	/// To use this view component you just need to set up one or more sitemaps within your web app, 
	/// then specify the provider name (optionally) using the <see cref="ProviderName"/>
	/// 
	/// <para>
	/// You can use the following nested sections:
	/// Supported sections: <br/>
	/// <c>startparentnode</c>: invoked for SiteMapNode without a parent <br/>
	/// <c>endparentnode</c>: same thing but after visiting its children <br/>
	/// <c>startnode</c>: invoked for node with parents and children <br/>
	/// <c>endnode</c>: ditto.  <br/>
	/// <c>leafnode</c>: invoked for nodes without children <br/>
	/// </para>
	/// </remarks>
	[ViewComponentDetails("SiteMapComponent", Sections = "startparentnode,endparentnode,leafnode,startnode,endnode")]
	public class SiteMapComponent : ViewComponent
	{
		private SiteMapProvider targetProvider;

		private string providerName;
		private string customView;
		private IProviderAccessor providerAccessor = new AspNetProviderAccessor();

		/// <summary>
		/// Gets or sets the provider accessor.
		/// </summary>
		/// <value>The provider accessor.</value>
		public IProviderAccessor ProviderAccessor
		{
			get { return providerAccessor; }
			set { providerAccessor = value; }
		}

		/// <summary>
		/// Gets or sets the name of the sitemap provider to use.
		/// </summary>
		/// <value>The name of the provider.</value>
		[ViewComponentParam]
		public string ProviderName
		{
			get { return providerName; }
			set { providerName = value; }
		}

		/// <summary>
		/// Gets or sets the custom view to be used to render the root node.
		/// If not provided, the sections will be used.
		/// </summary>
		/// <value>The custom view.</value>
		[ViewComponentParam]
		public string CustomView
		{
			get { return customView; }
			set { customView = value; }
		}

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			if (providerName == null)
			{
				targetProvider = providerAccessor.DefaultProvider;
			}
			else
			{
				targetProvider = providerAccessor[providerName];
			}

			if (targetProvider == null)
			{
				throw new ViewComponentException("Could not obtain provider instance");
			}

			base.Initialize();
		}

		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			SiteMapNode node = targetProvider.RootNode;

			if (customView != null)
			{
				PropertyBag["node"] = node;
				RenderView(customView);
			}
			else
			{
				RecursiveRenderNode(node);
			}
		}

		private void RecursiveRenderNode(SiteMapNode node)
		{
			if (node == null) return;

			if (node.ParentNode == null)
			{
				PropertyBag["node"] = node;
				RenderSection("startparentnode");

				foreach(SiteMapNode child in node.ChildNodes)
				{
					RecursiveRenderNode(child);
				}

				PropertyBag["node"] = node;
				RenderSection("endparentnode");
			}
			else if (node.HasChildNodes)
			{
				PropertyBag["node"] = node;
				PropertyBag["parent"] = node.ParentNode;
				RenderSection("startnode");

				foreach (SiteMapNode child in node.ChildNodes)
				{
					RecursiveRenderNode(child);
				}

				PropertyBag["node"] = node;
				PropertyBag["parent"] = node.ParentNode;
				RenderSection("endnode");
			}
			else
			{
				PropertyBag["node"] = node;
				PropertyBag["parent"] = node.ParentNode;
				RenderSection("leafnode");
			}
		}

		/// <summary>
		/// Abstract the access to sitemap providers
		/// </summary>
		public interface IProviderAccessor
		{
			/// <summary>
			/// Gets the default provider.
			/// </summary>
			/// <value>The default provider.</value>
			SiteMapProvider DefaultProvider { get; }

			/// <summary>
			/// Gets the <see cref="System.Web.SiteMapProvider"/> with the specified provider name.
			/// </summary>
			/// <value></value>
			SiteMapProvider this[string providerName] { get; }
		}

		/// <summary>
		/// Accessor for the sitemap provider using the ASP.Net API
		/// </summary>
		public class AspNetProviderAccessor : IProviderAccessor
		{
			/// <summary>
			/// Gets the default provider.
			/// </summary>
			/// <value>The default provider.</value>
			public SiteMapProvider DefaultProvider
			{
				get { return SiteMap.Provider; }
			}

			/// <summary>
			/// Gets the <see cref="System.Web.SiteMapProvider"/> with the specified provider name.
			/// </summary>
			/// <value></value>
			public SiteMapProvider this[string providerName]
			{
				get { return SiteMap.Providers[providerName]; }
			}
		}
	}
}