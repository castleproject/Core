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

	/// <summary>
	/// Summary description for IPreCommit.
	/// </summary>
	public interface IPreCommit
	{

		/// <summary>
		/// This function will be called on every file
		/// being commited, the file will be examined 
		/// on-demand to avoid loading unnecessary 
		/// information that the hooks dont use.
		/// </summary>
		/// <param name="file">
		/// The file that is being commited
		/// </param>
		/// <returns>
		/// null or an empty list if the commit is to 
		/// be allowed, or a list of errors that has 
		/// occured
		/// </returns>
		Error[] PreCommit(RepositoryFile file);

		/// <summary>
		/// This function will be called on every directory
		/// being commited, the directory will be examined 
		/// on-demand to avoid loading unnecessary 
		/// information that the hooks dont use.
		/// </summary>
		/// <param name="directory">
		/// The directory that is being commited
		/// </param>
		/// <returns>
		/// null or an empty list if the commit is to 
		/// be allowed, or a list of errors that has 
		/// occured
		/// </returns>
		Error[] PreCommit(RepositoryDirectory directory);

	}
}
