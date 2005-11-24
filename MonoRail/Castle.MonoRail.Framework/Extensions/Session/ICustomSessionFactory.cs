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

namespace Castle.MonoRail.Framework.Extensions.Session
{
	using System.Collections;

	/// <summary>
	/// Contract used by <see cref="CustomSessionExtension"/>
	/// to obtain the session implementation provided by the programmer
	/// </summary>
	public interface ICustomSessionFactory
	{
		/// <summary>
		/// Should identify the session using the context (usually a cookie is used
		/// for that)
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		IDictionary ObtainSession(IRailsEngineContext context);

		/// <summary>
		/// Should persist the session state associated with the context (again, a cookie 
		/// is the standard approach to identify the session)
		/// </summary>
		/// <param name="session"></param>
		/// <param name="context"></param>
		void PersistSession(IDictionary session, IRailsEngineContext context);
	}
}
