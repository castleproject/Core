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

namespace Castle.SvnHooks.CastleProject
{
	using System;	
	using System.IO;
	using System.Collections;
	
	using Castle.SvnHooks;
	/// <summary>
	/// This hook controls that a file has its svn:eol-style 
	/// set to native.
	/// </summary>
	public class PreCommitEolStyle : IPreCommit
	{
		public PreCommitEolStyle(String extensions) : this(extensions.Split(' ', '|'))
		{			
		}
		public PreCommitEolStyle(String[] extensionStrings)
		{
			this.extensions = extensionStrings;
		}


		#region IPreCommit Members

		public Error[] PreCommit(RepositoryFile file)
		{
			// Check files that have the proper extension
			// but skip the files that are deleted
			if (file.ContentsStatus == RepositoryStatus.Deleted ||
				Array.IndexOf(extensions, file.Extension) == -1)
			{
				return Error.NoErrors;
			}

			ArrayList errors = new ArrayList();

			String[] props = file.GetProperty("svn:eol-style");

			if (props == null || props.Length == 0 || props[0].ToLower() != "native")
			{
				return new Error[]{new Error(file, "This file must have svn:eol-style set to native")};
			}

			return Error.NoErrors;
		}


		Error[] Castle.SvnHooks.IPreCommit.PreCommit(RepositoryDirectory directory)
		{
			// Directories dont have any svn:eol-style properties
			return null;
		}


		#endregion

		private String[] extensions;
	}
}
