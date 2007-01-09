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

#if (DOTNET2 && NET)

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;

	public class BindingContext : AbstractBindingScope
	{
		private readonly ActionBinding action;
		private readonly ControllerBinder binder;
		private Stack<IBindingScope> scopes;
		private static readonly IExpressionEvaluator evaluator = new DataBindingEvaluator();

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

		public ActionBinding Action
		{
			get { return action; }
		}

		public void PushScope(IBindingScope scope)
		{
			scopes.Push(scope);
		}

		public void DispatchAction()
		{
			DispatchAction(action.ActionName);
		}

		public void DispatchAction(string actionName)
		{
			binder.DispatchAction(actionName, this);
		}

		public IDictionary ResolveActionArguments()
		{
			HybridDictionary actionArgs = new HybridDictionary(true);

			foreach(IBindingScope scope in scopes)
			{
				scope.AddActionArguments(this, actionArgs);
			}

			return actionArgs;
		}

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

		protected override void AddActionArguments(BindingContext context,
		                                           IDictionary resolvedActionArgs)
		{
			context.ResolveActionArguments(action.ActionArguments, resolvedActionArgs);
		}

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

		private void RegisterDefaultScopes()
		{
			scopes = new Stack<IBindingScope>();
			scopes.Push(binder);
			scopes.Push(this);
		}
	}
}

#endif