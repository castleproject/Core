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

	/// <summary>
	/// Default Error Report outputs to Console
	/// </summary>
	public class ErrorReport : IErrorReport
	{
		private int errorCount, warningCount;

		public ErrorReport()
		{
		}

		#region IErrorReport Members

		public void Error(String filename, LexicalPosition pos, String contents, params object[] args)
		{
			errorCount++;

			if (pos.Column == 0)
			{
				ErrorWriter.Write("{0}:{1}\terror:  ", filename, pos.Line);
			}
			else
			{
				ErrorWriter.Write("{0}:{1},{2}\terror:  ", filename, pos.Line, pos.Column);
			}

			ErrorWriter.WriteLine(contents, args);
		}

		public void Error(String contents, params object[] args)
		{
			errorCount++;

			ErrorWriter.Write("compiler error:  ");
			ErrorWriter.WriteLine(contents, args);
		}

		public void Warning(String filename, LexicalPosition pos, Severity severity, String contents, params object[] args)
		{
			warningCount++;

			if (pos.Column == 0)
			{
				OutWriter.Write("{0}:{1}\twarning:  ", filename, pos.Line);
			}
			else
			{
				OutWriter.Write("{0}:{1},{2}\twarning:  ", filename, pos.Line, pos.Column);
			}

			OutWriter.WriteLine(contents, args);
		}

		public void Warning(String contents, params object[] args)
		{
			warningCount++;

			OutWriter.Write("warning:  ");
			OutWriter.WriteLine(contents, args);
		}

		public bool HasErrors
		{
			get { return errorCount != 0; }
		}

		public bool HasWarnings
		{
			get { return warningCount != 0; }
		}

		#endregion

		protected virtual TextWriter OutWriter
		{
			get { return Console.Out; }
		}

		protected virtual TextWriter ErrorWriter
		{
			get { return Console.Error; }
		}
	}
}
