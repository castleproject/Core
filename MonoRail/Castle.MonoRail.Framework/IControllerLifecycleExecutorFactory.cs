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
	/// <summary>
	/// Depicts the contract for a 
	/// <see cref="IControllerLifecycleExecutor"/> factory.
	/// </summary>
	public interface IControllerLifecycleExecutorFactory
	{
		/// <summary>
		/// Creates an executor instance
		/// </summary>
		/// <returns>An <see cref="IControllerLifecycleExecutor"/> 
		/// implementation</returns>
		/// <param name="controller">Controller instance (cannot be null)</param>
		/// <param name="context">Request context (cannot be null)</param>
		IControllerLifecycleExecutor CreateExecutor(Controller controller, IRailsEngineContext context);
	}
}
