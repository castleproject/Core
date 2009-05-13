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

namespace Castle.MicroKernel.Proxy
{
	using System;

	/// <summary>
	/// Holds the keys used by the proxy factories.
	/// </summary>
	public abstract class ProxyConstants
	{
		/// <summary>
		/// Key used to supply custom proxy options.
		/// </summary>
		public static readonly String ProxyOptionsKey = "proxy.options";
	}
}