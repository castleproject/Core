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

namespace Castle.Rook.Compiler.Services.Default
{
	using System;
	using System.IO;

	using Castle.Rook.Compiler.Parser;
	using Castle.Rook.Compiler.AST;


	public class RookParser : IParser
	{
		public IErrorReport errorReport;

		public RookParser(IErrorReport errorReport)
		{
			this.errorReport = errorReport;
		}

		#region IParser Members

		public SourceUnit Parse(CompilationUnit unit, TextReader reader)
		{
			RookLexer lexer = new RookLexer(reader);

			RookBaseParser parser = new RookBaseParser(lexer);
			parser.ErrorReport = errorReport;

			return parser.sourceUnit(unit);
		}

		public SourceUnit Parse(CompilationUnit unit, String contents)
		{
			return Parse(unit, new StringReader(contents));
		}

		/// <summary>
		/// Handy for the test cases
		/// </summary>
		public SourceUnit Parse(String contents)
		{
			CompilationUnit unit = new CompilationUnit();

			return Parse(unit, new StringReader(contents));
		}

		#endregion
	}
}
