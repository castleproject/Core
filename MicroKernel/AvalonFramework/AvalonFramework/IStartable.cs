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

	/// <summary>	/// The Startable interface is used when components need to be "running" to	/// be active. It provides a method through which components can be	/// "started" and "stopped" without requiring a thread. Note that these	/// methods should start the component but return imediately.	/// </summary>	public interface IStartable	{		/// <summary>		/// Starts the component.		/// </summary>		/// <exception cref="Exception">		/// If there is a problem starting the component.		/// </exception>		void Start();		/// <summary>		/// Stops the component.		/// </summary>		/// <exception cref="Exception">		/// If there is a problem stoping the component.		/// </exception>		void Stop();	}}