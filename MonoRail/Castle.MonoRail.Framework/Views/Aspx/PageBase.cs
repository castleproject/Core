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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System.Collections;
	using Helpers;
	using Page=System.Web.UI.Page;

	/// <summary>
	/// Pendent
	/// </summary>
	public class PageBase : Page
	{
		private IControllerContext controllerContext;
		private FormHelper form;
		private UrlHelper url;
		private TextHelper text;

		/// <summary>
		/// Gets or sets the controller context.
		/// </summary>
		/// <value>The controller context.</value>
		public IControllerContext ControllerContext
		{
			get { return controllerContext; }
			set { controllerContext = value; }
		}

		/// <summary>
		/// Gets the property bag.
		/// </summary>
		/// <value>The property bag.</value>
		public IDictionary PropertyBag
		{
			get { return controllerContext.PropertyBag; }
		}

		/// <summary>
		/// Gets or sets the form helper.
		/// </summary>
		/// <value>The form helper.</value>
		public FormHelper FormHelper
		{
			get { return form; }
			set { form = value; }
		}

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public UrlHelper UrlHelper
		{
			get { return url; }
			set { url = value; }
		}

		/// <summary>
		/// Gets or sets the text helper.
		/// </summary>
		/// <value>The text helper.</value>
		public TextHelper TextHelper
		{
			get { return text; }
			set { text = value; }
		}
	}
}
