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

namespace Castle.MonoRail.Framework.Filters
{
	using System.Web;
	
	/// <summary>
	/// Forces ASP.Net to perform a more complete request validation
	/// </summary>
	public class RequestValidatorFilter : IFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestValidatorFilter"/> class.
		/// </summary>
		public RequestValidatorFilter()
		{
		}

		/// <summary>
		/// Implementors should perform they filter logic and
		/// return <c>true</c> if the action should be processed.
		/// </summary>
		/// <param name="exec">When this filter is being invoked</param>
		/// <param name="context">Current context</param>
		/// <param name="controller">The controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// 	<c>true</c> if the action
		/// should be invoked, otherwise <c>false</c>
		/// </returns>
		public bool Perform(ExecuteEnum exec, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			try
			{
				context.Request.ValidateInput();
				
				// Yeah compiler, this is an assignment
				object dummy = context.Request.Form;
				dummy = context.Request.QueryString;
			}
			catch(HttpRequestValidationException e)
			{
				context.Flash["validationError"] = context.Server.HtmlEncode(e.Message);
			}

			return true;
		}
	}
}
