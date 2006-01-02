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

namespace AspectSharp.Lang.Steps
{
	using System;

	using AspectSharp.Lang.AST;
	using AspectSharp.Lang.AST.Visitors;

	/// <summary>
	/// Summary description for AbstractVisitorStep.
	/// </summary>
	public class AbstractVisitorStep : DepthFirstVisitor, IStep
	{
		private InternalStep _step = new InternalStep();

		public AbstractVisitorStep()
		{
		}

		#region IStep Members

		protected void Init(Context context)
		{
			_step.Init(context);
		}

		public virtual void Process(Context context, EngineConfiguration configuration)
		{
			_step.Process(context, configuration);
		}

		public IStep Next
		{
			get { return _step.Next; }
			set { _step.Next = value; }
		}

		public Context Context
		{
			get { return _step.Context; }
		}

		#endregion

		internal class InternalStep : AbstractStep
		{
			public new void Init(Context context)
			{
				base.Init(context);
			}

			public new Context Context
			{
				get { return base.Context; }
			}

		}
	}

}