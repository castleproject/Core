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

namespace Castle.Facilities.WcfIntegration.Behaviors
{
	using System.ServiceModel;

	public abstract class AbstractExtension<T> : IExtension<T>
		where T : class, IExtensibleObject<T>
	{
		private T owner;

		public AbstractExtension()
		{
			ExecutionOrder = int.MaxValue;
		}

		public int ExecutionOrder { get; set; }

		public AbstractExtension<T> ExecuteAt(int order)
		{
			ExecutionOrder = order;
			return this;
		}

		public virtual void Attach(T owner)
		{
			this.owner = owner;
		}

		public virtual void Detach(T owner)
		{
			this.owner = null;
		}

		protected T Owner
		{
			get { return owner; }
		}
	}
}
