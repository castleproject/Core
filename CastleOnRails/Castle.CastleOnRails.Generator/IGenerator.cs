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

namespace Castle.CastleOnRails.Generator
{
	using System;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// Abstract a generator implementation
	/// </summary>
	public interface IGenerator
	{
		/// <summary>
		/// Ask the generator instance if is support the 
		/// arguments specified on the command line.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="options"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		bool Accept(String name, IDictionary options, TextWriter output);

		/// <summary>
		/// If accepted, ask it to execute
		/// </summary>
		/// <param name="options"></param>
		/// <param name="output"></param>
		void Execute(IDictionary options, TextWriter output);
	}
}
