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

namespace Castle.ActiveRecord.Tests.Model
{
	using System;
	using NHibernate.Event;

	public class NonAttributedPreLoadListener: IPreLoadEventListener
	{
		public void OnPreLoad(PreLoadEvent @event){}
	}

	public class TestLoadListener : ILoadEventListener
	{
		public void OnLoad(LoadEvent @event, LoadType loadType){}
	}

	public class MultipleEventListener: IPreInsertEventListener, IPreUpdateEventListener
	{
		public bool OnPreInsert(PreInsertEvent @event)
		{
			return true;
		}

		public bool OnPreUpdate(PreUpdateEvent @event)
		{
			return true;
		}
	}
}