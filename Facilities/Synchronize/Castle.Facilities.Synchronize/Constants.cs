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

namespace Castle.Facilities.Synchronize
{
	using System;

	/// <summary>
	/// Exposes constants used by the facility and its internal 
	/// components
	/// </summary>
	public class Constants
	{
		/// <summary>
		/// Configuration attribute to enable synchronization.
		/// </summary>
		public static readonly String SynchronizedAttrib = "synchronized";

		/// <summary>
		/// Configuration attribute to specify context key reference.
		/// </summary>
		public static readonly String ContextRefAttribute = "contextRef";

		/// <summary>
		/// Configuration attribute to specify context service reference.
		/// </summary>
		public static readonly String ContextTypeAttribute = "contextType";

		/// <summary>
		/// Configuration attribute to specify a custom control proxy hook.
		/// </summary>
		public static readonly String ControlProxyHookAttrib = "controlProxyHook";

		internal static readonly String WinFormsSyncContext = "sync.winforms.ctx";
		internal static readonly String MarshalControl = "sync.marshal.control";
		internal static readonly String CustomActivator = "sync.custom.activator";
	}
}