// Copyright 2003-2004 The Apache Software Foundation
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
// limitations under the License.namespace Apache.Avalon.Framework{	using System;

	///<summary>
	/// This interface should be implemented by classes that need to be	/// configured with custom parameters before initialization. The	/// contract surrounding a Configurable is that the instantiating	/// entity must call the configure method before it is valid.	///</summary>
	public interface IConfigurable	{		/// <summary>		/// This interface should be implemented by classes that need to be		/// configured with custom parameters before initialization. The		/// contract surrounding a Configurable is that the instantiating		/// entity must call the configure method before it is valid.		/// </summary>		/// <param name="config">The configuration object to parse.</param>		/// <exception cref="ConfigurationException">
		/// If there is any problem parsing the configuration object.
		/// </exception>		void Configure( IConfiguration config );	}}
