// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Web;

	/// <summary>
	/// Custom attributes can implement this
	/// interface to have a chance to apply
	/// some specific configuration to the 
	/// <see cref="HttpCachePolicy"/>
	/// </summary>
	public interface ICachePolicyConfigurer
	{
		/// <summary>
		/// Implementors should configure 
		/// the specified policy.
		/// </summary>
		/// <param name="policy">The cache policy.</param>
		void Configure(HttpCachePolicy policy);
	}
}
