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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	/// <summary>
	/// Exception handler that prevents further processment if the 
	/// request is local. This is useful if the next handler do something more 
	/// serious like sending an e-mail to admins/developers/cto/ceo.
	/// </summary>
	/// 
	/// <remarks>
	/// Inspired by
	/// http://sradack.blogspot.com/2007/07/monorail-exception-chaining.html
	/// </remarks>
	public class LocalExceptionFilterHandler : AbstractExceptionHandler
	{
		/// <summary>
		/// Prevents next handler from invoked if the request is local.
		/// </summary>
		/// <param name="context"></param>
		public override void Process(IEngineContext context)
		{
			if (context.Request.IsLocal)
			{
				return;
			}

			InvokeNext(context);
		}
	}
}
