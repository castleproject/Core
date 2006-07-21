// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	/// Depicts the contract the engine has 
	/// to perform the creation and disposal of
	/// <see cref="IHttpHandler"/> instances.
	/// </summary>
	public interface IMonoRailHttpHandlerProvider
	{
		/// <summary>
		/// Implementors should perform their logic to 
		/// return a instance of <see cref="IHttpHandler"/>.
		/// If the <see cref="IHttpHandler"/> can not be created,
		/// it should return <c>null</c>.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		IHttpHandler ObtainMonoRailHttpHandler(IRailsEngineContext context);

		/// <summary>
		/// Implementors should perform their logic 
		/// to release the <see cref="IHttpHandler"/> instance
		/// and its resources.
		/// </summary>
		/// <param name="handler"></param>
		void ReleaseHandler(IHttpHandler handler);
	}
}
