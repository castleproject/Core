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
	using System.IO;

	/// <summary>
	/// This is the interface that is used
	/// to access the repository directly.
	/// </summary>
	public interface IRepository
	{
		/// <summary>
		/// This function returns a stream to the file
		/// contents, the stream must allow seeking
		/// and reading.
		/// </summary>
		/// <param name="path">The path within the repository for the file</param>
		Stream GetFileContents(String path);
		/// <summary>
		/// Many properties can have more than one value, 
		/// in which case a list will be returned. If no
		/// property has been set null is returned.
		/// </summary>
		/// <param name="path">The path to get a property from</param>
		/// <param name="property">The property to get from the path</param>
		/// <returns>The property value(s)</returns>
		String[] GetProperty(String path, String property);
	}
}
