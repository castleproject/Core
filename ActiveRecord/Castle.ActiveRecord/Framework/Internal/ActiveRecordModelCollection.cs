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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;

	/// <summary>
	/// Map System.Type to their ActiveRecordModel
	/// </summary>
	public class ActiveRecordModelCollection : DictionaryBase, IEnumerable
	{
		/// <summary>
		/// Adds the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		public void Add(ActiveRecordModel model)
		{
			Dictionary.Add(model.Type, model);
		}

		/// <summary>
		/// Determines whether the collection contains the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified type; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(Type type)
		{
			return Dictionary.Contains(type);
		}

		/// <summary>
		/// Gets the <see cref="Castle.ActiveRecord.Framework.Internal.ActiveRecordModel"/> with the specified type.
		/// </summary>
		/// <value></value>
		public ActiveRecordModel this[Type type]
		{
			get { return Dictionary[type] as ActiveRecordModel; }
		}

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		/// </returns>
		public new IEnumerator GetEnumerator()
		{
			return Dictionary.Values.GetEnumerator();
		}

		#endregion
	}
}
