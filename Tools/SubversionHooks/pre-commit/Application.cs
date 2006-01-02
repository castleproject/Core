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

namespace Castle.SvnHooks
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Diagnostics;
	using System.Text.RegularExpressions;
	
	using Castle.MicroKernel;
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Configuration.Sources;

	using Castle.SvnHooks;

	/// <summary>
	/// Summary description for Application.
	/// </summary>
	public class Application
	{

		private static bool HooksAllow(RepositoryFile file, IPreCommit[] hooks)
		{
			bool hooksOK = true;
			foreach (IPreCommit hook in hooks)
			{
				Error[] errors = hook.PreCommit(file);
				if(errors != null && errors.Length > 0)
				{
					hooksOK = false;
					Console.Error.WriteLine("An error occured in \"{0}\"", file);
					foreach(Error error in errors)
					{						
						Console.Error.WriteLine(error.Description);
					}
				}
			}

			return hooksOK;
		}

		private static IPreCommit[] FetchHooks(IKernel kernel)
		{
			ArrayList list = new ArrayList();
			foreach (IHandler handler in kernel.GetHandlers(typeof(IPreCommit)))
			{
				IPreCommit hook = (IPreCommit)handler.Resolve();
				list.Add(hook);
			}

			return (IPreCommit[])list.ToArray(typeof(IPreCommit));
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			try
			{
				// Fetch paths and transaction from configuration and arguments
				String svnlookPath = ConfigurationSettings.AppSettings["svnlook.location"];
				String repositoryPath = args[0];
				String transactionString = args[1];
				Transaction transaction = Transaction.Parse(transactionString);
						
				// Initialize a WindsorContainer with all the relevant hooks
				// and repository.
				WindsorContainer container = new WindsorContainer(new XmlInterpreter(new AppDomainConfigSource("castle")));
				IRepository repository = new DefaultRepository(
					svnlookPath, 
					repositoryPath, 
					transaction);

				container.Kernel.AddComponentInstance("Repository", typeof(IRepository), repository);

				// Fetch all the hooks from the container, if we get none
				// we dont need to process the files in the transaction
				// and can simply exit the program with success at this point.
				IPreCommit[] hooks = FetchHooks(container.Kernel);

				if (hooks.Length == 0)
					return 0;

				// Start processing all the files commited and run the hooks
				// on them. This is done by executing a changed command on
				// svnlook and parsing the result for the files/directories.
				bool hooksOK = true;
				Regex lineRegex = new Regex("^(?<contents>[AUD_])(?<properties>[U ]) +(?<file>[^ ].*) *$");
				SvnLook svnLook = new SvnLook(svnlookPath, repositoryPath);
				using(Process process = svnLook.Execute(SvnLookCommand.Changed, transaction, null))
				{
					String line;
					while((line = process.StandardOutput.ReadLine()) != null)
					{
						Match m = lineRegex.Match(line);

						if(!m.Success)
						{
							Console.Error.WriteLine("Could not match line: " + line);
							return 3;
						}
						
						if(m.Groups["file"].Value.EndsWith("/"))
						{
							// This is a directory, not a file.
							
							// TODO: Add directory handling
						}
						else
						{
							RepositoryStatus contentsStatus;
							RepositoryStatus propertiesStatus;

							switch (m.Groups["contents"].Value)
							{
								case "A": 
									contentsStatus = RepositoryStatus.Added;
									break;
								case "U":
									contentsStatus = RepositoryStatus.Updated;
									break;
								case "D":
									contentsStatus = RepositoryStatus.Deleted;
									break;
								case "_":
									contentsStatus = RepositoryStatus.Unchanged;
									break;
								default:
									Console.Error.WriteLine("Could not match status flags for contents on line: " + line);
									return 3;
							}
							switch (m.Groups["properties"].Value)
							{
								case "U":
									propertiesStatus = RepositoryStatus.Updated;
									break;
								case " ":
									propertiesStatus = RepositoryStatus.Unchanged;
									break;
								default:
									Console.Error.WriteLine("Could not match status flags for properties on line: " + line);
									return 3;
							}

							using(RepositoryFile file = new RepositoryFile(repository, m.Groups["file"].Value, contentsStatus, propertiesStatus))
							{

								// If HooksAllow returns false we should not allow
								// the transaction, but rather than exiting we store
								// that fact in a boolean so that the user will be
								// told all errors his files contain rather than just
								// one at a time.

								hooksOK &= HooksAllow(file, hooks);

							}

						}
						
					}

					if (!hooksOK)
						return 1;

				}

				return 0;
			}
			catch(Exception e)
			{				
				Console.Error.WriteLine("An uncaught exception was thrown in the hook implementation");
				Console.Error.WriteLine(e.Message);
				return 2;
			}
		}

	}
}
