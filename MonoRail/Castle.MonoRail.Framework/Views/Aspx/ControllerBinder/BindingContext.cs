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

#if DOTNET2

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Web.UI;

	public class BindingContext : AbstractBindingScope
	{
		private ActionBinding action;
		private ControllerBinder binder;
		private Stack<IBindingScope> scopes;

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
				bool ignoreErrors;
				string expression = value as string;

				if (expression == null)
					expression = actionArg.Expression;

				if (IsDataBindingExpression(ref expression, out ignoreErrors))
				{
					value = PerformDataBinding(expression, ignoreErrors);
				}
				else
				{
					value = expression;
				}
			}

			return value;
		}

		private bool IsDataBindingExpression(ref string expression, out bool ignoreErrors)
		{
			ignoreErrors = false;

			if (!string.IsNullOrEmpty(expression) && expression.StartsWith("$"))
			{
				int start = 1;
				bool isDataBind;

				if (expression.StartsWith("$!"))
				{
					++start;
					ignoreErrors = true;
					isDataBind = true;
				}
				else
				{
					isDataBind = !expression.StartsWith("$$");
				}

				expression = expression.Substring(start);

				return isDataBind;
			}

			return false;
		}

		private object PerformDataBinding(string expression, bool ignoreErrors)
		{
			object value;
			int index = expression.IndexOf('.');

			if (index > 0)
			{
				string symbolName = expression.Substring(0, index);

				value = ResolveSymbolFromScopes(symbolName);

				if (value != null)
				{
					expression = expression.Substring(index + 1);

					try
					{
						value = DataBinder.Eval(value, expression);
					}
					catch
					{
						value = null;
						if (!ignoreErrors) throw;
					}
				}
				else if (!ignoreErrors)
				{
					throw new InvalidOperationException("Unable to resolve symbol " + symbolName);
				}
			}
			else
			{
				value = ResolveSymbolFromScopes(expression);
			}

			return value;
		}

		private object ResolveSymbolFromScopes(string symbol)
		{
			object value = null;

			foreach(IBindingScope scope in scopes)
			{
				value = scope.ResolveSymbol(symbol);
				if (value != null) break;
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