// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Web;
	
	public class RequestValidatorFilter : IFilter
	{
		public RequestValidatorFilter()
		{
		}

		#region IFilter Members

		public bool Perform(Castle.MonoRail.Framework.ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			try
			{

				context.Request.ValidateInput();
				
				object honeyPot = null;
				
				honeyPot = context.Request.Form;
				honeyPot = context.Request.QueryString;
			}
			catch (HttpRequestValidationException e)
			{
				context.Flash["validationError"] = context.Server.HtmlEncode(e.Message);
			}

			return true;
		}

		#endregion
	}
}
