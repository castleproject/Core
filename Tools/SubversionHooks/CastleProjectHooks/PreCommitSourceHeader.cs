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

namespace Castle.SvnHooks.CastleProject
{
	using System;	
	using System.IO;
	using System.Collections;
	
	using Castle.SvnHooks;

	/// <summary>
	/// At its current state this implementation
	/// is very naive, it does not allow for extra -
	/// or less - blankspace, which would make it
	/// very hard to use.
	/// </summary>
	public class PreCommitSourceHeader : IPreCommit
	{

		/// <summary>
		/// The header that must exist before any other text
		/// in the files checked.
		/// </summary>
		/// <remarks>
		/// Extra spaces are allowed in the header, but they are 
		/// stripped out in the static constructor to speed up
		/// the line-by-line comparsions.
		/// </remarks>
		private static readonly String[] LINES = new String[]
			{
				@"// Copyright 2004-2005 Castle Project - http://www.castleproject.org/",
				@"//",
				 "// Licensed under the Apache License, Version 2.0 (the \"License\");", 
				@"// you may not use this file except in compliance with the License.",
				@"// You may obtain a copy of the License at",
				@"//",
				@"//     http://www.apache.org/licenses/LICENSE-2.0",
				@"//",
				@"// Unless required by applicable law or agreed to in writing, software",
				 "// distributed under the License is distributed on an \"AS IS\" BASIS,",
				@"// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.",
				@"// See the License for the specific language governing permissions and",
				@"// limitations under the License."
			};

		static PreCommitSourceHeader()
		{
			for(int i = 0; i<LINES.Length; i++)
			{
				LINES[i] = Trim(LINES[i]);
			}
		}

		public PreCommitSourceHeader(String extensions) : this(extensions.Split(' ', '|'))
		{			
		}
		public PreCommitSourceHeader(String[] extensionStrings)
		{
			this.extensions = extensionStrings;
		}


		public Error[] PreCommit(RepositoryFile file)
		{
			// Check files that have the proper extension
			// but skip the files that are deleted
			if (file.ContentsStatus == RepositoryStatus.Deleted ||
				Array.IndexOf(extensions, file.Extension) == -1)
			{
				return Error.NoErrors;
			}

			// Skip AssemblyInfo.cs, it is a special
			// case file and should not be necessary
			// to add the header to. I think?
			if(file.FileName == "AssemblyInfo.cs")
				return Error.NoErrors;

			ArrayList errors = new ArrayList();

			using(Stream stream = file.GetContents())
			using(TextReader reader = new StreamReader(stream))
			{
				String line;
				int index = 0;
				while ((line = reader.ReadLine()) != null &&
						index < LINES.Length)
				{
					line = Trim(line);

					if (line == String.Empty)
					{
						if(index != 0)
						{
							AddError(errors, file, "Blank lines are not allowed in the Apache License 2.0 header");
						}

						continue;
					}

					if (line != LINES[index])
					{
						if(index == 0)
						{
							AddError(errors, file, "No text or code is allowed before the Apache License 2.0 header");
							continue;
						}
						AddError(errors, file, "Apache License 2.0 header has errors on line " + (index+1));
					}

					index++;
				}

				// if index is still at 0 we havent found the
				// header at all, clear the errors and add
				// that as error.
				if(index == 0)
				{
					errors.Clear();
					AddError(errors, file, "Apache License 2.0 header is missing or there are errors on the first line");
				}
			}

			return (Error[])errors.ToArray(typeof(Error));
		}

		Error[] Castle.SvnHooks.IPreCommit.PreCommit(RepositoryDirectory directory)
		{
			// Directories, for obvious reasons, cannot contain the copyright
			// notice at the top of its contents.
			return Error.NoErrors;
		}


		private static String Trim(String s)
		{
			return System.Text.RegularExpressions.Regex.Replace(s.Trim(), @"\s+", " ");
		}

		private static int DetermineLine(String s)
		{
			for(int i = 0; i<LINES.Length; i++)
			{
				if(s == Trim(LINES[i]))
					return i;
			}
			return -1;
		}

		private static void AddError(IList errors, RepositoryFile file, String description)
		{
			Error error = new Error(file, description);
			if (!errors.Contains(error))
				errors.Add(error);
		}


		private String[] extensions;
	}
}
