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

namespace Apache.Avalon.Framework{	using System;	/// <summary>	/// The Suspendable interface is used when a component will need to	/// temporarily halt execution of a component. The execution may be halted	/// so that you can take snapshot metrics of the component.	/// </summary>	public interface ISuspendable	{		/// <summary>		///   Suspends the component.		/// </summary>		void Suspend();		/// <summary>		///   Resumes the component.		/// </summary>		void Resume();	}}
