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
	using System.Collections;
	using System.IO;	
	using System.Diagnostics;

	/// <summary>
	/// Summary description for DefaultRepository.
	/// </summary>
	public class DefaultRepository : IRepository
	{
		private DefaultRepository(String svnLookPath, String repositoryPath)
		{
			this.SvnLook = new SvnLook(svnLookPath, repositoryPath);
		}
		public DefaultRepository(String svnLookPath, String repositoryPath, int revision)
			: this(svnLookPath, repositoryPath)
		{
			this.RevisionOrTransaction = revision;
		}
		public DefaultRepository(String svnLookPath, String repositoryPath, Transaction transaction)
			: this(svnLookPath, repositoryPath)
		{
			this.RevisionOrTransaction = transaction;
		}

		
		#region IRepository Members

		public virtual System.IO.Stream GetFileContents(String path)
		{
			using(Process proc = SvnGetFileContents(path))
			{
				MemoryStream stream = new MemoryStream();
				using(StreamWriter writer = new StreamWriter(stream))
				{
					String line;
					while((line = proc.StandardOutput.ReadLine()) != null)
					{
						writer.WriteLine(line);
					}
				}

				return new MemoryStream(stream.ToArray(), false);
			}
		}

		public virtual String[] GetProperty(String path, String property)
		{
			ArrayList properties = new ArrayList();
			using(Process proc = SvnGetProperty(path, property))
			{
				String line;
				while((line = proc.StandardOutput.ReadLine()) != null)
				{
					// Any line starting with svnlook: is an error message.
					// Technically you might get that from a log message,
					// but thats not considered in the current implementation.
					if (line.StartsWith("svnlook:"))
					{
						Console.Error.WriteLine(line);
						return null;                     
					}

					properties.Add(line);
				}
			}

			// If we have no properties in the list
			// then return null, otherwise return the 
			// properties in an array.
			if (properties.Count > 0)
				return (String[]) properties.ToArray(typeof(String));
			return null;
		}


		#endregion

		protected Process SvnGetProperty(String path, String property)
		{
			String arguments = String.Concat(property, " \"", path, "\"");

			return SwitchedExecuteSvnLook(SvnLookCommand.PropertyGet, arguments);
		}
		protected Process SvnGetFileContents(String path)
		{
			return SwitchedExecuteSvnLook(SvnLookCommand.Cat, String.Concat('"', path, '"'));
		}


		private Process SwitchedExecuteSvnLook(SvnLookCommand command, String arguments)
		{
			if(RevisionOrTransaction is int)
			{
				return SvnLook.Execute(command, (int)RevisionOrTransaction, arguments);
			}
			else if(RevisionOrTransaction is Transaction)
			{
				return SvnLook.Execute(command, (Transaction)RevisionOrTransaction, arguments);
			}
			// Due to constructor logic we can never actually reach this point
			throw new InvalidOperationException("RevisionOrTransaction is not a valid type");
		}
		

		private readonly ValueType RevisionOrTransaction;
		private readonly SvnLook SvnLook;
	}
}
