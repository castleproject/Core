// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System.IO;

	/// <summary>
	/// Represents a view template source
	/// </summary>
	public interface IViewSource
	{
		/// <summary>
		/// Opens the view stream.
		/// </summary>
		/// <returns></returns>
		Stream OpenViewStream();

		/// <summary>
		/// Gets or sets the last updated.
		/// </summary>
		/// <value>The last updated.</value>
		long LastUpdated { get; set; }

		/// <summary>
		/// Gets the last modified.
		/// </summary>
		/// <value>The last modified.</value>
		long LastModified { get; }

		/// <summary>
		/// Gets a value indicating whether cache is enabled for it.
		/// </summary>
		/// <value><c>true</c> if cache is enabled for it; otherwise, <c>false</c>.</value>
		bool EnableCache { get; }
	}
}
