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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using System.Collections;
	using Helpers;

	/// <summary>
	/// Pendent
	/// </summary>
	public abstract class AbstractPaginationViewComponent : ViewComponent
	{
		private string paginatefunction;
		private object urlParam;
		private IPaginatedPage page;
		private UrlPartsBuilder urlPartsBuilder;
		private bool usePathInfo;
		private string pageParamName = "page";

		/// <summary>
		/// Gets or sets the paginated page instance.
		/// </summary>
		/// <value>The page.</value>
		[ViewComponentParam(Required = true)]
		public IPaginatedPage Page
		{
			get { return page; }
			set { page = value; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <value>The name of the page param.</value>
		public string PageParamName
		{
			get { return pageParamName; }
			set { pageParamName = value; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		[ViewComponentParam]
		public bool UsePathInfo
		{
			get { return usePathInfo; }
			set { usePathInfo = value; }
		}

		/// <summary>
		/// Gets or sets the paginate function name.
		/// <para>
		/// A paginate function is a javascript fuction 
		/// that receives the page index as the only argument. 
		/// </para>
		/// </summary>
		/// <value>The paginate function.</value>
		[ViewComponentParam]
		public string PaginateFunction
		{
			get { return paginatefunction; }
			set { paginatefunction = value; }
		}

		/// <summary>
		/// Gets or sets the URL to be used when generating links
		/// </summary>
		/// <value>The URL.</value>
		[ViewComponentParam]
		public object Url
		{
			get { return urlParam; }
			set { urlParam = value; }
		}

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			if (page == null)
			{
				throw new ViewComponentException("The DiggStylePagination requires a view component " +
					"parameter named 'page' which should contain 'IPaginatedPage' instance");
			}

			// So when we render the blocks, the user might access the page
			PropertyBag["page"] = page;

			CreateUrlPartBuilder();
		}

		private void CreateUrlPartBuilder()
		{			
			IDictionary urlParams = urlParam as IDictionary;
			
			if (urlParams != null)
			{				
				urlParams["encode"] = "true";

				IUrlBuilder urlBuilder = RailsContext.GetService<IUrlBuilder>();
				urlPartsBuilder = urlBuilder.CreateUrlPartsBuilder(RailsContext.UrlInfo, urlParams);
			}
			else
			{
				if (urlParam != null)
				{
					urlPartsBuilder = UrlPartsBuilder.Parse(urlParam.ToString());
				}
				else
				{
					urlPartsBuilder = new UrlPartsBuilder(RailsContext.Request.FilePath);
				}
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="pageIndex">The page index.</param>
		/// <returns></returns>
		protected string CreateUrlForPage(int pageIndex)
		{
			if (usePathInfo)
			{
				urlPartsBuilder.PathInfoDict[pageParamName] = pageIndex.ToString();
			}
			else
			{
				urlPartsBuilder.QueryString.Remove(pageParamName);
				urlPartsBuilder.QueryString[pageParamName] = pageIndex.ToString();
			}

			return urlPartsBuilder.BuildPath();
		}
	}
}
