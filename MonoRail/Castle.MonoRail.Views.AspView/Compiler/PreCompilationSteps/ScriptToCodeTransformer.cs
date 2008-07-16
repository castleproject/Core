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

namespace Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps
{
	using System;
	using System.Text;
	using System.Text.RegularExpressions;
	using StatementProcessors;

	public class ScriptToCodeTransformer
	{
		/// <summary>
		/// Code Generators - would transform &lt;% %&gt; blocks to actual code.
		/// <remarks>
		/// The order is important
		/// </remarks>
		/// </summary>
		static readonly IStatementProcessor[] statementProcessors = new IStatementProcessor[]
			{
				new EqualsAndParenthesisStatementProcessor(),
				new EqualsStatementProcessor(),
				new SharpStatementProcessor()
			};

		public string Transform(string mixed)
		{
			StringBuilder sb = new StringBuilder(mixed.Length);
			foreach (Match match in Internal.RegularExpressions.Script.Matches(mixed))
			{
				string markup = match.Groups["markup"].Value;
				string statement = match.Groups["statement"].Value.Trim();
				if (!string.IsNullOrEmpty(markup))
					AppendMarkup(sb, markup);
				if (!string.IsNullOrEmpty(statement))
					AppendStatement(sb, statement);
			}

			return sb.ToString();
		}

		private static void AppendMarkup(StringBuilder sb, string markup)
		{
			markup = markup.Replace("\"", "\"\"");
			sb.AppendFormat("Output(@\"{0}\");", markup)
				.AppendLine();
		}

		private static void AppendStatement(StringBuilder sb, string statement)
		{
			string code = GetCodeFrom(statement);
			sb.AppendLine(code);
		}

		private static string GetCodeFrom(string statement)
		{
			IStatementProcessor processor = GetProcessorFor(statement);
			if (processor == null)
				return statement;

			return processor.GetInfoFor(statement).ToCode();
		}

		private static IStatementProcessor GetProcessorFor(string statement)
		{
			return Array.Find(statementProcessors, delegate(IStatementProcessor processor)
			{
				return processor.CanHandle(statement);
			});
		}
	}
}
