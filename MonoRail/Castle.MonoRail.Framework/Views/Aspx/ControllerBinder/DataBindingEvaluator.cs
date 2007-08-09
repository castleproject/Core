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
	using System.Web.UI;

	public class DataBindingEvaluator : IExpressionEvaluator
	{
		private static readonly char[] indexExprStartChars = { '[', '(' };
		private static readonly char[] indexExprEndChars = { ']', ')' };

		public object Evaluate(string expression, BindingContext context)
		{
			bool ignoreErrors;

			if (IsDataBindingExpression(ref expression, out ignoreErrors))
			{
				return PerformDataBinding(expression, context, ignoreErrors);
			}

			return expression;
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

		private object PerformDataBinding(string expression, BindingContext context, bool ignoreErrors)
		{
			object value;
			int index = expression.IndexOf('.');

			if (index > 0)
			{
				string symbolName = expression.Substring(0, index).Trim();

				value = context.ResolveSymbol(symbolName, !ignoreErrors);

				if (value != null)
				{
					expression = expression.Substring(index + 1).Trim();

					try
					{
						value = PerformDataBinding(value, expression, context);
					}
					catch
					{
						if (!ignoreErrors) throw;
						value = null;
					}
				}
			}
			else
			{
				value = context.ResolveSymbol(expression, !ignoreErrors);
			}

			return value;
		}

		private object PerformDataBinding(object target, string expression, BindingContext context)
		{
			if (string.IsNullOrEmpty(expression)) return target;

			int indexStart = expression.IndexOfAny(indexExprStartChars);

			if (indexStart < 0)
			{
				int formatIndex = expression.LastIndexOf(':');

				if (formatIndex < 0)
				{
					return DataBinder.Eval(target, expression);
				}
				else
				{
					string format = expression.Substring(formatIndex + 1);
					expression = expression.Substring(0, formatIndex);
					return DataBinder.Eval(target, expression, format);
				}
			}
			else
			{
				return PerformIndexDataBinding(target, expression, context, indexStart);
			}
		}

		private object PerformIndexDataBinding(object target, string expression,
		                                       BindingContext context, int start)
		{
			int end;

			string indexProperty = expression.Substring(0, start).Trim();

			if (!string.IsNullOrEmpty(indexProperty))
			{
				target = DataBinder.Eval(target, indexProperty);
			}

			string indexExpression = ExtractIndexerExpression(expression, start, out end);

			object indexer = Evaluate(indexExpression, context);

			indexExpression = string.Format("{0}{1}{2}", indexExprStartChars[0],
			                                indexer, indexExprEndChars[0]);

			target = DataBinder.Eval(target, indexExpression);

			expression = expression.Substring(end + 1).TrimStart('.');

			return PerformDataBinding(target, expression, context);
		}

		private string ExtractIndexerExpression(string expression, int start, out int end)
		{
			end = start;
			int counter = 0;

			for(int i = start + 1; i < expression.Length; ++i)
			{
				if (Array.Exists(indexExprEndChars,
				                 delegate(char c) { return expression[i] == c; }))
				{
					if (counter == 0)
					{
						end = i;
						expression = expression.Substring(start + 1, i - start - 1);
						return expression;
					}
					else
					{
						--counter;
					}
				}
				else if (Array.Exists(indexExprStartChars,
				                      delegate(char c) { return expression[i] == c; }))
				{
					++counter;
				}
			}

			throw new ArgumentException("Mismatched index expression");
		}
	}
}

#endif