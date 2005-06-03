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

namespace AspectSharp.Builder
{
	using System;
	using System.IO;

	using AspectSharp.Lang;
	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for AspectLanguageEngineBuilder.
	/// </summary>
	public class AspectLanguageEngineBuilder : AspectEngineBuilder
	{
		private TextReader _reader;

		/// <summary>
		/// Should be used cautiously by subclasses.
		/// </summary>
		protected AspectLanguageEngineBuilder()
		{
		}

		public AspectLanguageEngineBuilder(String contents) : this( new StringReader(contents))
		{
		}

		public AspectLanguageEngineBuilder(TextReader reader)
		{
			AssertUtil.ArgumentNotNull( reader, "reader" );
			_reader = reader;
		}

		public override AspectEngine Build()
		{
			Configuration = ParseContents();
			return base.Build();
		}

		protected TextReader Reader
		{
			get { return _reader; }
			set { _reader = value; }
		}

		protected EngineConfiguration ParseContents()
		{
			AspectLanguageLexer lexer = new AspectLanguageLexer(_reader);
			AspectParser parser = new AspectParser(lexer);

			try
			{
				return parser.Parse();
			}
			catch(antlr.RecognitionException e)
			{
				int line = e.getLine();
				int startColumn = e.getColumn();
				String filename = e.getFilename();
				LexicalInfo info = new LexicalInfo(filename, line, startColumn, -1);

				throw new BuilderException(info, e.Message);
			}
		}
	}
}
