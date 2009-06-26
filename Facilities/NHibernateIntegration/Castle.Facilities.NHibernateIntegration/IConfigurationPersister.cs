// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.NHibernateIntegration
{
	using System.Collections.Generic;
	using NHibernate.Cfg;

	/// <summary>
	/// Knows how to read/write an NH <see cref="Configuration"/> from
	/// a given filename, and whether that file should be trusted or a new
	/// Configuration should be built.
	/// </summary>
	public interface IConfigurationPersister
	{
		/// <summary>
		/// Gets the <see cref="Configuration"/> from the file.
		/// </summary>
		/// <param name="filename">The name of the file to read from</param>
		/// <returns>The <see cref="Configuration"/></returns>
		Configuration ReadConfiguration(string filename);

		/// <summary>
		/// Writes the <see cref="Configuration"/> to the file
		/// </summary>
		/// <param name="filename">The name of the file to write to</param>
		/// <param name="cfg">The NH Configuration</param>
		void WriteConfiguration(string filename, Configuration cfg);

		/// <summary>
		/// Checks if a new <see cref="Configuration"/> is required or a serialized one should be used.
		/// </summary>
		/// <param name="filename">Name of the file containing the NH configuration</param>
		/// <param name="dependencies">Files that the serialized configuration depends on. </param>
		/// <returns>If the <see cref="Configuration"/> should be created or not.</returns>
		bool IsNewConfigurationRequired(string filename, IList<string> dependencies);
	}
}