// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Container;

	/// <summary>
	/// This interface can be implemented on the application class (the one
	/// that extends HttpApplication)
	/// </summary>
	public interface IMonoRailContainerEvents
	{
		/// <summary>
		/// Gives implementors a chance to register services into MonoRail's container.
		/// </summary>
		/// <param name="container">The container.</param>
		void Created(IMonoRailContainer container);

		/// <summary>
		/// Gives implementors a chance to get MonoRail's services and uses it somewhere else - 
		/// for instance, registering them on an IoC container.
		/// </summary>
		/// <param name="container"></param>
		void Initialized(IMonoRailContainer container);
	}
}
