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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;


	public class ActiveRecordModelCollection : DictionaryBase, IEnumerable
	{
		public ActiveRecordModelCollection()
		{
		}

		public void Add(ActiveRecordModel model)
		{
			Dictionary.Add(model.Type, model);
		}

		public bool Contains(Type type)
		{
			return Dictionary.Contains(type);
		}

		public ActiveRecordModel this[Type type]
		{
			get { return Dictionary[type] as ActiveRecordModel; }
		}

		#region IEnumerable Members

		public new IEnumerator GetEnumerator()
		{
			return Dictionary.Values.GetEnumerator();
		}

		#endregion
	}
}