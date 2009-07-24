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
// limitations under the License

namespace Castle.Facilities.WcfIntegration
{
	using System;

	/// <summary>
	/// Base implementation for <see cref="IWcfPolicy"/>
	/// The lower the <see cref="ExecutionOrder"/> the higher priority.
	/// </summary>
	public abstract class AbstractWcfPolicy : IWcfPolicy 
	{
		public AbstractWcfPolicy()
		{
			ExecutionOrder = Int32.MaxValue;
		}

		public int ExecutionOrder { get; set; }
	}
}
