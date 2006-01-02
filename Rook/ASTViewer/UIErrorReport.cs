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

namespace ASTViewer
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler;
	using Castle.Rook.Compiler.Services;


	public class UIErrorReport : IErrorReport
	{
		int errors;
		int warnings;
		int disabled;
		ArrayList list = new ArrayList();

		public UIErrorReport()
		{
		}

		public void Error(String filename, LexicalPosition pos, String contents, params object[] args)
		{
			if (disabled != 0) return;

			errors++;

			list.Add( new ErrorEntry(true, filename, pos, String.Format(contents, args) ) );
		}

		public void Error(String contents, params object[] args)
		{
			if (disabled != 0) return;

			errors++;

			list.Add( new ErrorEntry(true, "", new LexicalPosition(), String.Format(contents, args) ) );
		}

		public void Warning(String filename, LexicalPosition pos, Severity severity, String contents, params object[] args)
		{
			if (disabled != 0) return;

			warnings++;

			list.Add( new ErrorEntry(false, "", new LexicalPosition(), String.Format(contents, args) ) );
		}

		public void Warning(String contents, params object[] args)
		{
			if (disabled != 0) return;

			warnings++;

			list.Add( new ErrorEntry(false, "", new LexicalPosition(), String.Format(contents, args) ) );
		}

		public bool HasErrors
		{
			get { return errors != 0; }
		}

		public bool HasWarnings
		{
			get { return warnings != 0; }
		}

		public void Enable()
		{
			disabled--;
		}

		public void Disable()
		{
			disabled++;
		}

		public ArrayList List
		{
			get { return list; }
		}
	}

	public class ErrorEntry
	{
		private readonly string filename;
		private readonly bool isError;
		private readonly LexicalPosition pos;
		private readonly string contents;

		public ErrorEntry(bool error, string filename, LexicalPosition pos, string contents)
		{
			this.filename = filename;
			this.isError = error;
			this.pos = pos;
			this.contents = contents;
		}

		public bool IsError
		{
			get { return isError; }
		}

		public string Filename
		{
			get { return filename; }
		}

		public LexicalPosition Pos
		{
			get { return pos; }
		}

		public string Contents
		{
			get { return contents; }
		}
	}
}
