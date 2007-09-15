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
	using System.Collections.Generic;
	using System.Collections.Specialized;

	/// <summary>
	/// Pendent
	/// </summary>
	public class BindingContext : AbstractBindingScope
	{
		private readonly ActionBinding action;
		private readonly ControllerBinder binder;
		private Stack<IBindingScope> scopes;
		private static readonly IExpressionEvaluator evaluator = new DataBindingEvaluator();

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingContext"/> class.
		/// </summary>
		/// <param name="binder">The binder.</param>
		/// <param name="action">The action.</param>
		internal BindingContext(ControllerBinder binder, ActionBinding action)
		{
			if (binder == null)
			{
				throw new ArgumentNullException("binder");
			}

			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			this.action = action;
			this.binder = binder;

			RegisterDefaultScopes();
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public ActionBinding Action
		{
			get { return action; }
		}

		/// <summary>
		/// Pushes the scope.
		/// </summary>
		/// <param name="scope">The scope.</param>
		public void PushScope(IBindingScope scope)
		{
			scopes.Push(scope);
		}

		/// <summary>
		/// Dispatches the action.
		/// </summary>
		public void DispatchAction()
		{
			DispatchAction(action.ActionName);
		}

		/// <summary>
		/// Dispatches the action.
		/// </summary>
		/// <param name="actionName">Name of the action.</param>
		public void DispatchAction(string actionName)
		{
			binder.DispatchAction(actionName, this);
		}

		/// <summary>
		/// Resolves the action arguments.
		/// </summary>
		/// <returns></returns>
		public IDictionary ResolveActionArguments()
		{
			HybridDictionary actionArgs = new HybridDictionary(true);

			foreach(IBindingScope scope in scopes)
			{
				scope.AddActionArguments(this, actionArgs);
			}

			return actionArgs;
		}

		/// <summary>
		/// Resolves the action arguments.
		/// </summary>
		/// <param name="actionArgs">The action args.</param>
		/// <param name="resolvedActionArgs">The resolved action args.</param>
		public void ResolveActionArguments(ActionArgumentCollection actionArgs,
		                                   IDictionary resolvedActionArgs)
		{
			if (actionArgs == null)
			{
				throw new ArgumentNullException("actionArgs");
			}

			if (resolvedActionArgs == null)
			{
				throw new ArgumentNullException("resolvedActionArgs");
			}

			foreach(ActionArgument actionArg in actionArgs)
			{
				if (actionArg.IsValid() &&
				    (!resolvedActionArgs.Contains(actionArg.Name)))
				{
					object argument = ResolveActionArgument(actionArg);

					if (argument != null)
					{
						resolvedActionArgs.Add(actionArg.Name, argument);
					}
				}
			}
		}

		/// <summary>
		/// Resolves the symbol.
		/// </summary>
		/// <param name="symbolName">Name of the symbol.</param>
		/// <param name="throwIfNotFound">if set to <c>true</c> [throw if not found].</param>
		/// <returns></returns>
		public object ResolveSymbol(string symbolName, bool throwIfNotFound)
		{
			object value = null;

			foreach(IBindingScope scope in scopes)
			{
				value = scope.ResolveSymbol(symbolName);
				if (value != null) break;
			}

			if (value == null && throwIfNotFound)
			{
				throw new InvalidOperationException("Unable to resolve symbol " + symbolName);
			}

			return value;
		}

		/// <summary>
		/// Adds the action arguments.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="resolvedActionArgs">The resolved action args.</param>
		protected override void AddActionArguments(BindingContext context,
		                                           IDictionary resolvedActionArgs)
		{
			context.ResolveActionArguments(action.ActionArguments, resolvedActionArgs);
		}

		/// <summary>
		/// Resolves the action argument.
		/// </summary>
		/// <param name="actionArg">The action arg.</param>
		/// <returns></returns>
		private object ResolveActionArgument(ActionArgument actionArg)
		{
			object value = actionArg.Value;

			if (value == null || value is string)
			{
				string expression = value as string;

				if (expression == null)
					expression = actionArg.Expression;

				value = evaluator.Evaluate(expression, this);
			}

			return value;
		}

		/// <summary>
		/// Registers the default scopes.
		/// </summary>
		private void RegisterDefaultScopes()
		{
			scopes = new Stack<IBindingScope>();
			scopes.Push(binder);
			scopes.Push(this);
		}
	}
}

#endif