// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// This <see cref="IThreadScopeInfo"/> implementation will first try to get the current scope from the current 
	/// request, and if not found, will use a thread lcoal scope.
	/// </summary>
	/// <remarks>
	/// This is used for scenarios where most of the you need per request scope, but you also does some work outside a 
	/// request (in a thread pool thread, for instnace).
	/// </remarks>
	public class HybridWebThreadScopeInfo : AbstractThreadScopeInfo
	{
		const string ActiveRecordCurrentStack = "activerecord.currentstack";

		static readonly Object syncObject = new Object();
		
		[ThreadStatic] static Stack stack;

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
					lock (syncObject)
					{
						if (stack == null)
						{
							stack = new Stack();
						}

						return stack;
					}
				}

				Stack contextstack = (Stack) current.Items[ActiveRecordCurrentStack];

				if (contextstack == null)
				{
					contextstack = new Stack();

					current.Items[ActiveRecordCurrentStack] = contextstack;
				}

				return contextstack;
			}
		}
	}
}
