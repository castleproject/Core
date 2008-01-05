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
	/// <summary>
	/// Pendent
	/// </summary>
	public class SessionlessMonoRailHttpHandler : BaseHttpHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SessionlessMonoRailHttpHandler"/> class.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public SessionlessMonoRailHttpHandler(IEngineContext engineContext, IController controller, IControllerContext context)
			: base(engineContext, controller, context, true)
		{
		}

		/// <summary>
		/// Overriden to prevent acquiring a session from the custom session
		/// </summary>
		protected override void AcquireCustomSession()
		{
		}

		/// <summary>
		/// Overriden to prevent persisting a session to the custom session
		/// </summary>
		protected override void PersistCustomSession()
		{
		}
	}
}
