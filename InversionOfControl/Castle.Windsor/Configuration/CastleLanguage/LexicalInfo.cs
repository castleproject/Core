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

namespace Castle.Windsor.Configuration.CastleLanguage
{
	using System;

	/// <summary>
	/// Summary description for LexicalInfo.
	/// </summary>
	[Serializable]
	public struct LexicalInfo
	{
		private static readonly LexicalInfo EMPTY = new LexicalInfo("", -1,-1,-1);

		private String _filename;
		private int _line;
		private int _startCol;
		private int _endCol;

		public LexicalInfo( String filename, int line, int startCol, int endCol )
		{
			_filename = filename;
			_line = line;
			_startCol = startCol;
			_endCol = endCol;
		}

		public static LexicalInfo Empty
		{
			get { return EMPTY; }
		}

		public String Filename
		{
			get { return _filename; }
		}

		public int Line
		{
			get { return _line; }
		}

		public int StartCol
		{
			get { return _startCol; }
		}

		public int EndCol
		{
			get { return _endCol; }
		}

	}
}
