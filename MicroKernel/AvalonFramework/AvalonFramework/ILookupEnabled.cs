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
// limitations under the License.

namespace Apache.Avalon.Framework{	using System;

	/// <summary>	/// A Serviceable is a class that need to connect to software components	/// or obtain other context information using a name, thus not depending	/// on particular implementations but on behavioral interfaces. The	/// contract surrounding a ILookupEnabled is that it is a user. The	/// ILookupEnabled is able to use Objects managed by the ILookupManager	/// it was initialized with. As part of the contract with the system, the	/// instantiating entity (container) must call the enableLookups method	/// before the ILookupEnabled can be considered valid.	/// </summary>	public interface ILookupEnabled	{		/// <summary>		/// Pass the ILookupManager to the ILookupEnabled. The ILookupEnabled		/// implementation should use the specified ILookupManager to acquire		/// the components it needs for execution.		/// </summary>		/// <param name="manager">The lookup manager</param>		/// <exception cref="LookupException">If a required object could not be found.</exception>		void EnableLookups( ILookupManager manager );	}}