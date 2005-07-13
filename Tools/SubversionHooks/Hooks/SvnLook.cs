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

namespace Castle.SvnHooks
{
	using System;
	using System.Diagnostics;
	/// <summary>
	/// Summary description for SvnLook.
	/// </summary>
	public sealed class SvnLook
	{
		public SvnLook(String svnLookPath, String repositoryPath)
		{
			this.SvnLookPath = svnLookPath;
			this.RepositoryPath = repositoryPath;
		}

		public Process Execute(SvnLookCommand command, int revision, String arguments)
		{
			Trace.WriteLine(String.Format("Excuting {0} with revision {1}", command, revision));
			switch (command)
			{
				case SvnLookCommand.Lock:
					throw new InvalidOperationException("Lock cannot be called with a revision number");
				case SvnLookCommand.UUID:
					throw new InvalidOperationException("UUID cannot be called with a revision number");
				case SvnLookCommand.Youngest:
					throw new InvalidOperationException("Youngest cannot be called with a revision number");
				default: break;
			}

			arguments = String.Concat("-r ", revision, " ", arguments);

			return Execute(command, arguments);
		
		}
		public Process Execute(SvnLookCommand command, Transaction transaction, String arguments)
		{
			Trace.WriteLine(String.Format("Excuting {0} with transaction {1}", command, transaction));
			switch (command)
			{
				case SvnLookCommand.History:
					throw new InvalidOperationException("History cannot be called with a transaction number");
				case SvnLookCommand.Lock:
					throw new InvalidOperationException("Lock cannot be called with a transaction number");
				case SvnLookCommand.UUID:
					throw new InvalidOperationException("UUID cannot be called with a transaction number");
				case SvnLookCommand.Youngest:
					throw new InvalidOperationException("Youngest cannot be called with a transaction number");
				default: break;
			}

			arguments = String.Concat("-t ", transaction, " ", arguments);

			return Execute(command, arguments);

		}
		public Process Execute(SvnLookCommand command, String arguments)
		{
			Trace.WriteLine(String.Format("Excuting {0} with arguments: {1}", command, arguments));
			arguments = String.Concat(
				GetCommandString(command), " \"", 
				RepositoryPath, "\" ", 
				arguments);
			
			ProcessStartInfo info = new ProcessStartInfo();
			info.Arguments = arguments;
			info.FileName = SvnLookPath;
			// Required for redirecting standard output
			info.UseShellExecute = false; 
			// Required to be able to catch the output
			info.RedirectStandardOutput = true;
			// Required to keep the called program from 
			// writing errors that destroy formatting
			// of *our* error messages.
			info.RedirectStandardError = true; 

			return Process.Start(info);
		}

		private static String GetCommandString(SvnLookCommand command)
		{

			switch (command)
			{
				case SvnLookCommand.Author:
					return "author";

				case SvnLookCommand.Cat:
					return "cat";

				case SvnLookCommand.Changed:
					return "changed";

				case SvnLookCommand.Date:
					return "date";

				case SvnLookCommand.Diff:
					return "diff";

				case SvnLookCommand.DirsChanged:
					return "dirs-changed";

				case SvnLookCommand.History:
					return "history";

				case SvnLookCommand.Info:
					return "info";

				case SvnLookCommand.Lock:
					return "lock";

				case SvnLookCommand.Log:
					return "log";

				case SvnLookCommand.PropertyGet:
					return "propget";

				case SvnLookCommand.PropertyList:
					return "proplist";

				case SvnLookCommand.Tree:
					return "tree";

				case SvnLookCommand.UUID:
					return "uuid";

				case SvnLookCommand.Youngest:
					return "youngest";

				default:
					throw new ArgumentOutOfRangeException("command");
			}

		}
		

		private readonly String SvnLookPath;
		private readonly String RepositoryPath;

	}
}
