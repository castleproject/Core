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

// $ANTLR 2.7.5 (20050128): "langparser.g" -> "WindsorLanguageParser.cs"$

    using antlr;
    using System.Text;
    using Castle.Model.Configuration;

namespace Castle.Windsor.Configuration.Interpreters.CastleLanguage
{
	public class windsorLanguageTokenTypes
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int IN = 4;
		public const int IMPORT = 5;
		public const int EOS = 6;
		public const int NEWLINE = 7;
		public const int COLON = 8;
		public const int EQUAL = 9;
		public const int INDENT = 10;
		public const int DEDENT = 11;
		public const int ID = 12;
		public const int DOT = 13;
		public const int STRING_LITERAL = 14;
		public const int DATA = 15;
		
	}
}
