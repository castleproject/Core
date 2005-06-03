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

namespace AspectSharp.Lang.Steps
{
	using System;
	using System.ComponentModel;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for AbstractStep.
	/// </summary>
	public abstract class AbstractStep : IStep
	{
		private IStep _next;
		private Context _context;

		public AbstractStep()
		{
		}

		#region IStep Members

		public virtual void Process(Context context, EngineConfiguration configuration)
		{
			if (Next != null)
			{
				Next.Process(context, configuration);
			}
		}

		public IStep Next
		{
			get { return _next; }
			set { _next = value; }
		}

		#endregion

		protected virtual void Init(Context context)
		{
			_context = context;
		}

		protected virtual Context Context
		{
			get { return _context; }
		}

		protected void RaiseErrorEvent(LexicalInfo info, String message)
		{
			Context.RaiseErrorEvent(info, message);
		}
	}
}