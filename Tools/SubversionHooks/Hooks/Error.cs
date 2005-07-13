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
	using System.Text;

	/// <summary>
	/// Summary description for Error.
	/// </summary>
	public class Error
	{
		public static readonly Error[] NoErrors = new Error[]{};

		public Error(RepositoryFile file, String description)
		{
			this.file = file;
			this.description = description;
		}

		
		public override string ToString()
		{

			StringBuilder sb = new StringBuilder();

			if (file != null)
				sb.AppendFormat("Error in file: {0}\n", file.Path);

			sb.Append(Description);

			return sb.ToString();

		}

		public override bool Equals(object obj)
		{
			if (obj is Error)
			{
				return Description == ((Error)obj).Description &&
					   File.Equals(((Error)obj).File);
			}
			return base.Equals(obj);
		}


		public String Description
		{
			get { return description; }
		}
		public RepositoryFile File
		{
			get { return file; }
		}


		private String description;
		private RepositoryFile file;
	}
}
