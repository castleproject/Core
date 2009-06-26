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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using NHibernate.Cfg;
	using NHibernate.Event;
	using NHibernate.Search.Event;
	using System.Collections.Generic;

	/// <summary>
	/// Contributor to add the NHSearch event listeners
	/// </summary>
	public class NHSearchContributor : AbstractNHContributor
	{

		/// <summary>
		/// The actual contribution method.
		/// </summary>
		/// <param name="configuration">The configuration to be modified.</param>
		public override void Contribute(Configuration configuration)
		{
			if (configuration.Properties.ContainsKey("hibernate.search.analyzer"))
			{
				configuration.SetListeners(ListenerType.PostDelete, AddListenerTo(typeof(IPostDeleteEventListener), configuration.EventListeners.PostDeleteEventListeners));
				configuration.SetListeners(ListenerType.PostInsert, AddListenerTo(typeof(IPostInsertEventListener), configuration.EventListeners.PostInsertEventListeners));
				configuration.SetListeners(ListenerType.PostUpdate, AddListenerTo(typeof(IPostUpdateEventListener), configuration.EventListeners.PostUpdateEventListeners));
			}
		}

		private object[] AddListenerTo(Type targetType, object[] eventListeners)
		{
			var list = new List<object>(eventListeners);
			list.Add(new FullTextIndexEventListener());
			var array = Array.CreateInstance(targetType, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				array.SetValue(list[i],i);
			}
			return (object[])array;
		}
	}
}
