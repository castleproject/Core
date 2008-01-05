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

namespace Castle.ActiveRecord.Framework.Scopes
{
	using System;
	using System.Collections;
	using System.Runtime.CompilerServices;
	using System.Web;

	/// <summary>
	/// This <see cref="IThreadScopeInfo"/> implementation will first get the current scope from the current 
	/// request, thus implementing a Session Per Request pattern.
	/// </summary>
	public class WebThreadScopeInfo : AbstractThreadScopeInfo, IWebThreadScopeInfo
	{
		const string ActiveRecordCurrentStack = "activerecord.currentstack";

		/// <summary>
		/// Gets the current stack.
		/// </summary>
		/// <value>The current stack.</value>
		public override Stack CurrentStack
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				HttpContext current = HttpContext.Current;

				if (current == null)
				{
					String message = "WebThreadScopeInfo: Could not access HttpContext.Current";
					
					throw new ScopeMachineryException(message);
				}

				Stack stack = (Stack)current.Items[ActiveRecordCurrentStack];

				if (stack == null)
				{
					stack = new Stack();

					current.Items[ActiveRecordCurrentStack] = stack;
				}

				return stack;
			}
		}
	}
}
