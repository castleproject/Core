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

#if NET

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;

	/// <summary>
	/// Pendent
	/// </summary>
	public abstract class AbstractEventScope : AbstractBindingScope
	{
		private object source;
		private EventArgs eventArgs;
		private BindingContext context;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractEventScope"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		protected AbstractEventScope(BindingContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Gets the context.
		/// </summary>
		/// <value>The context.</value>
		protected BindingContext Context
		{
			get { return context;  }
		}

		/// <summary>
		/// Dispatches the action.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void DispatchAction(object source, EventArgs e)
		{
			PushEventScope(source, e);
			context.DispatchAction();
		}

		/// <summary>
		/// Dispatches the action.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <param name="action">The action.</param>
		protected void DispatchAction(object source, EventArgs e, string action)
		{
			PushEventScope(source, e);
			context.DispatchAction(action);
		}

		/// <summary>
		/// Resolves the symbol.
		/// </summary>
		/// <param name="symbol">The symbol.</param>
		/// <returns></returns>
		protected override object ResolveSymbol(string symbol)
		{
			switch(symbol)
			{
				case "this":
					return source;

				case "event":
					return eventArgs;
			}

			return null;
		}

		/// <summary>
		/// Pushes the event scope.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void PushEventScope(object source, EventArgs e)
		{
			this.source = source;
			this.eventArgs = e;
			context.PushScope(this);
		}
	}
}

#endif
